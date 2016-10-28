using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Config;
using Services;

namespace TradeCalculator
{
    public class PowerTradeCalculator:IPowerTradeCalculator
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IDisposable disposableObserver;

        public PowerTradeCalculator()
        {
            XmlConfigurator.Configure();
            Func<int, int> a = (b) =>
            {
                return b;
            };
            
        }
        
        public void RunReport(IPowerService ps, DateTime runTime, TimeZoneInfo timeZoneInfo,
            int pollingTime,  string file,StringBuilder output,IScheduler scheduler)
        {
            
            
            var dateTimeHelper = new HelperMethods(runTime, timeZoneInfo);

            disposableObserver = Observable.Interval(TimeSpan.FromMinutes(pollingTime), scheduler)
                .Select(a =>
                {
                    runTime = runTime.AddMinutes((a + 1) * pollingTime);
                    return Observable.FromAsync(() => ps.GetTradesAsync(runTime));
                })
                .Subscribe(m =>
                {
                    output.Clear();
                    output.AppendLine("Local Time,Volume");
                    m.Catch((PowerServiceException ex) =>
                    {
                        Logger.Error(string.Format("Exception Occured  {0}", ex.Message));
                        return Observable.FromAsync(() => ps.GetTradesAsync(runTime));
                    })
                        .Retry()
                        .SelectMany(a => a.SelectMany(b => b.Periods))
                        .GroupBy(a => a.Period)
                        .Select(a => new { Period = a.Key, Volume = a.Sum(b => b.Volume) })
                        .Subscribe(val =>
                        {
                            val.Volume.Subscribe(vol =>
                            {
                                output.AppendLine(string.Format("{0},{1}",
                                    dateTimeHelper.CheckDayLightSaving(runTime, timeZoneInfo).inputIndexDateMapping[val.Period],
                                    vol));
                                Logger.Info(string.Format("Period {0}, Volume {1}",
                                    val.Period,
                                    vol));
                            });
                        }, delegate
                        {
                            output.AppendLine("Error");
                            Logger.Error("Error");
                        }
                            , async () =>
                            {
                                
                                var csvPath = Path.Combine(file,
                                    "PowerPosition" + runTime.ToString("_yyyyMMdd_") + DateTime.Now.ToString("HHmm") +
                                    ".csv");
                                if (Directory.Exists(file))
                                {
                                    using (var stream = new StreamWriter(csvPath))
                                    {
                                        await stream.WriteAsync(output.ToString());
                                        await stream.FlushAsync();
                                    }

                                    Logger.Info("Completed"  + "\n");
                                }
                                else
                                {
                                    Logger.Error("Completed but Path" +"\n");
                                }
                            });
                });
        }

        public void Stop()
        {
            if (disposableObserver != null)
                disposableObserver.Dispose();
        }
    }
}
