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

            var sections = Repository.GetSections();
            foreach(var s in sections)
            {
                var sectionArticles = Repository.GetArticles((Constants.Section)s.id, 3);
                model.Data.Add(new DisplaySectionModel { Section = s, Articles = sectionArticles });
            }

            // @TODO: replace with more scalable method
            ViewBag.Sections = sections;

            return View(model);
        }

        [OutputCache(Duration = 600, VaryByParam = "section")]
        public ActionResult Section(Constants.Section section)
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

            var articles = Repository.GetArticles(section);
            return View(articles);
        }
    }
}