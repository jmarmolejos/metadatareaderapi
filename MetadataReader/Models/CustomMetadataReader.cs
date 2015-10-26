using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataExtractor;

namespace MetadataReader.Models
{
    public class CustomMetadataReader
    {
        public List<ImageMetadataTag> ReadFromStream(MemoryStream stream)
        {
            IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(stream);

            var imageMetadataTags = new List<ImageMetadataTag>();

            foreach (var directory in directories)
            {
                if(directory.HasError)
                    continue;

                var range = directory.Tags.Select(t => new ImageMetadataTag()
                {
                    MetadataValue = t.Description,
                    Name = t.TagName,
                    Type = t.DirectoryName
                });

                imageMetadataTags.AddRange(range);
            }

            return imageMetadataTags;
        }
    }
}