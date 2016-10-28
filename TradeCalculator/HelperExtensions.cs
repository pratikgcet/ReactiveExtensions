using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeCalculator
{
    public static class HelperExtensions
    {
        public static HelperMethods CheckDayLightSaving(this HelperMethods dateTimeHelper, DateTime now, TimeZoneInfo timeZoneInfo)
        {
            var dateCheck =
                new DateTime(dateTimeHelper.RunDateTime.Year, dateTimeHelper.RunDateTime.Month,
                    dateTimeHelper.RunDateTime.Day).Date.AddDays(1);
            if (DateTime.Compare(now, dateCheck) > 0)
            {
                return new HelperMethods(now, timeZoneInfo);
            }
            return dateTimeHelper;
        }
    }
}
