using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeCalculator
{
    public class HelperMethods
    {
        public readonly DateTime RunDateTime;
        public readonly Dictionary<int, string> inputIndexDateMapping = new Dictionary<int, string>();

        public HelperMethods(DateTime runDate, TimeZoneInfo timeZoneInfo)
        {
            RunDateTime = runDate;
            var dateTimeStart =
                new DateTime(runDate.Year, runDate.Month, runDate.Day).Date.AddHours(-1);
            var dateTimeEnd = dateTimeStart.AddDays(1);
            var dateTimeUtcStart = TimeZoneInfo.ConvertTimeToUtc(dateTimeStart, timeZoneInfo);
            var dateTimeUtcEnd = TimeZoneInfo.ConvertTimeToUtc(dateTimeEnd, timeZoneInfo);
            
            var k = 0;
            for (var i = dateTimeUtcStart; i < dateTimeUtcEnd; i = i.AddHours(1.0))
            {
                inputIndexDateMapping.Add(++k,
                    TimeZoneInfo.ConvertTimeFromUtc(i, timeZoneInfo).ToString(@"HH:00"));
            }
        }

       
    }
    
}

