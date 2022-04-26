using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Asadas.Core.BL;

namespace Ticopay.BNModel.BNArchivoConciliacion
{
    /// <summary>
    /// Archivo de Pagos:  Archivo de Conciliación.
    /// </summary>
    public class RegistroTotales : BnConectividadConciliacion
    {

        public IList<NotasConciliacion> Notas { get; set; }
        public IList<ArchivoConciliacion> Facturas { get; set; }

        public static RegistroTotales ParserArchivoConciliacion(string trama)
        {
            bool listo = false;
            IList<ArchivoConciliacion> factrasTemp = new List<ArchivoConciliacion>();
            IList<NotasConciliacion> notasTemp = new List<NotasConciliacion>();
            int index = 0;
            char letra;
            string registro = "", conca = "";

            for (int i = 0; i < trama.Length; i++)
            {
                registro = trama.Substring(index, 2);
                letra = trama[i];

                if (registro.Equals("01"))
                {
                    if (letra != '\r' && letra != '\n')
                        conca += letra;
                    else
                    {
                        Console.WriteLine(conca);
                        ArchivoConciliacion archivo = ArchivoConciliacion.ParserArchivoConciliacion(conca, ref listo);
                        factrasTemp.Add(archivo);
                        conca = "";
                        i += 1;
                        index += 288;
                    }

                }
                else if (registro.Equals("02") || registro.Equals("03"))
                {
                    if (letra != '\r' && letra != '\n')
                        conca += letra;
                    else
                    {
                        Console.WriteLine(conca);
                        NotasConciliacion archivo = NotasConciliacion.Parsernotasconciliacion(conca, ref listo);
                        notasTemp.Add(archivo);
                        conca = "";
                        i += 1;
                        index += 80;
                    }
                }
            }
            if (!conca.Equals("") || conca == null)
            {
                NotasConciliacion archivo1 = NotasConciliacion.Parsernotasconciliacion(conca, ref listo);
                notasTemp.Add(archivo1);
            }
            RegistroTotales rt = new  RegistroTotales();
            rt.Facturas = factrasTemp;
            rt.Notas = notasTemp;
            return rt;
        }

    }
}
