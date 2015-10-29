using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetadataExtractor;
using MetadataReader.Models;

namespace MetadataReader.BackgroundJobs
{
    public class JobsHelper
    {
        private IMetadataContext _context;
        private IDownloadToStream _downloader;
        private ICustomMetadataReader _reader;

        public JobsHelper(IMetadataContext context, 
            IDownloadToStream downloader, 
            ICustomMetadataReader reader)
        {
            _context = context;
            _downloader = downloader;
            _reader = reader;
        }

        public JobsHelper() : this(new MetadataContext(), new DownloadToStream(), new CustomMetadataReader())
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
        }
    }
}