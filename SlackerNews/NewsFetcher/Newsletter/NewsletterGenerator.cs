using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFetcher.Newsletter
{
    class NewsletterGenerator
    {
        internal const string TemplateReplacementTag = "##SLACKERWEEKLYREPLACEMENTTAG##";

        internal static NewsletterPayload GetLoadedPayload()
        {
            var payload = new NewsletterPayload
            {
                Sections = new List<NewsletterSection>()
            };

            // Get articles from db
            payload.TopArticles = Repository.GetArticles(10, Repository.Grouping.LastWeek);

            var sections = Repository.GetSections();
            foreach (var s in sections)
            {
                var sectionArticles = Repository.GetArticles((Constants.Section)s.id, 5, Repository.Grouping.LastWeek);
                payload.Sections.Add(new NewsletterSection { Section = s, Articles = sectionArticles });
            }

            return payload;
        }

        internal static string FormatPayload(NewsletterPayload payload)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine(@"<h1 id=""ThisWeekInTech"">The week in tech</h1>");
            sb.AppendLine(@"<p id=""WeekDescription"">" + DateTimeHelpers.LastWeekFormatted + "</p>");

            // Top 10
            sb.AppendLine(@"<div class=""news-section""><h2 class=""section-heading"">Top 10 Hottest Stories</h2>");
            if (payload.TopArticles != null && payload.TopArticles.Any())
            {
                sb.AppendLine(@"<table class=""news-list"">");
                foreach (var a in payload.TopArticles)
                {
                    sb.Append(FormatArticle(a));
                }
                sb.AppendLine("</table>");
            }
            else
            {
                sb.AppendLine("<div>No hot articles this week.</div>");
            }
            sb.AppendLine("</div>");


            // Other Sections
            if (payload.Sections != null && payload.Sections.Any())
            {
                foreach (var sModel in payload.Sections)
                {
                    sb.AppendLine(@"<div class=""news-section""><h2 class=""section-heading"">" + sModel.Section.name + "</h2>");
                    if (sModel.Articles != null && sModel.Articles.Any())
                    {
                        sb.AppendLine(@"<table class=""news-list"">");
                        foreach (var a in sModel.Articles)
                        {
                            sb.Append(FormatArticle(a));
                        }
                        sb.AppendLine("</table>");
                    }
                    else
                    {
                        sb.AppendLine("<div>No articles this week for this section.</div>");
                    }
                    sb.AppendLine("</div>");
                }
            }

            return sb.ToString();
        }

        internal static string FormatArticle(article a)
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"<tr class=""news"">");
                sb.Append(@"<td class=""news-left text-right"">");
                    sb.Append(@"<a href=""" + a.GetCommentsUrl() + @""">");
                    sb.Append(a.score + " pts<br /> " + a.create_datetime.ToString("ddd") + "</a>");
                sb.AppendLine("</td>");
                sb.AppendLine(@"<td class=""news-body""><a href=""" + a.GetUrl() + @""">" + a.title + "</a></td>");
                sb.AppendLine(@"<td class=""news-share""><a href=""" + a.GetShareOnLinkedInUrl() + @""">Share<br /><img alt=""LinkedIn"" src=""http://slackernews.io/content/images/linkedin/In-2CRev-21px-R.png""/></a></td>");
            sb.AppendLine("</tr>");

            return sb.ToString();
        }
    }
}
