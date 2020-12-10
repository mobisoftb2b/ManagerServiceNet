using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServiceNet
{
    static class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            logger.Info("main");
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ManagerService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
