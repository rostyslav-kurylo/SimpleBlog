using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace SimpleBlog.Controllers
{
    class FeedResult: ActionResult
    {
        private SyndicationFeedFormatter feedFormatter;

        public FeedResult(SyndicationFeedFormatter feedFormatter)
        {
            this.feedFormatter = feedFormatter;
        }
        
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/rss+xml";

            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                feedFormatter.WriteTo(writer);
            }
        }
    }
}
