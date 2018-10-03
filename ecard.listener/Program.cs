using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace DealListener
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			    { 
				    new Service1() 
			    };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset, "bbb");
                var service1 = new Service1();
                service1.Run();
                wait.WaitOne();
                service1.Stop();
            }
        }
    }
}
