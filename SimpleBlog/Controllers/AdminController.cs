using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleBlog.Models;

namespace SimpleBlog.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private SimpleBlogDbContext db = new SimpleBlogDbContext();

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            IEnumerable<Post> posts = db.Posts.OrderByDescending(p => p.CreatedDate);
            // take only first paragraph
            foreach (var post in posts)
                post.Body = post.Body.Substring(0, post.Body.IndexOf("</p>") + 4);

            return View(posts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}