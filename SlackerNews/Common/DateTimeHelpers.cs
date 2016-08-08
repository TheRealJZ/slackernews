using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class DateTimeHelpers
    {
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }

        public static DateTime Last24
        {
            get
            {
                return DateTime.UtcNow.AddDays(-1);
            }
        }

        public static DateTime ThisWeek
        {
            get
            {
                // Start from monday
                var now = DateTime.UtcNow;

                if (now.DayOfWeek == DayOfWeek.Monday)
                {
                    return new DateTime(now.Year, now.Month, now.Day);
                }

                if (now.DayOfWeek > DayOfWeek.Monday)
                {
                    return new DateTime(now.Year, now.Month, now.Day).AddDays((int)DayOfWeek.Monday - (int)now.DayOfWeek);
                }

                // Sunday back to previous monday
                return new DateTime(now.Year, now.Month, now.Day).AddDays(-6);
            }
        }

        public static string ThisWeekFormatted
        {
            get
            {
                var thisWeekStart = ThisWeek;
                var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                return thisWeekStart.ToString("ddd, MMM dd") + " - " + thisWeekEnd.ToString("ddd, MMM dd");
            }
        }

        public static DateTime LastWeek
        {
            get
            {
                return ThisWeek.AddDays(-7);
            }
        }

        public static DateTime ThisMonth
        {
            get
            {
                return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            }
        }

        public static DateTime LastMonth
        {
            get
            {
                return ThisMonth.AddMonths(-1);
            }
        }
    }
}
