using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<article> articles;

            // TODO: Move to repository method
            // TODO: Add filtering capabilities
            using (var context = new SlackerNewsEntities())
            {
                var last24 = DateTime.UtcNow.AddDays(-1);
                articles = context.articles.Where(t => t.create_datetime > last24).OrderByDescending(t => t.score).Take(15).ToList();
            }

            return View(articles);
        }
    }
}