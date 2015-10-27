using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataReader.BackgroundJobs;
using MetadataReader.Models;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class JobsTests
    {
        private Mock<IMetadataContext> _context;

        [SetUp]
        public void Setup()
        {
            var scheduledImages = new Mock<DbSet<ScheduledImage>>();
            _context = new Mock<IMetadataContext>();
            _context.Setup(ctx => ctx.ScheduledImages).Returns(scheduledImages.Object);
        }

        [Test]
        public void DownloadAndReadMetadata_Should_get_image_from_db()
        {
            // Arrange
            var scheduledImages = new Mock<DbSet<ScheduledImage>>();
            scheduledImages.Setup(si => si.Find(It.IsAny<int>())).Returns(new ScheduledImage());
            _context.Setup(ctx => ctx.ScheduledImages).Returns(scheduledImages.Object);

            var downloader = new Mock<IDownloadToStream>();

            var metadataReader = new Mock<ICustomMetadataReader>();

            var jobsHelper = new JobsHelper(_context.Object, downloader.Object, metadataReader.Object);

            // Act
            jobsHelper.DownloadAndReadMetadata(1);

            // Assert
            scheduledImages.Verify(x => x.Find(It.Is<int>(id => id == 1)));
            downloader.Verify(x => x.Download(It.IsAny<string>()));
            metadataReader.Verify(x => x.ReadFromStream(It.IsAny<MemoryStream>()));
        }
    }
}
