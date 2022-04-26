using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ticopay.BNModel.BNArchivoConciliacion
{
    public class BnConectividadConciliacion
    {
        /// <summary>
        /// Obtiene el Tipo de Mensaje
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>tipo de mensaje segun la peticion del Banco Nacional</returns>
        public static int GetTipoRegistro(string trama) //Obtiene el tipo de Mensaje
        {
            int result = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 2) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 2
                Int32.TryParse(trama.Substring(0, 2), out result); // Tipo Mensaje --> Numerico, Tamaño 4
            return result;
        }


        /// <summary>
        /// Obtiene el Código de la Asada (Empresa) a la que pertenece el convenio
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Codigo de la Asada</returns>
        public static int GetCodigoEmpresa(string trama)
        {
            int codigoEmpresa = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 5) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 5
                Int32.TryParse(trama.Substring(2, 3), out codigoEmpresa); //Avanza 3 campos en la cadena y a partir de alli extrae 3 campos la subcadena de codigo de banco 
            return codigoEmpresa;
        }

        /// <summary>
        /// Código de convenio del archivo batch
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Codigo del Convenio</returns>
        public static int GetCodigoConvenio(string trama)
        {
            int codigoConvenio = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 8) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 8
                Int32.TryParse(trama.Substring(5, 3), out codigoConvenio); //Avanza 3 campos en la cadena y a partir de alli extrae 3 campos la subcadena de codigo de banco 
            return codigoConvenio;
        }


        /// <summary>
        /// Código del ente recaudador hacia el cual es dirigido el archivo.
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// /// <param name="index">Indice donde empieza el campo</param>
        /// <returns>Codigo de Banco</returns>
        public static int GetCodigoBanco(string trama, int index)
        {
            int codigoBanco = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 12) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a12
                Int32.TryParse(trama.Substring(index, 4), out codigoBanco); //Avanza 4 campos en la cadena y a partir de alli extrae 5 campos la subcadena de codigo de banco 
            return codigoBanco;
        }

        /// <summary>
        /// Cantidad de pagos que se están acreditando y cuyo desglose se muestra en el archivo de pagos.
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Cantidad de Pagos</returns>
        public static int GetCantidadPagos(string trama)
        {
            int cantidadPagos = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 78) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 72
                Int32.TryParse(trama.Substring(72, 6), out cantidadPagos); //Avanza 6 campos en la cadena y a partir de alli extrae 6 campos la subcadena de codigo de banco 
            return cantidadPagos;
        }

        /// <summary>
        /// Cuenta que emitió el cheque
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Numero Cuenta</returns>
        public static int GetNumeroCuenta(string trama)
        {
            int numeroCuenta = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 147) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 147
                Int32.TryParse(trama.Substring(133, 19), out numeroCuenta); //Avanza 19 campos en la cadena y a partir de alli extrae 19 campos la subcadena de codigo de banco 
            return numeroCuenta;
        }

        /// <summary>
        /// Número del cheque con que se realiza el pago
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Codigo de Banco</returns>
        public static int GetNumeroCheque(string trama)
        {
            int numeroCheque = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 161) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 161
                Int32.TryParse(trama.Substring(152, 14), out numeroCheque); //Avanza 14 campos en la cadena y a partir de alli extrae 14 campos la subcadena de codigo de banco 
            return numeroCheque;
        }

        /// <summary>
        /// Agencia o canal de recaudación desde el cual se realizó el pago
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Codigo de Agencia</returns>
        public static int GetCodigoAgencia(string trama)
        {
            int codigoAgencia = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 16) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 16
                Int32.TryParse(trama.Substring(12, 4), out codigoAgencia); //Avanza 4 campos en la cadena y a partir de alli extrae 5 campos la subcadena de codigo de banco 
            return codigoAgencia;
        }

        /// <summary>
        /// Número de factura asignado al pago por la empresa externa
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Codigo de Banco</returns>
        public static int GetNumeroFactura(string trama)
        {
            int numeroFactura = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 36) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 36
                Int32.TryParse(trama.Substring(16, 20), out numeroFactura); //Avanza 20 campos en la cadena y a partir de alli extrae 20 campos la subcadena de codigo de banco 
            return numeroFactura;
        }

        /// <summary>
        /// Self del número de factura
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Self Verificacion</returns>
        public static int GetSelfVerificacion(string trama)
        {
            int selfVerificacion = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 37) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 37
                Int32.TryParse(trama.Substring(36, 1), out selfVerificacion); //Avanza 1 campos en la cadena y a partir de alli extrae 1 campos la subcadena de codigo de banco 
            return selfVerificacion;
        }

        /// <summary>
        /// Obtiene el tipo de Moneda 01: Colones 02: Dólares 03:Euros
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <returns>Codigo Moneda</returns>
        public static int GetCodigoMoneda(string trama)
        {
            int codigoMoneda = -1;
            if (!String.IsNullOrEmpty(trama) && trama.Length >= 40) // La trama debe ser diferente de null y empty, ademas el tamano debe ser mayor o igual a 40
                Int32.TryParse(trama.Substring(37, 3), out codigoMoneda); //Avanza 20 campos en la cadena y a partir de alli extrae 20 campos la subcadena de codigo de banco 
            return codigoMoneda;
        }

        /// <summary>
        /// Obtiene la llave que identifica el Acceso
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
        /// Obtiene la llave que identifica el Acceso
        /// </summary>
        /// <param name="trama">Trama enviada por el banco</param>
        /// <param name="index">Indice donde empieza el campo</param>
        /// <returns>Llave de acceso</returns>
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

        public enum TipoDato
        {
            Alfanumerico = 0,
            Numerico = 1
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
