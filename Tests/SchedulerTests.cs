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
        private IBackgroundJobClient _client;
        private IMetadataContext _context;
        private DbSet<ScheduledImage> _scheduledImages;
        private IPostNotificationSender _postNotificationSender;


        [SetUp]
        public void Setup()
        {
            _client = Mock.Of<IBackgroundJobClient>();

            _context = Mock.Of<IMetadataContext>();
            _scheduledImages = Mock.Of<DbSet<ScheduledImage>>();

            _postNotificationSender = Mock.Of<IPostNotificationSender>();

            var imageMetadataTags = Mock.Of<DbSet<ImageMetadataTag>>();
            Mock.Get(_context).Setup(ctx => ctx.ScheduledImages).Returns(_scheduledImages);
            Mock.Get(_context).Setup(ctx => ctx.ImageMetadataTags).Returns(imageMetadataTags);
        }

        [Test]
        public void SchedulerController_Should_start_background_process_on_post()
        {
            // Arrange
            Mock.Get(_scheduledImages)
                .Setup(si => si.Add(It.IsAny<ScheduledImage>()))
                .Callback<ScheduledImage>((img) =>
                {
                    img.Id = 3;
                });

            var controller = new SchedulerController(_context, _client, _postNotificationSender);

            // Act
            controller.Post(new AssetApiModel() {FileName = "foo", Url = "bar"});
            
            // Assert
            Mock.Get(_client).Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == "DownloadAndReadMetadata" && Convert.ToInt32(job.Args[0]) == 3),
                It.IsAny<EnqueuedState>()));
        }

        [Test]
        public void SchedulerController_Should_save_scheduled_image_entity()
        {
            // Arrange
            var controller = new SchedulerController(_context, _client, _postNotificationSender);

            // Act
            controller.Post(new AssetApiModel()
            {
                FileName = "foo",
                Url = "bar"
            });

            // Assert
            Mock.Get(_scheduledImages).Verify(x => x.Add(It.Is<ScheduledImage>(img => img.FileName == "foo")), "Filename is different from expected.");
            Mock.Get(_scheduledImages).Verify(x => x.Add(It.Is<ScheduledImage>(img => img.CreatedDate != DateTime.MinValue)), "Date was not set.");
        }
        
        [Test]
        public void MetadataReader_Reads_data_from_stream()
        {
            // Arrange
            var fileStream = new FileStream("../../files/img1.tif", FileMode.Open);
            MemoryStream memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            var reader = new CustomMetadataReader();

            // Act
            List<ImageMetadataTag> info = reader.ReadFromStream(memoryStream);

            // Assert
            Assert.That(info, Is.Not.Empty);
        }
    }
}
