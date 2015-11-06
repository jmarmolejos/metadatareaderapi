using System;
using System.Collections.Generic;

namespace MetadataReader.Models
{
    public class ScheduledImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string DownloadUrl { get; set; }

        public ICollection<ImageMetadataTag> MetadataTags { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? JobCompletedDate { get; set; }
        public string SuccessNotificationUrl { get; set; }
    }
}