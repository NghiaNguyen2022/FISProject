using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ERPService.BackJob;

namespace ERPService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new BackgroundJob()//,
                //new PayooSettlementService(),
                //new ClearService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
