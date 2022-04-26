using System.Collections.Generic;
using System.Globalization;
using System.Text;
//using Asadas.Core.BL;

namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama # 6 : Respuesta Fechas Vencimiento Recibos
    /// </summary>
    public class RespuestaFechaVencimientoRecibos : RespuestaBNConectivdad
    {
        /// <summary>
        /// El codigo para este mensaje es 150
        /// </summary>
        public int TipoMensaje { get; set; }

        /// <summary>
        /// Código de la SUGEF asignado al Banco Nacional. Actualmente es 151.
        /// </summary>
        public int CodigoBanco { get; set; }

        /// <summary>
        /// Código de la agencia del Banco Nacional  desde la cual se envía la consulta.
        /// </summary>
        public int CodigoAgencia { get; set; }

        /// <summary>
        /// 100100 : calendarización por vencimiento de recibos.
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// Identificador único de transacción utilizado para validaciones. Debe ser el que se envió en la consulta.
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        /// Código de convenio acordado por la empresa y el  Banco Nacional de Costa Rica para ofrecer el servicio PAR.
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// 0 : Solicitud procesada satisfactoriamente, Caso contrario, se envía el código correspondiente al error sucitado.
        /// </summary>
        public int CodigoRespuesta { get; set; }

        /// <summary>
        /// Indicador si existen más recibos a procesar.
        /// </summary>
        public int Indicador { get; set; }

        /// <summary>
        /// Lista de registros (facturas de previstas con Pago automatico) en la trama . El máximo número de registros por trama que se puede enviar es de 10.
        /// </summary>
        public List<Registro> Registros { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RespuestaFechaVencimientoRecibos()
        {
            Registros = new List<Registro>();
        }

        public RespuestaFechaVencimientoRecibos(CalendarizacionVencimientoRecibo calendarizacion)
        {
            TipoMensaje = 150;
            CodigoBanco = 151;
            CodigoAgencia = calendarizacion.CodigoAgencia;
            CodigoTransaccion = 100100;
            ConsecutivoTransaccion = calendarizacion.ConsecutivoTransaccion;
            CodigoConvenio = calendarizacion.CodigoConvenio;
            CodigoRespuesta = 0;
            Registros = new List<Registro>();
        }

        /// <summary>
        /// Devuelve la respuesta en una trama segun el formato sugerido por el banco
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var trama = new StringBuilder();
            trama.Append(GetDato(TipoMensaje.ToString(CultureInfo.InvariantCulture), 4, TipoDato.Numerico)); // TipoMensaje --> Campo Numerico, Tamaño = 4
            trama.Append(GetDato(CodigoBanco.ToString(CultureInfo.InvariantCulture), 5, TipoDato.Numerico)); // CodigoBanco --> Campo Numerico, Tamaño = 5
            trama.Append(GetDato(CodigoAgencia.ToString(CultureInfo.InvariantCulture), 6, TipoDato.Numerico)); // CodigoAgencia --> Campo Numerico, Tamaño = 6
            trama.Append(GetDato(CodigoTransaccion.ToString(CultureInfo.InvariantCulture), 6, TipoDato.Numerico)); //CodigoTransaccion --> Campo Numerico, Tamaño = 6
            trama.Append(GetDato(ConsecutivoTransaccion.ToString(CultureInfo.InvariantCulture), 6, TipoDato.Numerico)); //ConsecutivoTransaccion --> Campo Numerico, Tamaño = 6
            trama.Append(GetDato(CodigoConvenio.ToString(CultureInfo.InvariantCulture), 3, TipoDato.Numerico)); // CodigoConvenio --> Campo Numerico, Tamaño = 3
            trama.Append(GetDato(CodigoRespuesta.ToString(CultureInfo.InvariantCulture), 2, TipoDato.Numerico)); // CodigoRespuesta --> Campo Numerico, Tamaño = 2
            trama.Append(GetDato(Indicador.ToString(CultureInfo.InvariantCulture), 1, TipoDato.Numerico)); // Indicador --> Campo Numerico, Tamaño = 1
            trama.Append(GetDato(Registros.Count.ToString(CultureInfo.InvariantCulture), 4, TipoDato.Numerico)); // Cantidad  de registros --> Campo Numerico, Tamaño = 4

            foreach (var registro in Registros)
            {
                trama.Append(GetDato(registro.LlaveServicio, 30, TipoDato.Alfanumerico)); // LlaveServicio (Contrato) --> Campo AlfaNumerico, Tamaño = 30 
                trama.Append(GetPeriodo(registro.Periodo, 8));// Periodo --> Campo Numerico, Tamaño = 8
                trama.Append(GetMontoConDecimales(registro.Monto, 18)); // Monto --> Campo Numerico, Tamaño = 18, con 2 decimales
                trama.Append(GetFecha(registro.FechaVencimiento));// FechaVencimiento --> Campo Numerico, Tamaño = 8
                trama.Append(GetDato(registro.NumeroFactura.ToString(CultureInfo.InvariantCulture), 20, TipoDato.Numerico)); //Numero de factura --> Campo Numerico, Tamaño = 20
                trama.Append(GetDato(registro.Verificador.ToString(CultureInfo.InvariantCulture), 1, TipoDato.Numerico)); //Verificador --> Campo Numerico, Tamaño = 1
            }
            return trama.ToString();
        }
    }
}
