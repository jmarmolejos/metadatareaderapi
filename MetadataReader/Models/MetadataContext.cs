using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MetadataReader.Models
{
    public class MetadataContext : DbContext, IMetadataContext
    {
        public MetadataContext() : base("DefaultConnection")
        {
            Database.SetInitializer<MetadataContext>(new DropCreateDatabaseIfModelChanges<MetadataContext>());
        }

        public DbSet<ScheduledImage> ScheduledImages { get; set; }
        public DbSet<ImageMetadataTag> ImageMetadataTags { get; set; }

        void IMetadataContext.SaveChanges()
        {
            base.SaveChanges();
        }

        public void MarkAsModified(object item)
        {
            Entry(item).State = EntityState.Modified;
        }
    }
}