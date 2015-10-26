using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public void Should_start_background_process_on_post()
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
    }
}
