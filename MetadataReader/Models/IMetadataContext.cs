using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MetadataReader.Models
{
    public interface IMetadataContext
    {
        DbSet<ScheduledImage> ScheduledImages { get; set; }
        DbSet<ImageMetadataTag> ImageMetadataTags { get; set; }
        void SaveChanges();
        void MarkAsModified(object item);
    }
}