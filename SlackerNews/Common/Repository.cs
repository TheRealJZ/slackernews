using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Repository
    {
        static DateTime Last24
        {
            get
            {
                return DateTime.UtcNow.AddDays(-1);
            }
        }

        public static List<article> GetArticles(int numArticles = 15)
        {
            using (var context = new SlackerNewsEntities())
            {
                return context.articles
                    .Where(t => t.create_datetime > Last24)
                    .OrderByDescending(t => t.score)
                    .Take(numArticles)
                    .ToList();
            }
        } 

        public static List<article> GetArticles(Constants.Section section, int numArticles = 15)
        {
            using (var context = new SlackerNewsEntities())
            {
                return context.articles
                    .Where(t => t.create_datetime > Last24 && t.section_id == (int)section)
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
