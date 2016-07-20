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
            
            return View(model);
        }

        public ActionResult OldIndex()
        {
            var articles = Repository.GetArticles();
            return View(articles);
        }

        public ActionResult Section(Constants.Section section)
        {
            var sectionEntry = Repository.GetSection(section);
            if(sectionEntry == null)
            {
                return HttpNotFound();
            }
            ViewBag.SectionName = sectionEntry.name;

            var articles = Repository.GetArticles(section);
            return View(articles);
        }
    }
}