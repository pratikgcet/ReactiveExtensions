using System;
using System.Configuration;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using log4net;
using log4net.Config;
using Services;
using TradeCalculator;
using Microsoft.Practices.Unity;
using System.Reactive;
using System.Reactive.Concurrency;

namespace PowerTradeGenerator
{
    partial class PowerTradeService : ServiceBase
    {   
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IUnityContainer unityContainer;
        private IPowerTradeCalculator csvCalc;

        public PowerTradeService()
        {
            XmlConfigurator.Configure();
            InitializeComponent();
        }

        public void Start()
        {
            ConfigureUunity();
            OnStart(null);
        }

        private void ConfigureUunity()
        {
            unityContainer = new UnityContainer();
            unityContainer.RegisterType<IPowerTradeCalculator, PowerTradeCalculator>();
            unityContainer.RegisterType<IPowerService, PowerService>();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("Starting Power Trade Calculator Service");
            var timeIntervals = int.Parse(ConfigurationManager.AppSettings["IntervalExtract"]);
            var outputPath = ConfigurationManager.AppSettings["CSVPath"];
            csvCalc = unityContainer.Resolve<IPowerTradeCalculator>();
            var powerService = unityContainer.Resolve<IPowerService>();
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            csvCalc.RunReport(powerService, DateTime.Now, timeZone, timeIntervals,
                outputPath,new StringBuilder(),Scheduler.Default);
        }

        protected override void OnStop()
        {
            Logger.Info("Shutting down Trade Calculator Service");
            csvCalc.Stop();
            csvCalc = null;
        }
    }
}
