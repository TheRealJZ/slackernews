using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Website
{
    public static class RouteExtensionMethods {

        public static void AddCustomRoute(this RouteCollection routes, string routeText, Constants.Section section)
        {
            string routeName = "sections/" + routeText;
            routes.MapRoute(
                name: routeName,
                url: routeName,
                defaults: new { Controller = "Home", action = "Section", section = (int)section });
        }
    }

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.AddCustomRoute("general", Constants.Section.General);
            routes.AddCustomRoute("product-announcements", Constants.Section.ProductAnnouncements);
            routes.AddCustomRoute("business", Constants.Section.Business);
            routes.AddCustomRoute("show-hn", Constants.Section.ShowHN);
            routes.AddCustomRoute("ask-hn", Constants.Section.AskHN);
            routes.AddCustomRoute("persuasion", Constants.Section.Persuasion);
            routes.AddCustomRoute("science", Constants.Section.Science);
            routes.AddCustomRoute("world-news", Constants.Section.WorldNews);
            routes.AddCustomRoute("security-alerts", Constants.Section.Outages);
            
            routes.MapRoute(
                name: "Hottest",
                url: "hottest",
                defaults: new { Controller = "Home", action = "Hottest" });

            routes.MapRoute(
                name: "About",
                url: "about",
                defaults: new { Controller = "Home", action = "About" });

            routes.MapRoute(
                name: "SlackerWeekly",
                url: "slackerweekly",
                defaults: new { Controller = "Home", action = "SlackerWeekly" });

            routes.MapRoute(
                name: "Search",
                url: "search",
                defaults: new { Controller = "Home", action = "Search" });

            routes.MapRoute(
                name: "ByTag",
                url: "tag/{tag}",
                defaults: new { Controller = "Home", action = "ByTag" }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        
    }
}
