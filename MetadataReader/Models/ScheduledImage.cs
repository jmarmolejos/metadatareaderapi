using System.Collections.Generic;

namespace MetadataReader.Models
{
    public class ScheduledImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string DownloadUrl { get; set; }

        public ICollection<ImageMetadataTag> MetadataTags { get; set; }
    }
}