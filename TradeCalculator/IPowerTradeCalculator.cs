using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;
using System.Reactive;
using System.Reactive.Concurrency;

namespace TradeCalculator
{
    public interface IPowerTradeCalculator
    {
        void Stop();
        void RunReport(IPowerService svc,  DateTime dtrunDate, TimeZoneInfo timeZoneInfo,
           int observationIntervalInMinutes, string csvFilePath,StringBuilder ouput,IScheduler scheduler);
    }
}
