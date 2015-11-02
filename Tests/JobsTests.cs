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
        private IMetadataContext _context;
        private DbSet<ScheduledImage> _scheduledImages;

        [SetUp]
        public void Setup()
        {
            _context = Mock.Of<IMetadataContext>();
            _scheduledImages = Mock.Of<DbSet<ScheduledImage>>();

            Mock.Get(_scheduledImages).Setup(si => si.Find(It.IsAny<int>())).Returns(new ScheduledImage());
            Mock.Get(_context).Setup(c => c.ScheduledImages).Returns(_scheduledImages);
        }

        [Test]
        public void DownloadAndReadMetadata_Should_get_scheduled_image_from_db_and_process_it()
        {
            // Arrange
            var downloader = new Mock<IDownloadToStream>();

            var metadataReader = new Mock<ICustomMetadataReader>();
            metadataReader.Setup(m => m.ReadFromStream(It.IsAny<MemoryStream>())).Returns(new List<ImageMetadataTag>());

            var jobsHelper = new JobsHelper(_context, downloader.Object, metadataReader.Object);

            // Act
            jobsHelper.DownloadAndReadMetadata(1);

            // Assert
            Mock.Get(_scheduledImages).Verify(x => x.Find(It.Is<int>(id => id == 1)));
            downloader.Verify(x => x.Download(It.IsAny<string>()));
            metadataReader.Verify(x => x.ReadFromStream(It.IsAny<MemoryStream>()));
        }
    }
}
