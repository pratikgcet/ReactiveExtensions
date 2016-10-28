using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Linq.Expressions;
namespace PowerTradeGenerator
{
    delegate int Calculator(int m, int n);
    public  class Program
    {
        
        static void Main(string[] args)
        {
            if (!args.Contains("asWin32Cmd"))
            {
                var myService = new ServiceBase[]
                {
                    new PowerTradeService()
                };
                ServiceBase.Run(myService);
            }
            else
            {
                Console.WriteLine("Press 'Exit' to stop anytime");
                var app = new PowerTradeService();
                app.Start();
                while (Console.ReadLine().ToString() != "Exit")
                {
                }
                app.Stop();
            }
        }

    }
}
