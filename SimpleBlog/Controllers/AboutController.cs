using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleBlog.Models;

namespace SimpleBlog.Controllers
{
    public class AboutController : Controller
    {
        private SimpleBlogDbContext db = new SimpleBlogDbContext();

        public ActionResult Index()
        {
            ViewBag.About = db.Admins.First().About;

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}