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
                return $"{Math.Round(delta.TotalDays,0)} ago";
            }

            if(delta.Hours <= 1)
            {
                return $"<1h ago";
            }

            return $"{delta.Hours}h ago";
        }
    }
}
