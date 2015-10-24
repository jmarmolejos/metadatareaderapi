using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MetadataReader.Models;

namespace MetadataReader.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var context = new MetadataContext();

            var stuff = context.ImageMetadata.ToList();

            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
