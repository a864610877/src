using System.IO;
using System.ServiceProcess;
using System.Threading;
using Moonlit.Data;

namespace Ecard.Nurse
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        { 
            if (args.Length == 0)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                                    {
                                        new NurseHost(),
                                    };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                var wait = new EventWaitHandle(false, EventResetMode.ManualReset, "ccc");
                var service1 = new NurseHost();
                service1.Run();
                wait.WaitOne();
                service1.Stop();
            }
        }
    }
}