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
            if (TopArticles == null || !TopArticles.Any())
            {
                return "Slacker Weekly for " + DateTimeHelpers.ThisWeekFormatted;
            }

            int maxChars = 150;
            int curLength = 0;
            var titles = new List<string>();
            foreach (var a in TopArticles)
            {
                titles.Add(a.title);

                curLength += a.title.Length + 3;

                if (curLength > maxChars)
                {
                    break;
                }
            }

            return string.Join(" — ", titles);
        }
    }
}
