using System;
using System.Globalization;
using System.Text;
//using Asadas.Core.BL;

namespace Ticopay.BNModel
{
    public class RespuestaBNConectivdad
    {
        /// <summary>
        /// Convierte una cadena en el formato correspondiente segun el parametro TipoDato, Numerico agrega ceros a la izquierda, alfanumerico agrega espacios a la derecha
        /// </summary>
        /// <param name="str">Cadena a convertir</param>
        /// <param name="tamano">Tamaño de la cadena que se debe retornar</param>
        /// <param name="tipo">Tipo de dato, Numerico o Alfanumerico</param>
        /// <returns>La cadena convertida con ceros a la izquierda o espacios a la derecha</returns>
        public static string GetDato(string str, int tamano, TipoDato tipo)
        {
            var dato = new StringBuilder(tamano);
            if (tipo == TipoDato.Numerico)
            {
                dato.Append(str.PadLeft(tamano));
                dato.Replace(' ', '0');
            }
            else
            {
                dato.Append(str.PadRight(tamano));
            }
            return dato.ToString();
        }

        /// <summary>
        /// Convierte el monto en una cadena sin decimales Ej: Tamaño = 18 // 2000.45 = 000000000000002000 **** Tamaño = 18 // 2000.0 = 000000000000002000
        /// </summary>
        /// <param name="monto">Monto a convertir a string</param>
        /// <param name="tamano">Tmaño de la cadena a retornar</param>
        /// <returns>Cadena trasformada sin decimales y con ceros hacia la izquierda</returns>
        public static string GetMontoNoDecimales(double monto, int tamano)
        {
            monto = Math.Round(monto, 0);
            var dato = new StringBuilder(tamano);
            dato.Append(monto.ToString(CultureInfo.InvariantCulture).PadLeft(tamano));
            dato.Replace(' ', '0');
            return dato.ToString();
        }

        public static string GetMontoConDecimales(double monto, int tamano)
        {
            monto = Math.Round(monto, 2);
            string strmonto;
            var dato = new StringBuilder(tamano);
            string[] parts = monto.ToString(CultureInfo.InvariantCulture).Split('.');
            if (parts.Length > 1)
            {
                string part1 = parts[0];
                string part2 = parts[1];
                if(part2.Length < 2)
                {
                    part2 = string.Format("{0}{1}", part2, "0");
                }
                strmonto = string.Format("{0}{1}", part1, part2);
            }
            else
            {
                strmonto = string.Format("{0}{1}", monto.ToString(CultureInfo.InvariantCulture), "00");
            }
            dato.Append(strmonto.PadLeft(tamano));
            dato.Replace(' ', '0');
            return dato.ToString();
        }

        public static string GetPeriodo(DateTime dtPeriodo, int tamano)
        {
            var dato = new StringBuilder(tamano);
            string mes = dtPeriodo.Month.ToString(CultureInfo.InvariantCulture).PadLeft(2);
            mes = mes.Replace(' ', '0');
            string periodo = string.Format("{0}{1}", dtPeriodo.Year.ToString(CultureInfo.InvariantCulture), mes);
            dato.Append(periodo.PadLeft(tamano));
            dato.Replace(' ', '0');
            return dato.ToString();
        }

        public static string GetFecha(DateTime dateTime)
        {
            var dato = new StringBuilder(8);

            string dia = dateTime.Day.ToString(CultureInfo.InvariantCulture).PadLeft(2);
            dia = dia.Replace(' ', '0');

            string mes = dateTime.Month.ToString(CultureInfo.InvariantCulture).PadLeft(2);
            mes = mes.Replace(' ', '0');

            string fecha = string.Format("{0}{1}{2}", dateTime.Year.ToString(CultureInfo.InvariantCulture), mes, dia);
            dato.Append(fecha.PadLeft(8));
            dato.Replace(' ', '0');
            return dato.ToString();
        }

        public static string GetFechaHora(DateTime datetime)
        {
            var dato = new StringBuilder(14);

            string segundo = datetime.Second.ToString(CultureInfo.InvariantCulture).PadLeft(2);
            segundo = segundo.Replace(' ', '0');

            string minuto = datetime.Minute.ToString(CultureInfo.InvariantCulture).PadLeft(2);
            minuto = minuto.Replace(' ', '0');

            string hora = datetime.Hour.ToString(CultureInfo.InvariantCulture).PadLeft(2);
            hora = hora.Replace(' ', '0');

            string dia = datetime.Day.ToString(CultureInfo.InvariantCulture).PadLeft(2);
            dia = dia.Replace(' ', '0');

            string mes = datetime.Month.ToString(CultureInfo.InvariantCulture).PadLeft(2);
            mes = mes.Replace(' ', '0');


            string fecha = string.Format("{0}{1}{2}{3}{4}{5}", datetime.Year.ToString(CultureInfo.InvariantCulture), mes, dia, hora, minuto, segundo);
            dato.Append(fecha.PadLeft(14));
            dato.Replace(' ', '0');
            return dato.ToString();
        }
    }
    public enum TipoDato
    {
        Alfanumerico = 0,
        Numerico = 1
    }
}
