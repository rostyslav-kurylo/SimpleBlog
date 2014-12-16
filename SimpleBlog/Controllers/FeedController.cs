using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleBlog.Models;
using System.ServiceModel.Syndication;
using System.Web.Routing;

namespace SimpleBlog.Controllers
{
    public class FeedController : Controller
    {
        private SimpleBlogDbContext db = new SimpleBlogDbContext();
        private const int POSTS_IN_FEED = 10;   // post count in the feed

        /// <summary>
        /// Build and show rss feed
        /// </summary>
        /// <returns>FeedResult - application/rss+xml view</returns>
        public ActionResult Rss()
        {
            IEnumerable<SyndicationItem> posts = db.Posts
                .OrderByDescending(p => p.CreatedDate)
                .Take(POSTS_IN_FEED).ToList()
                .Select(p => GetSyndicationItem(p));

            SyndicationFeed feed = new SyndicationFeed("SimpleBlog", "SimpleBlog RSS feed", new Uri(Request.Url.AbsoluteUri), posts);
            Rss20FeedFormatter feedFormatter = new Rss20FeedFormatter(feed);

            return new FeedResult(feedFormatter);
        }

        private SyndicationItem GetSyndicationItem(Post post)
        {
            // build link for each post
            string postLink = string.Format("{0}://{1}{2}",
                Request.Url.Scheme, Request.Url.Authority, Url.Action("Details", "Posts", new { id = post.ID }));

            return new SyndicationItem(post.Title, post.Body, new Uri(postLink));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}