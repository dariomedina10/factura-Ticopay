using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Socket.BL
{
    public static class DateTimeZone
    {
        public const string CentralAmericaStandardTime = "Central America Standard Time";

        public static DateTime Now()
        {
            var myTimeZone = TimeZoneInfo.FindSystemTimeZoneById(CentralAmericaStandardTime);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, myTimeZone);
        }

        public static DateTime MillisToDate(double miliseconds)
        {
            var dateTemp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(miliseconds);

            var myTimeZone = TimeZoneInfo.FindSystemTimeZoneById(CentralAmericaStandardTime);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTemp, myTimeZone);
        }

        public static double ConvertToMillisUnixTimeStamp(DateTime date)
        {
            return date.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}
