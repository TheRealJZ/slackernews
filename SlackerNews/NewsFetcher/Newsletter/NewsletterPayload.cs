using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFetcher.Newsletter
{
    class NewsletterPayload
    {
        internal List<article> TopArticles { get; set; }
        internal List<NewsletterSection> Sections { get; set; }

        internal string GetSubjectLine()
        {
            string fallbackSubjectLine = "Slacker Weekly for " + DateTimeHelpers.LastWeekFormatted;

            if (TopArticles == null || !TopArticles.Any())
            {
                return fallbackSubjectLine;
            }

            int maxChars = 100;
            int curLength = 0;
            var titles = new List<string>();
            foreach (var a in TopArticles.OrderByDescending(t => t.score))
            {
                curLength += a.title.Length;

                // Characters used to join titles together
                if (titles.Count != 0)
                {
                    curLength += 3;
                }
                
                if (curLength > maxChars)
                {
                    break;
                }

                titles.Add(a.title);
            }

            string subjectLine = string.Join(" — ", titles);
            return !string.IsNullOrWhiteSpace(subjectLine) ? subjectLine : fallbackSubjectLine;
        }
    }
}
