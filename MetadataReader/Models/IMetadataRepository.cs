using System.Linq;
using System.Data.Entity;

namespace MetadataReader.Models
{
    public interface IMetadataRepository
    {
        ScheduledImage GetScheduledImage(int id);
    }

    public class MetadataRepository : IMetadataRepository
    {
        private IMetadataContext _context;

        public MetadataRepository(IMetadataContext context)
        {
            _context = context;
        }

        public ScheduledImage GetScheduledImage(int id)
        {
            return _context.ScheduledImages.Include(s => s.MetadataTags).FirstOrDefault(i => i.Id == id);
        }
    }
}