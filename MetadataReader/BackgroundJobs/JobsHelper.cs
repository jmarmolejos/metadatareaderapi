using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Hangfire;
using MetadataExtractor;
using MetadataReader.Models;

namespace MetadataReader.BackgroundJobs
{
    public class JobsHelper
    {
        private IMetadataContext _context;
        private IDownloadToStream _downloader;
        private ICustomMetadataReader _reader;
        private IBackgroundJobClient _client;
        private IPostNotificationSender _sender;

        public JobsHelper(IMetadataContext context, 
            IDownloadToStream downloader, 
            ICustomMetadataReader reader, IBackgroundJobClient client, 
            IPostNotificationSender sender)
        {
            _context = context;
            _downloader = downloader;
            _reader = reader;
            _client = client;
            _sender = sender;
        }

        // This constructor is awful, implement IoC asap
        public JobsHelper() : this(
            new MetadataContext(), 
            new DownloadToStream(), 
            new CustomMetadataReader(),
            new BackgroundJobClient(), 
            new PostNotificationSender(
                new HttpNotificationSender(), 
                new MetadataRepository(new MetadataContext())))
        {
            
        }

        public void DownloadAndReadMetadata(int imageId)
        {
            // Get scheduled image from db
            var image = _context.ScheduledImages.Find(imageId);
            
            // Download file
            var stream = _downloader.Download(image.DownloadUrl);

            // Read metadata
            var metadata = _reader.ReadFromStream(stream);

            // Save entities

            image.JobCompletedDate = DateTime.UtcNow;
            _context.MarkAsModified(image);

            metadata.ForEach(tag =>
            {
                tag.ScheduledImageId = imageId;
                _context.ImageMetadataTags.Add(tag);
            });
            _context.SaveChanges();

            // Schedule success notification
            _client.Enqueue(() => SendHttpSuccessNotification(image.Id));
        }

        public void SendHttpSuccessNotification(int id)
        {
            Task.FromResult(_sender.SendCompleteNotification(id));
        }
    }
}