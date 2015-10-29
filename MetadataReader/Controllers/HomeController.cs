using System.Web.Mvc;
using MetadataReader.Models;

namespace MetadataReader.Controllers
{
    public class HomeController : Controller
    {
        private IMetadataContext _context;

        public HomeController(IMetadataContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            var scheduledImages = _context.ScheduledImages;

            ViewBag.Title = "Home Page";

            return View(scheduledImages);
        }
    }
}
