using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataReader.Models;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class HttpNotificationSenderTests
    {
        private IHttpNotificationSender _sender;
        private IMetadataRepository _repository;

        [SetUp]
        public void SetUp()
        {

            _repository = Mock.Of<IMetadataRepository>();

            _sender = Mock.Of<IHttpNotificationSender>();
        }

        [Test]
        public void HttpNotificationSender_Should_send_request()
        {
            // Arrange
            var postNotificationSender = new PostNotificationSender(_sender, _repository);

            // Act
            postNotificationSender.SendCompleteNotification(3);

            // Assert
            Mock.Get(_sender).Verify(s => s.PostHttpNotificationAsync(It.IsAny<ScheduledImage>()));
        }
    }
}
