using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirmadorTicopay
{
    static class Program
    {        
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string _tenant = "";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _tenant = ConfigurationManager.AppSettings["tenant"];
            if ((_tenant == null) || (_tenant.Length == 0))
            {
                Application.Run(new Tenant());
                _tenant = ConfigurationManager.AppSettings["tenant"];
                if ((_tenant != null) && (_tenant.Length >= 2))
                {
                    Application.Run(new Login());
                }
            }
            else
            {
                Application.Run(new Login());
            }   
        }
    }
}
