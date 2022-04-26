using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Common
{
    public static class CertsHelper
    {
        public static string AdmittedCertFileExtension = ".p12";

        public static X509Certificate2 Load(string recourceName, string password, Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = typeof(CertsHelper).Assembly;
            }
            using (var stream = assembly.GetManifestResourceStream(recourceName))
            {
                return new X509Certificate2(ReadStream(stream), password);
            }
        }

        private static byte[] ReadStream(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static bool IsValidX509Certificate2(string certFileName, string password)
        {
            bool result = false;

            if (File.Exists(certFileName) && Path.GetExtension(certFileName) == AdmittedCertFileExtension)
            {
                try
                {
                    var cert = new X509Certificate2(certFileName, password);
                    result = cert.NotAfter > DateTime.Today;
                }
                catch (Exception ex)
                {
                }
            }

            return result;
        }
    }
}
