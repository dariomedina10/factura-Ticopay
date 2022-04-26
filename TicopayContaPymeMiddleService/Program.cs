using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TicopayContaPymeMiddleService
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
        #if (DEBUG)
                TicopayMiddleService service = new TicopayMiddleService();
                service.OnDebug();
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        #else            
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new TicopayMiddleService()
                };
                ServiceBase.Run(ServicesToRun);
        #endif

        }
    }
}
