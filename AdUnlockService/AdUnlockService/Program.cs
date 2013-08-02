using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AdUnlockService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            // Code to run for debugging.

            // Instantiate service.
            AdUnlockService adUnlockService = new AdUnlockService();

            // Call service starter.
            adUnlockService.OnDebug();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new AdUnlockService() 
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
