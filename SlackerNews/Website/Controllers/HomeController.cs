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
        [OutputCache(Duration = 600, VaryByParam = "none")]
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

            // @TODO: replace with more scalable method
            ViewBag.Sections = sections;

            return View(model);
        }

        [OutputCache(Duration = 600, VaryByParam = "none")]
        public ActionResult About()
        {
            // @TODO: replace with more scalable method
            var sections = Repository.GetSections();
            ViewBag.Sections = sections;

            return View();
        }

        [OutputCache(Duration = 600, VaryByParam = "groupBy")]
        public ActionResult Hottest(Repository.Grouping groupBy = Repository.Grouping.ThisWeek)
        {
            // @TODO: replace with more scalable method
            var sections = Repository.GetSections();
            ViewBag.Sections = sections;
            ViewBag.GroupBy = groupBy;

            var articles = Repository.GetArticles(15, groupBy);
            return View(articles);
        }

        [OutputCache(Duration = 600, VaryByParam = "section;groupBy")]
        public ActionResult Section(Constants.Section section, Repository.Grouping groupBy = Repository.Grouping.ThisWeek)
        {
            var sectionEntry = Repository.GetSection(section);
            if(sectionEntry == null)
            {
                return HttpNotFound();
            }
            ViewBag.SectionName = sectionEntry.name;

            // @TODO: replace with more scalable method
            var sections = Repository.GetSections();
            ViewBag.Sections = sections;
            ViewBag.SectionId = (int)section;
            ViewBag.GroupBy = groupBy;

            var articles = Repository.GetArticles(section, 15, groupBy);
            return View(articles);
        }
    }
}