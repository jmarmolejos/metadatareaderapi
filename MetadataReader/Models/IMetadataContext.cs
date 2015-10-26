using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MetadataReader.Models
{
    public interface IMetadataContext
    {
        DbSet<ImageMetadata> ImageMetadata { get; set; }
        void SaveChanges();
        void MarkAsModified(object item);
    }
}