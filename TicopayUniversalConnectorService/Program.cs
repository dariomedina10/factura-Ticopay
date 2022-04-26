using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
        #if (DEBUG)
            UniversalConnectorService service = new UniversalConnectorService();
            service.OnDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        #else
                ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new UniversalConnectorService()
            };
            ServiceBase.Run(ServicesToRun);
        #endif

        }
    }
}
