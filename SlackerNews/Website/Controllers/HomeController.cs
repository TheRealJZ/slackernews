using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Models;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
#if DEBUG
// Don't cache output when developing
#else
        [OutputCache(Duration = 600, VaryByParam = "none")]
#endif
        public ActionResult Index()
        {
            var model = new HomePageModel
            {
                Data = new List<DisplaySectionModel>()
            };

            model.TopArticles = Repository.GetArticles(10);

            var sections = Repository.GetSections();
            foreach(var s in sections)
            {
                var sectionArticles = Repository.GetArticles((Constants.Section)s.id, 5);
                model.Data.Add(new DisplaySectionModel { Section = s, Articles = sectionArticles });
            }

            SetNavLinks(sections);

            return View(model);
        }

        private void SetNavLinks(List<section> sections = null)
        {
            // @TODO: replace with more scalable method
            if(sections == null)
            {
                sections = Repository.GetSections();
            }
            
            ViewBag.Sections = sections;

            ViewBag.ShowSignupFooter = true;
        }

#if DEBUG
// Don't cache output when developing
#else
        // Cache for 12hours - 60*60*12 = 43,200 seconds
        [OutputCache(Duration = 43200, VaryByParam = "none")]
#endif
        public ActionResult About()
        {
            SetNavLinks();
            return View();
        }

        public ActionResult SlackerWeekly()
        {
            SetNavLinks();
            ViewBag.ShowSignupFooter = false;
            return View();
        }

#if DEBUG
// Don't cache output when developing
#else
        [OutputCache(Duration = 600, VaryByParam = "groupBy")]
#endif
        public ActionResult Hottest(Repository.Grouping groupBy = Repository.Grouping.ThisWeek)
        {
            SetNavLinks();
            ViewBag.GroupBy = groupBy;

            var articles = Repository.GetArticles(15, groupBy);
            return View(articles);
        }

#if DEBUG
// Don't cache output when developing
#else
        [OutputCache(Duration = 600, VaryByParam = "section;groupBy")]
#endif
        public ActionResult Section(Constants.Section section, Repository.Grouping groupBy = Repository.Grouping.ThisWeek)
        {
            var sectionEntry = Repository.GetSection(section);
            if(sectionEntry == null)
            {
                return HttpNotFound();
            }
            ViewBag.SectionName = sectionEntry.name;
                        
            SetNavLinks();
            // @TODO: replace with more scalable method
            ViewBag.SectionId = (int)section;
            ViewBag.GroupBy = groupBy;

            var articles = Repository.GetArticles(section, 15, groupBy);
            return View(articles);
        }

#if DEBUG
        // Don't cache output when developing
#else
        [OutputCache(Duration = 600, VaryByParam = "tag;groupBy")]
#endif
        public ActionResult ByTag(string tag, Repository.Grouping groupBy = Repository.Grouping.ThisWeek)
        {
            // @TODO: replace with more scalable method
            var sections = Repository.GetSections();
            ViewBag.Sections = sections;
            ViewBag.GroupBy = groupBy;
            ViewBag.TagUnsafe = tag;
            ViewBag.TagHtmlEncoded = Server.HtmlEncode(tag);

            var articles = Repository.GetArticlesByTag(tag, 15, groupBy);
            return View(articles);
        }

#if DEBUG
        // Don't cache output when developing
#else
        // Cache for 12hours - 60*60*12 = 43,200 seconds
        [OutputCache(Duration = 43200)]
#endif
        public ActionResult Search()
        {
            SetNavLinks();
            return View();
        }
    }
}