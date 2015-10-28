namespace MetadataReader.Models
{
    public class ImageMetadataTag
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string MetadataValue { get; set; }

        public int ScheduledImageId { get; set; }

        public ScheduledImage ScheduledImage { get; set; }
    }
}