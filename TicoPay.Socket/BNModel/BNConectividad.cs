using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ticopay.BNModel
{
   public class BNConectividad
    {
        //TODO: Se debe tener cuidado si a futuro las tramas cambian los primeros datos estandar (Tamaño o orden en la cadena), Tipo Mensaje, Codigo Banco, Codigo Agencia, Codigo Transaccion, Consecutivo Transaccion, Codigo Convenio
        /// <summary>
        /// Obtiene el Tipo de Mensaje
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>tipo de mensaje segun la peticion del Banco Nacional</returns>
        public static int GetMensaje(string trama) //Obtiene el tipo de Mensaje
        {
            int result = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 4) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 4
                Int32.TryParse(trama.Substring(0, 4), out result); // Tipo Mensaje --> Numerico, Tamaño 4
            return result;
        }

        /// <summary>
        /// Obtiene Código de la SUGEF asignado al Banco Nacional. Actualmente 151.
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Codigo de Banco</returns>
        public static int GetCodigoBanco(string trama)
        {
            int codigoBanco = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 9) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 9
                Int32.TryParse(trama.Substring(4, 5), out codigoBanco); //Avanza 4 campos en la cadena y a partir de alli extrae 5 campos la subcadena de codigo de banco 
            return codigoBanco;
        }

        /// <summary>
        /// Obtiene Código de la agencia de donde se envió la consulta.
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Código de la agencia de donde se envió la consulta.</returns>
        public static int GetCodigoAgencia(string trama)
        {
            int codigoAgencia = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 15) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 15
                Int32.TryParse(trama.Substring(9, 6), out codigoAgencia);
            return codigoAgencia;
        }

        /// <summary>
        /// Obtiene el codigo de transaccion
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>codigo de transaccion</returns>
        public static int GetCodigoTransaccion(string trama) // 
        {
            int result = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 21) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 21
                Int32.TryParse(trama.Substring(15, 6), out result); // Código de transacción --> Numerico, Tamaño 6
            return result;
        }

        /// <summary>
        /// Obtiene el Identificador único de transacción utilizado para validaciones. Debe ser el mismo que se envíe en la respuesta.
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Retorna Identificador único de transacción utilizado para validaciones. Debe ser el mismo que se envíe en la respuesta.</returns>
        public static int GetConsecutivoTransaccion(string trama) //Obtiene el consecutivo de transaccion
        {
            int consecutivoTransaccion = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 27) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 27
                Int32.TryParse(trama.Substring(21, 6), out consecutivoTransaccion);
            return consecutivoTransaccion;
        }

        /// <summary>
        /// Obtiene el Número que identifica al convenio y es asignado por el Banco Nacional.
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Número que identifica al convenio y es asignado por el Banco Nacional.</returns>
        public static int GetCodigoConvenio(string trama) //Obtiene el codigo de convenio
        {
            int codigoConvenio = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 30) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 30
                Int32.TryParse(trama.Substring(27, 3), out codigoConvenio);
            return codigoConvenio;
        }

      /// <summary>
        /// Obtiene un numero a partir de la cadena enviada como parametro
      /// </summary>
      /// <param name="trama">Trama enviada por el banco</param>
      /// <param name="index">Indice donde arranca el campo a extraer</param>
      /// <param name="length">Largo del campo a extraer</param>
      /// <returns>Numero</returns>
      public static int GetNumero(string trama, int index, int length)
      {
          string strNumero = string.Empty;
          if (!String.IsNullOrEmpty(trama) && trama.Length >= index + length) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a index + length
              strNumero = trama.Substring(index, length);
          int numero = 0;
          Int32.TryParse(strNumero, out numero);
          return numero;
      }

      /// <summary>
      /// Obtiene la feha y hora a partir de la trama enviada
      /// </summary>
      /// <param name="trama">Trama enviada por el BN</param>
      /// <param name="index">Indice donde empieza el campo que contiene la fecha y hora</param>
      /// <returns>Fecha y Hora</returns>
      public static DateTime GetFechaHoraFromTrama(string trama, int index)
      {
          string formatFecha = string.Empty;

          if (!String.IsNullOrEmpty(trama) && trama.Length >= index + 14) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a index + length
              formatFecha = trama.Substring(index, 14);

          int ano = 0;
          Int32.TryParse(formatFecha.Substring(0, 4), out ano);

          int mes = 0;
          Int32.TryParse(formatFecha.Substring(4, 2), out mes);

          int dia = 0;
          Int32.TryParse(formatFecha.Substring(6, 2), out dia);

          int hora = 0;
          Int32.TryParse(formatFecha.Substring(8, 2), out hora);
          hora = 20;

          int minuto = 0;
          Int32.TryParse(formatFecha.Substring(10, 2), out minuto);

          int segundo = 0;
          Int32.TryParse(formatFecha.Substring(12, 2), out segundo);

          return new DateTime(ano, mes, dia, hora, minuto, segundo);
      }
        /// <summary>
        /// Obtiene la feha a partir de la trama enviada
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <param name="index">Indice donde empieza la fecha a extraer</param>
        /// <returns></returns>
      public static DateTime GetFechaFromTrama(string trama, int index)
      {
          string formatFecha = string.Empty;

          if (!String.IsNullOrEmpty(trama) && trama.Length >= index + 8) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a index + 8
              formatFecha = trama.Substring(index, 8);

          int ano = 0;
          Int32.TryParse(formatFecha.Substring(0, 4), out ano);

          int mes = 0;
          Int32.TryParse(formatFecha.Substring(4, 2), out mes);

          int dia = 0;
          Int32.TryParse(formatFecha.Substring(6, 2), out dia);

          return new DateTime(ano, mes, dia);
      }

       /// <summary>
       /// Obtiene el monto desde la trama 
       /// </summary>
       /// <param name="trama">Trama enviada por el banco</param>
       /// <param name="index">indice donde empieza el campo del monto</param>
       /// <param name="length">Largo del campo monto</param>
       /// <returns>Monto</returns>
      public static double GetMontoFromTrama(string trama, int index, int length)
      {
          string strmonto = string.Empty;

          if (!String.IsNullOrEmpty(trama) && trama.Length >= index + length) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a index + length
              strmonto = trama.Substring(index, length);

          string parteEntera = strmonto.Substring(0, length - 2);
          string parteFraccionaria = strmonto.Substring(length - 2, 2);

          string monto = string.Format("{0}.{1}", parteEntera, parteFraccionaria);
          double montodbl = 0;
          Double.TryParse(monto, out montodbl);
          return montodbl;
      }

     /// <summary>
     /// Obtiene la llave que identifica el servicio
     /// </summary>
     /// <param name="trama">Trama enviada por el banco</param>
     /// <param name="index">Indice donde empieza el campo</param>
     /// <returns>Llave de acceso</returns>
      public static string GetLlaveAcceso(string trama, int index)
      {
          string llave = string.Empty;

          if (!String.IsNullOrEmpty(trama) && trama.Length >= index + 30) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a index + 30
              llave = trama.Substring(index, 30);

          if (llave.IndexOf(' ') != -1)
              return llave.Substring(0, llave.IndexOf(' '));
          return llave.Substring(0, 30);
      }

       /// <summary>
       /// Extrae una subcadena segun el index y length
       /// </summary>
      /// <param name="trama">Trama enviada por el banco</param>
      /// <param name="index">Indice donde empieza el campo</param>
       /// <param name="length">Tamaño de la cadena a extraer</param>
       /// <returns>Subcadena</returns>
      public static string GetCadena(string trama, int index, int length)
      {
          if (!String.IsNullOrEmpty(trama) && trama.Length >= index + length) // La trama debe ser diferente de null y empty
            return trama.Substring(index, length);
          return string.Empty;
      }
    }
}
