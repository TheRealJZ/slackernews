using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    partial class article
    {
        public string GetFormattedTimeSinceCreated()
        {
            TimeSpan delta = DateTime.UtcNow - create_datetime;
            int hoursSinceCreated = delta.Hours;

            if(delta.TotalDays >= 1)
            {
                return $"{Math.Round(delta.TotalDays,0)}d ago";
            }

            if(delta.Hours <= 1)
            {
                return $"<1h ago";
            }

            return $"{delta.Hours}h ago";
        }

        public string GetFormattedTimeAsDayOfWeek()
        {
            return create_datetime.ToString("ddd") + create_datetime.ToString("ddMMM");
        }

        public string GetCommentsUrl()
        {
            return "https://news.ycombinator.com/item?id=" + hn_article_id;
        }

        public string GetUrl()
        {
            if(url == null)
            {
                return GetCommentsUrl();
            }

            return url;
        }
    }
}
