using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniversalTrayApp
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "Sincronizador Ticopay", out createdNew))
            {
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Principal());
                }
                else
                {
                    MessageBox.Show("La aplicación del Conector de Ticopay ya esta abierta", "Sincronizador Ticopay", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }
    }
}
