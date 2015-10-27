using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using MetadataReader.Controllers;
using MetadataReader.Models;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class SchedulerTests
    {
        private Mock<IBackgroundJobClient> _client;
        private Mock<IMetadataContext> _context;

        [SetUp]
        public void Setup()
        {
            _client = new Mock<IBackgroundJobClient>();

            _context = new Mock<IMetadataContext>();
            var scheduledImages = new Mock<DbSet<ScheduledImage>>().Object;
            var imageMetadataTags = new Mock<DbSet<ImageMetadataTag>>().Object;
            _context.Setup(ctx => ctx.ScheduledImages).Returns(scheduledImages);
            _context.Setup(ctx => ctx.ImageMetadataTags).Returns(imageMetadataTags);
        }

        [Test]
        public void SchedulerController_Should_start_background_process_on_post()
        {
            // Arrange
            var scheduledImages = new Mock<DbSet<ScheduledImage>>();
            scheduledImages
                .Setup(si => si.Add(It.IsAny<ScheduledImage>()))
                .Callback<ScheduledImage>((img) =>
                {
                    img.Id = 3;
                });
            _context.Setup(ctx => ctx.ScheduledImages).Returns(scheduledImages.Object);

            var controller = new SchedulerController(_context.Object, _client.Object);

            // Act
            controller.Post(new AssetApiModel() {FileName = "foo", Url = "bar"});
            
            // Assert
            _client.Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == "DownloadAndReadMetadata" && Convert.ToInt32(job.Args[0]) == 3),
                It.IsAny<EnqueuedState>()));
        }

        [Test]
        public void SchedulerController_Should_save_scheduled_image()
        {
            // Arrange
            var scheduledImages = new Mock<DbSet<ScheduledImage>>();
            _context.Setup(ctx => ctx.ScheduledImages).Returns(scheduledImages.Object);

            var controller = new SchedulerController(_context.Object, _client.Object);

            // Act
            controller.Post(new AssetApiModel()
            {
                FileName = "foo",
                Url = "bar"
            });

            // Assert
            scheduledImages.Verify(x => x.Add(It.Is<ScheduledImage>(img => img.FileName == "foo")));
        }

        [Test]
        public void DownloadToStream_Downloads_file_to_stream()
        {
            // Arrange
            var downloader = new DownloadToStream();

            // Act
            MemoryStream stream = downloader.Download("https://github.com/drewnoakes/metadata-extractor-images/blob/master/tif/Issue%2016.tif?raw=true");

            // Assert
            Assert.That(stream.Length > 0);
            Assert.That(stream.Position, Is.EqualTo(0));
        }

        [Test]
        public void MetadataReader_Reads_data_from_stream()
        {
            // Arrange
            var downloader = new DownloadToStream();
            var reader = new CustomMetadataReader();

            // Act
            List<ImageMetadataTag> info = reader.ReadFromStream(downloader.Download("https://github.com/drewnoakes/metadata-extractor-images/blob/master/tif/Issue%2016.tif?raw=true"));

            // Assert
            Assert.That(info, Is.Not.Empty);
        }
    }
}
