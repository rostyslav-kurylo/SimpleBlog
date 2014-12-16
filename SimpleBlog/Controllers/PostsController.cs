using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SimpleBlog.Models;

namespace SimpleBlog.Controllers
{
    public class PostsController : Controller
    {
        private SimpleBlogDbContext db = new SimpleBlogDbContext();
        private const int PAGE_SIZE = 10;   // post count on the page

        public ActionResult Index(int page = 0)
        {
            IEnumerable<Post> posts = db.Posts
                .OrderByDescending(p => p.CreatedDate)
                .Skip(page * PAGE_SIZE)
                .Take(PAGE_SIZE + 1);

            // take only first paragraph
            foreach (var post in posts)
                post.Body = post.Body.Substring(0, post.Body.IndexOf("</p>") + 4);

            // ViewBag's values for paging
            ViewBag.Page = page;
            ViewBag.IsPrevLinkVisible = page > 0;
            ViewBag.IsNextLinkVisible = posts.Count() > PAGE_SIZE;

            return View(posts.Take(PAGE_SIZE));
        }

        /// <summary>
        /// View full post with comments
        /// </summary>
        /// <param name="id">post id</param>
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Post post = db.Posts.Find(id);
            if (post == null)
                return HttpNotFound();
            
            return View(post);
        }

        /// <summary>
        /// View posts by tags
        /// </summary>
        /// <param name="id">tag value</param>
        public ActionResult Tags(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Tag tag = db.Tags.Where(t => t.Name == id).FirstOrDefault() ?? null;
            if (tag == null)
                return HttpNotFound();

            return View(tag.Posts.OrderByDescending(p => p.CreatedDate));
        }

        /// <summary>
        /// Create new post
        /// </summary>
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create new post
        /// </summary>
        /// <param name="post">post model</param>
        /// <param name="tags">post's tags</param>
        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,CreatedDate,Body,tags")] Post post, string tags)
        {
            if (ModelState.IsValid)
            {
                post.CreatedDate = DateTime.Now;

                tags = tags ?? string.Empty;
                string[] tagNames = tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);  // tags parsing
                foreach (string tagVal in tagNames)
                    post.Tags.Add(GetTag(tagVal));

                db.Posts.Add(post);
                db.SaveChanges();
                
                return RedirectToAction("Index");
            }

            return View(post);
        }

        /// <summary>
        /// Get existing tags from the database. If tag hasn't found - create new
        /// </summary>
        /// <param name="tag">tag value</param>
        private Tag GetTag(string tag)
        {
            return db.Tags.Where(x => x.Name == tag).FirstOrDefault() ?? new Tag { Name = tag };
        }

        /// <summary>
        /// Create new comment
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comment([Bind(Include = "postID, name, message")] int postId, string name, string message)
        {
            Post post = db.Posts.Find(postId);
            Comment comment = new Comment();

            comment.PostID = postId;
            comment.CreatedDate = DateTime.Now;
            comment.Name = name;
            comment.Body = message;
            comment.Posts = post;

            db.Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = postId });
        }

        /// <summary>
        /// Delete comment
        /// </summary>
        /// <param name="id">comment id</param>
        [Authorize]
        public ActionResult DeleteComment(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
                return HttpNotFound();

            db.Comments.Remove(comment);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = comment.PostID });
        }

        /// <summary>
        /// Edit existing post
        /// </summary>
        /// <param name="id">post id</param>
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Post post = db.Posts.Find(id);
            if (post == null)
                return HttpNotFound();

            return View(post);
        }

        /// <summary>
        /// Edit existing post
        /// </summary>
        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,CreatedDate,Body")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = post.ID });
            }

            return View(post);
        }

        /// <summary>
        /// Delete existing post
        /// </summary>
        /// <param name="id">post id</param>
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Post post = db.Posts.Find(id);
            if (post == null)
                return HttpNotFound();

            return View(post);
        }

        /// <summary>
        /// Delete existing post
        /// </summary>
        /// <param name="id">post id</param>
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            
            return RedirectToAction("Index", "Admin");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
