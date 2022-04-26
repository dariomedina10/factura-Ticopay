using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickBooksTestApp
{
    public class FileLog
    {
        public bool NuevaLinea(string linea)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + "LogConexionQuickbooks.txt";
                if (!File.Exists(path))
                {                    
                    TextWriter tw = File.CreateText(path);
                    tw.WriteLine(linea);
                    tw.Close();
                    return true;
                }
                else if (File.Exists(path))
                {
                    TextWriter tw = new StreamWriter(path, true);
                    tw.WriteLine(linea);
                    tw.Close();
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }
    }
}
