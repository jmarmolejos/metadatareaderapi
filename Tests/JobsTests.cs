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
        private IDownloadToStream _downloader;
        private ICustomMetadataReader _metadataReader;
        private IBackgroundJobClient _client;
        private IPostNotificationSender _postNotificationSender;

        [SetUp]
        public void Setup()
        {
            _context = Mock.Of<IMetadataContext>();
            _scheduledImages = Mock.Of<DbSet<ScheduledImage>>();

            _downloader = Mock.Of<IDownloadToStream>();

            _metadataReader = Mock.Of<ICustomMetadataReader>();

            _client = Mock.Of<IBackgroundJobClient>();

            _postNotificationSender = Mock.Of<IPostNotificationSender>();

            Mock.Get(_scheduledImages).Setup(si => si.Find(It.IsAny<int>())).Returns(new ScheduledImage());
            Mock.Get(_context).Setup(c => c.ScheduledImages).Returns(_scheduledImages);
            Mock.Get(_metadataReader).Setup(m => m.ReadFromStream(It.IsAny<MemoryStream>())).Returns(new List<ImageMetadataTag>());
        }

        [Test]
        public void DownloadAndReadMetadata_Should_get_scheduled_image_from_db_and_process_it()
        {
            // Arrange
            var jobsHelper = new JobsHelper(_context, _downloader, _metadataReader, _client, _postNotificationSender);

            // Act
            jobsHelper.DownloadAndReadMetadata(1);

            // Assert
            Mock.Get(_scheduledImages).Verify(x => x.Find(It.Is<int>(id => id == 1)));
            Mock.Get(_downloader).Verify(x => x.Download(It.IsAny<string>()));
            Mock.Get(_metadataReader).Verify(x => x.ReadFromStream(It.IsAny<MemoryStream>()));
        }

        [Test]
        public void DownloadAndReadMetadata_Should_schedule_http_notification()
        {
            // Arrange
            var jobsHelper = new JobsHelper(_context, _downloader, _metadataReader, _client, _postNotificationSender);

            // Act
            jobsHelper.DownloadAndReadMetadata(1);

            // Assert
            Mock.Get(_client).Verify(c => c.Create(It.IsAny<Job>(), It.IsAny<IState>()));
        }
    }
}
