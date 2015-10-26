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
            var collectionMock = new Mock<DbSet<ImageMetadata>>().Object;
            _context.Setup(ctx => ctx.ImageMetadata).Returns(collectionMock);
        }

        [Test]
        public void SchedulerController_Should_start_background_process_on_post()
        {
            // Arrange
            var controller = new SchedulerController(_context.Object, _client.Object);

            // Act
            controller.Post(new AssetApiModel() {FileName = "foo", Url = "bar"});
            
            // Assert
            _client.Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == "DownloadAndReadMetadata" && job.Args[0] == "fooId"),
                It.IsAny<EnqueuedState>()));
        }

        [Test]
        public void DownloadToStream_Downloads_file_to_stream()
        {
            // Arrange
            var downloader = new DownloadToStream("https://github.com/drewnoakes/metadata-extractor-images/blob/master/tif/Issue%2016.tif?raw=true");

            // Act
            MemoryStream stream = downloader.Download();

            // Assert
            Assert.That(stream.Length > 0);
            Assert.That(stream.Position, Is.EqualTo(0));
        }

        [Test]
        public void MetadataReader_Reads_data_from_stream()
        {
            // Arrange
            var downloader = new DownloadToStream("https://github.com/drewnoakes/metadata-extractor-images/blob/master/tif/Issue%2016.tif?raw=true");
            var reader = new CustomMetadataReader();

            // Act
            List<ImageMetadataTag> info = reader.ReadFromStream(downloader.Download());

            // Assert
            Assert.That(info, Is.Not.Empty);
        }
    }
}
