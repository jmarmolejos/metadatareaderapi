using System.Web.Mvc;
using MetadataReader.Models;

namespace MetadataReader.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var context = new MetadataContext();

            var stuff = context.ScheduledImages;

            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
