using System;
using System.Collections.Generic;
using System.Linq;
using DT = Common.DateTimeHelpers;

namespace Common
{
    public class Repository
    {  
        public enum Grouping
        {
            ThisWeek,
            LastWeek,
            ThisMonth,
            LastMonth
        }

        static DateTime StartForGrouping(Grouping groupBy)
        {
            // Default to this week
            DateTime start = DT.ThisWeek;

            switch (groupBy)
            {
                case Grouping.LastWeek:
                    start = DT.LastWeek;
                    break;

                case Grouping.ThisMonth:
                    start = DT.ThisMonth;
                    break;

                case Grouping.LastMonth:
                    start = DT.LastMonth;
                    break;
            }

            return start;
        }

        static DateTime EndForGrouping(Grouping groupBy)
        {
            // Default to this week
            DateTime end = DateTime.UtcNow;

            switch (groupBy)
            {
                case Grouping.LastWeek:
                    end = DT.ThisWeek;
                    break;
                    
                case Grouping.LastMonth:
                    end = DT.ThisMonth;
                    break;
            }

            return end;
        }

        public static List<article> GetArticles(int numArticles = 15, Grouping groupBy = Grouping.ThisWeek)
        {
            using (var context = new SlackerNewsEntities())
            {
                DateTime start = StartForGrouping(groupBy);
                DateTime end = EndForGrouping(groupBy);

                return context.articles
                    .Where(t => t.create_datetime > start && t.create_datetime < end)
                    .OrderByDescending(t => t.score)
                    .Take(numArticles)
                    .OrderByDescending(t => t.create_datetime)
                    .ToList();
            }
        } 

        public static List<article> GetArticles(Constants.Section section, int numArticles = 15, Grouping groupBy = Grouping.ThisWeek)
        {
            using (var context = new SlackerNewsEntities())
            {
                DateTime start = StartForGrouping(groupBy);
                DateTime end = EndForGrouping(groupBy);
                
                return context.articles
                    .Where(t => t.create_datetime > start && t.create_datetime < end && t.section_id == (int)section)
                    .OrderByDescending(t => t.score)
                    .Take(numArticles)
                    .OrderByDescending(t => t.create_datetime)
                    .ToList();
            }
        }

        public static List<article> GetArticlesByTag(string tag, int numArticles = 15, Grouping groupBy = Grouping.ThisWeek)
        {
            using (var context = new SlackerNewsEntities())
            {
                DateTime start = StartForGrouping(groupBy);
                DateTime end = EndForGrouping(groupBy);

                string tagStripped = tag.ToLower().Trim();

                return context.articles
                    .Where(t => t.create_datetime > start && t.create_datetime < end 
                        && (
                            t.title.StartsWith(tagStripped + " ")
                            || t.title.Contains(" " + tagStripped + " ")
                            || t.title.EndsWith(" " + tagStripped)))
                    .OrderByDescending(t => t.score)
                    .Take(numArticles)
                    .ToList();
            }
        }

        public static List<section> GetSections()
        {
            using (var context = new SlackerNewsEntities())
            {
                return context.sections.OrderBy(t => t.display_order).ToList();
            }
        }

        public static section GetSection(Constants.Section section)
        {
            using (var context = new SlackerNewsEntities())
            {
                return context.sections.SingleOrDefault(t => t.id == (int)section);
            }
        }
    }
}
