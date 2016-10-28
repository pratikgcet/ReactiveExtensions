using System;
using NUnit;
using NUnit.Framework;
using Services;
using Moq;
using Microsoft.Reactive.Testing;
using TradeCalculator;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace PowerTradeTest
{
    [TestFixture]
    public class TradeTest
    {
        private Mock<IPowerService> psMock;
        private TestScheduler scheduler;

        [SetUp]
        public void Initialize()
        {
            scheduler = new TestScheduler();
            psMock = new Mock<IPowerService>();
        }

        [Test]
        public void GivenTrades_WhenGetResuts_ShouldDisplayAggregateLocalTime()
        {   
            var pdc = new PowerTradeCalculator();
            psMock.Setup(p => p.GetTradesAsync(It.IsAny<DateTime>()))
                .Returns(
                    Task.FromResult(CreateMockPowerTrades(It.IsAny<DateTime>(), 2, new[]
                    {
                        new PowerPeriod {Period = 1, Volume = 100},
                        new PowerPeriod {Period = 2, Volume = 200},
                        new PowerPeriod {Period = 3, Volume = 300}
                    }
                        )));
            var date = DateTime.ParseExact("2016/10/27 10:00:00", "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
            var sb = new StringBuilder();
            var gmtTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            const string expected = "Local Time,Volume\r\n23:00,200\r\n00:00,400\r\n01:00,600\r\n";

           
            pdc.RunReport(psMock.Object, date, gmtTimeZoneInfo, 1,  It.IsAny<String>(), sb,scheduler);
            scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
            var actual = sb.ToString();

           
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GivenTrades_WhenException_ThenRetry()
        {
           
            var pdc = new PowerTradeCalculator();
            psMock.SetupSequence(p => p.GetTradesAsync(It.IsAny<DateTime>()))
                .Throws(new PowerServiceException("Thrown from Unit Test"))
                .Returns(
                    Task.FromResult(CreateMockPowerTrades(It.IsAny<DateTime>(), 2, new[]
                    {
                        new PowerPeriod {Period = 1, Volume = 100},
                        new PowerPeriod {Period = 2, Volume = 200},
                        new PowerPeriod {Period = 3, Volume = 300}
                    }
                        )));
            var date = DateTime.ParseExact("2016/10/27 10:00:00", "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
            var sb = new StringBuilder();
            var gmtTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            const string expectedfirst = "Local Time,Volume\r\n23:00,200\r\n00:00,400\r\n01:00,600\r\n";
            
            pdc.RunReport(psMock.Object, date, gmtTimeZoneInfo, 1, It.IsAny<String>(), sb, scheduler);
            scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
            var actual = sb.ToString();
            Assert.AreEqual(expectedfirst, actual);
        }

        [Test]
        public void GivenTrades_WhenDaylightSavingStarts_ShouldHandleCorrectly()
        {
            
            var pdc = new PowerTradeCalculator();
            psMock.Setup(p => p.GetTradesAsync(It.IsAny<DateTime>()))
                .Returns(
                    Task.FromResult(CreateMockPowerTrades(It.IsAny<DateTime>(), 2, new[]
                    {
                        new PowerPeriod {Period = 1, Volume = 10},
                        new PowerPeriod {Period = 2, Volume = 10},
                        new PowerPeriod {Period = 3, Volume = 10},
                        new PowerPeriod {Period = 4, Volume = 10},
                        new PowerPeriod {Period = 5, Volume = 10},
                        new PowerPeriod {Period = 6, Volume = 10},
                        new PowerPeriod {Period = 7, Volume = 10},
                        new PowerPeriod {Period = 8, Volume = 10},
                        new PowerPeriod {Period = 9, Volume = 10},
                        new PowerPeriod {Period = 10, Volume = 10},
                        new PowerPeriod {Period = 11, Volume = 10},
                        new PowerPeriod {Period = 12, Volume = 10},
                        new PowerPeriod {Period = 13, Volume = 10},
                        new PowerPeriod {Period = 14, Volume = 10},
                        new PowerPeriod {Period = 15, Volume = 10},
                        new PowerPeriod {Period = 16, Volume = 10},
                        new PowerPeriod {Period = 17, Volume = 10},
                        new PowerPeriod {Period = 18, Volume = 10},
                        new PowerPeriod {Period = 19, Volume = 10},
                        new PowerPeriod {Period = 20, Volume = 10},
                        new PowerPeriod {Period = 21, Volume = 10},
                        new PowerPeriod {Period = 22, Volume = 10}
                    }
                        )));
            var date = DateTime.ParseExact("2015/03/29", "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var sb = new StringBuilder();
            var gmtTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            const string expected =
                "Local Time,Volume\r\n23:00,20\r\n00:00,20\r\n02:00,20\r\n03:00,20\r\n04:00,20\r\n05:00,20\r\n06:00,20\r\n07:00,20\r\n08:00,20\r\n09:00,20\r\n10:00,20\r\n11:00,20\r\n12:00,20\r\n13:00,20\r\n14:00,20\r\n15:00,20\r\n16:00,20\r\n17:00,20\r\n18:00,20\r\n19:00,20\r\n20:00,20\r\n21:00,20\r\n";

           
            pdc.RunReport(psMock.Object, date, gmtTimeZoneInfo, 1, It.IsAny<String>(), sb, scheduler);
            scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
            var actual = sb.ToString();

           
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GivenTrades_WhenDaylightSavingEnds_ShouldHandleCorrectly()
        {
            
            var pdc = new PowerTradeCalculator();
            psMock.Setup(p => p.GetTradesAsync(It.IsAny<DateTime>()))
                .Returns(
                    Task.FromResult(CreateMockPowerTrades(It.IsAny<DateTime>(), 2, new[]
                    {
                        new PowerPeriod {Period = 1, Volume = 10},
                        new PowerPeriod {Period = 2, Volume = 10},
                        new PowerPeriod {Period = 3, Volume = 10},
                        new PowerPeriod {Period = 4, Volume = 10},
                        new PowerPeriod {Period = 5, Volume = 10},
                        new PowerPeriod {Period = 6, Volume = 10},
                        new PowerPeriod {Period = 7, Volume = 10},
                        new PowerPeriod {Period = 8, Volume = 10},
                        new PowerPeriod {Period = 9, Volume = 10},
                        new PowerPeriod {Period = 10, Volume = 10},
                        new PowerPeriod {Period = 11, Volume = 10},
                        new PowerPeriod {Period = 12, Volume = 10},
                        new PowerPeriod {Period = 13, Volume = 10},
                        new PowerPeriod {Period = 14, Volume = 10},
                        new PowerPeriod {Period = 15, Volume = 10},
                        new PowerPeriod {Period = 16, Volume = 10},
                        new PowerPeriod {Period = 17, Volume = 10},
                        new PowerPeriod {Period = 18, Volume = 10},
                        new PowerPeriod {Period = 19, Volume = 10},
                        new PowerPeriod {Period = 20, Volume = 10},
                        new PowerPeriod {Period = 21, Volume = 10},
                        new PowerPeriod {Period = 22, Volume = 10},
                        new PowerPeriod {Period = 23, Volume = 10},
                        new PowerPeriod {Period = 24, Volume = 10},
                        new PowerPeriod {Period = 25, Volume = 10}
                    }
                        )));
            var date = DateTime.ParseExact("2015/10/25", "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var sb = new StringBuilder();
            var gmtTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            const string expected =
                "Local Time,Volume\r\n23:00,20\r\n00:00,20\r\n01:00,20\r\n01:00,20\r\n02:00,20\r\n03:00,20\r\n04:00,20\r\n05:00,20\r\n06:00,20\r\n07:00,20\r\n08:00,20\r\n09:00,20\r\n10:00,20\r\n11:00,20\r\n12:00,20\r\n13:00,20\r\n14:00,20\r\n15:00,20\r\n16:00,20\r\n17:00,20\r\n18:00,20\r\n19:00,20\r\n20:00,20\r\n21:00,20\r\n22:00,20\r\n";

            
            pdc.RunReport(psMock.Object, date, gmtTimeZoneInfo, 1, It.IsAny<String>(), sb, scheduler);
            scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
            var actual = sb.ToString();

            
            Assert.AreEqual(expected, actual);
        }

        private IEnumerable<PowerTrade> CreateMockPowerTrades(DateTime date, int numTrades, PowerPeriod[] powerPeriods)
        {

            var powerTrades =
                Enumerable.Range(1, numTrades).Select(_ =>
                {
                    var trade = PowerTrade.Create(date, powerPeriods.Count());
                    foreach (var powerperiod in trade.Periods)
                    {
                        powerperiod.Volume =
                            powerPeriods.Single(p => p.Period == powerperiod.Period).Volume;
                    }
                    return trade;
                });

            return powerTrades;
        }
    }
}
