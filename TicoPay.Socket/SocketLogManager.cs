using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicoPay.Socket
{
    public static class SocketLogManager
    {
        public static ILog GetLogger()
        {
            if (log4net.LogManager.GetCurrentLoggers().Length == 0)
            {
                // load logger config with XmlConfigurator
                log4net.Config.XmlConfigurator.Configure();
                Console.WriteLine("log4net.Config.XmlConfigurator.Configure();");
            }
            return LogManager.GetLogger("AcWebErrorLogger");
        }
    }
}
