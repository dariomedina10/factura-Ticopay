using System;
using System.Text;
using Ticopay.BNModel;

namespace TicoPay.Socket
{
    public static class Parser
    {
        public static int GetMensaje(string trama) //Obtiene el tipo de Mensaje
        {
            int result = -1;
            Int32.TryParse(trama.Substring(0, 4), out result);
            return result;
        }

        public static int GetCodigoTransaccion(string trama) // Obtiene el codigo de transaccion
        {
            int result = -1;
            Int32.TryParse(trama.Substring(15, 6), out result);
            return result;
        }

        public static int GetCodigoBanco(string trama) // Obtiene el codigo del banco
        {
            int codigoBanco = 0;
            Int32.TryParse(trama.Substring(4, 5), out codigoBanco); //Obtiene el codigo de banco
            return codigoBanco;
        }

        public static int GetCodigoAgencia(string trama) //Obtiene el codigo de agencia
        {
            int codigoAgencia = 0;
            Int32.TryParse(trama.Substring(9, 6), out codigoAgencia); 
            return codigoAgencia;
        }

        public static int GetConsecutivoTransaccion(string trama) //Obtiene el consecutivo de transaccion
        {
            int consecutivoTransaccion = 0;
            Int32.TryParse(trama.Substring(21, 6), out consecutivoTransaccion); 
            return consecutivoTransaccion;
        }

        public static int GetCodigoConvenio(string trama) //Obtiene el codigo de convenio
        {
            int codigoConvenio = 0;
            Int32.TryParse(trama.Substring(27, 3), out codigoConvenio);
            return codigoConvenio;
        }

        public static int GetNumCoutas(string trama) //Obtiene el numero Coutas
        {
            int numCouta = 0;
            Int32.TryParse(trama.Substring(78, 4), out numCouta);
            return numCouta;
        }

        //public static TipoAcceso GetTipoAcceso(string trama) //Obtiene el tipo de acceso
        //{
        //    int tipoAcceso = 0;
        //    Int32.TryParse(trama.Substring(31, 3), out tipoAcceso);

        //    switch (tipoAcceso)
        //    {
        //        case 126:
        //            return TipoAcceso.Cuenta;
        //    }

        //    return TipoAcceso.Cuenta;
        //}


        public static TipoBusqueda GetTipoBusqueda(string tipobusquedastr) //Obtiene el tipo de busqueda
        {
            int tipoBusqueda = 0;
            Int32.TryParse(tipobusquedastr, out tipoBusqueda);

            switch (tipoBusqueda)
            {
                case 0:
                    return TipoBusqueda.Identificacion;
                case 1:
                    return TipoBusqueda.Servicio;
            }

            return TipoBusqueda.Identificacion;
        }

        public static DateTime GetDateTime(string formatFecha) //Obtiene la fecha
        {
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

        public static string LlaveAcceso(string llave) //Obtiene la llave de acceso
        {
            if (llave.IndexOf(' ') != -1)
            return llave.Substring(0, llave.IndexOf(' '));
            return llave.Substring(0, 30);
        }

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


        //public static ConsultaRecibo ParserConsultaRecibo(string trama)
        //{

        //    var consultaRecibo = new ConsultaRecibo
        //    {
        //        TipoMensaje = 200,
        //        CodigoBanco = GetCodigoBanco(trama),
        //        CodigoAgencia = GetCodigoAgencia(trama),
        //        CodigoTransaccion = 310000,
        //        ConsecutivoTransaccion = GetConsecutivoTransaccion(trama),
        //        CodigoConvenio = GetCodigoConvenio(trama),
        //        TipoBusqueda = GetTipoBusqueda(trama.Substring(30, 1)),
        //        TipoAcceso = GetTipoAcceso(trama),
        //        LlaveAcceso = LlaveAcceso(trama.Substring(34, 30)),
        //        FechaTransaccion = GetDateTime(trama.Substring(64, 14)),
        //        NumeroCouta = GetNumCoutas(trama)
        //    };
        //    return consultaRecibo;
        //}
    }

    public enum TipoConsulta
    {
        ConsultaRecibo,
        RespuestaConsultaRecibo,
        AplicacionPago,
        RespuestaAplicacionPago,
        ReversionPago,
        RespuestaReversionPago,
        AplicacionNota,
        RespuestaAplicacionNota,
        ConsultaServicioIdentificacion,
        RespuestaConsultaServicioIdentificacion,
        ConsultaDisponibilidadServicio,
        RespuestaConsultaDisponibilidadServicio,
        AfiliacionDesafiliacionPagoAutomatico,
        RespuestaAfilicacionDesafiliacion,
        SolicitaCambioLocalizacion,
        RespuestaSolicitudCambioLocalizacion,
        CalendarizacionVencimientoRecibo,
        RespuestaFechaVencimientoRecibo,
        SolicitudVencimientoRecibo,
        RespuestaSolicitudVencimientoCiclos,
        SolicitudReasignacionCiclos,
        RespuestaReasignacionCiclos
    }
}
