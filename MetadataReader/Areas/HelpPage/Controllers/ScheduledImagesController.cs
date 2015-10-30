using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MetadataReader.Models;

namespace MetadataReader.Areas.HelpPage.Controllers
{
    public class ScheduledImagesController : Controller
    {
        private IMetadataContext _context;

        public ScheduledImagesController(IMetadataContext context)
        {
            _context = context;
        }

        public ActionResult Show(int id)
        {
            var img = _context.ScheduledImages.Include(s => s.MetadataTags).FirstOrDefault(s => s.Id == id);
            return View(img);
        }
    }
}