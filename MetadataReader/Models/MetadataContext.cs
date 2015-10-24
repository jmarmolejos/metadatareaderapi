using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MetadataReader.Models
{
    public class MetadataContext : DbContext
    {
        public MetadataContext() : base("DefaultConnection")
        {
            Database.SetInitializer<MetadataContext>(new DropCreateDatabaseIfModelChanges<MetadataContext>());
        }
        public DbSet<ImageMetadata> ImageMetadata { get; set; }
    }
}