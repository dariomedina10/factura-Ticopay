using System;

namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama #5 Calendarizacion por vencimiento de recibos
    /// </summary>
    public class CalendarizacionVencimientoRecibo : BNConectividad
    {
       /// <summary>
       /// Tipo de Mensaje = 140. Tamaño = 4
       /// </summary>
        public int TipoMensaje { get; set; }

       /// <summary>
        /// Código de la SUGEF asignado al Banco Nacional. Actualmente es 151. Tamaño = 5
       /// </summary>
        public int CodigoBanco { get; set; }

       /// <summary>
        /// Código de la agencia del Banco Nacional  desde la cual se envía la consulta. Tamaño = 6
       /// </summary>
        public int CodigoAgencia { get; set; }

       /// <summary>
        /// 100000 : Calendarización por  vencimiento. Tamaño = 6
       /// </summary>
        public int CodigoTransaccion { get; set; }

       /// <summary>
        /// Identificador único de transacción utilizado para validaciones. Debe ser el que se envíe en la respuesta. Tamaño = 6
       /// </summary>
        public int ConsecutivoTransaccion { get; set; }

       /// <summary>
        /// Código de convenio acordado por la empresa y el  Banco Nacional de Costa Rica para ofrecer el servicio PAR. Tamaño = 3
       /// </summary>
        public int CodigoConvenio { get; set; }

       /// <summary>
        /// Fecha y hora de la transacción. Formato AAAAMMDDHH24MISS. Tamaño = 14
       /// </summary>
        public DateTime FechaTransaccion { get; set; }

       /// <summary>
        /// Ultima localización recibida. Para la primera corrida  se envía en espacios en blanco. Tamaño = 30
       /// </summary>
        public string UltimaLlaveServicio { get; set; }

       /// <summary>
        ///Periodo del último recibo recibido.  Para la primera ejecución se debe enviar en ceros. Tamaño = 8
       /// </summary>
        public string UltimoPeriodo { get; set; }

       public CalendarizacionVencimientoRecibo()
       {

       }

       /// <summary>
       /// Serializa la informacion de la trama calendarizacion de vencimientos en un objeto
       /// </summary>
       /// <param name="trama">Trama enviada por el banco nacional</param>
       /// <returns>Datos de la peticion calendarizacion serializada en un objeto</returns>
       public static CalendarizacionVencimientoRecibo ParserCalendarizacionVencimientos(string trama)
       {
           var calendarizacion = new CalendarizacionVencimientoRecibo
           {
               TipoMensaje = 140,
               CodigoBanco = 151,
               CodigoAgencia = GetCodigoAgencia(trama),
               CodigoTransaccion = 100000,
               ConsecutivoTransaccion = GetConsecutivoTransaccion(trama),
               CodigoConvenio = GetCodigoConvenio(trama),
               FechaTransaccion = GetFechaHoraFromTrama(trama, 30),
               UltimaLlaveServicio = GetLlaveAcceso(trama, 44),
               UltimoPeriodo = GetCadena(trama, 74, 8)
           };
           return calendarizacion;
       }
    }
}
