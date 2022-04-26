using System;
using System.Globalization;
using System.Text;
//using Asadas.Core.BL;

namespace Ticopay.BNModel
{
    public class RespuestaReversionPago : RespuestaBNConectivdad
    {
        /// <summary>
        /// El codigo para este mensaje es 430. Tamaño = 4
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
        /// Codigo Transaccion = 800000. Tamaño = 6
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// identificador único de transacción utilizado para validaciones. Debe ser el mismo que se envió en la transacción de reversión de pago. Tamaño = 6
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        /// Número que identifica al convenio y es asignado por el Banco Nacional. Tamaño = 3
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// Valor del servicio cancelado. Tamaño = 30
        /// </summary>
        public string ValorServicio { get; set; }

        /// <summary>
        /// Periodo del recibo del servicio cancelado. Debe enviarse un formato de periodo válido ya sea (AAAAMMDD), (AAAAMM), (AAAA). Tamaño = 8
        /// </summary>
        public string Periodo { get; set; }

        /// <summary>
        /// Monto total pagado para el recibo del servicio cancelado. El punto decimal no debe ser especificado. Tamaño = 18(2)
        /// </summary>
        public double Monto { get; set; }

        /// <summary>
        /// Número de factura del recibo del servicio cancelado. Campo requerido. Tamaño = 20
        /// </summary>
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Dígito verificador del  recibo del servicio cancelado. Tamaño = 1
        /// </summary>
        public int Verificador { get; set; }

        /// <summary>
        /// Fecha y hora de la transacción, utilizando el formato AAAAMMDDHH24MISS. Tamaño = 14
        /// </summary>
        public DateTime FechaTransaccion { get; set; }

        /// <summary>
        /// 00 : Reversión realizada satisfactoriamente. 05 : El recibo no está pagado. 06 : El recibo no fue cancelado en la misma fecha de la reversión. Tamaño = 2
        /// </summary>
        public int CodigoRespuesta { get; set; }

        public RespuestaReversionPago(ReversionPago reversionPago)
        {
            TipoMensaje = 430;
            CodigoBanco = 151;
            CodigoAgencia = reversionPago.CodigoAgencia;
            CodigoTransaccion = 800000;
            ConsecutivoTransaccion = reversionPago.ConsecutivoTransaccion;
            CodigoConvenio = reversionPago.CodigoConvenio;
            ValorServicio = reversionPago.ValorServicio;
            Periodo = reversionPago.Periodo;
            Monto = reversionPago.Monto;
            NumeroFactura = reversionPago.NumeroFactura;
            Verificador = 0;
            FechaTransaccion = DateTime.Now;
            CodigoRespuesta = 0;
        }

        /// <summary>
        /// Devuelve la respuesta en una trama segun el formato sugerido por el banco
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var trama = new StringBuilder();
            trama.Append(GetDato(TipoMensaje.ToString(CultureInfo.InvariantCulture), 4, TipoDato.Numerico));
            trama.Append(GetDato(CodigoBanco.ToString(CultureInfo.InvariantCulture), 5, TipoDato.Numerico));
            trama.Append(GetDato(CodigoAgencia.ToString(CultureInfo.InvariantCulture), 6, TipoDato.Numerico));
            trama.Append(GetDato(CodigoTransaccion.ToString(CultureInfo.InvariantCulture), 6, TipoDato.Numerico));
            trama.Append(GetDato(ConsecutivoTransaccion.ToString(CultureInfo.InvariantCulture), 6, TipoDato.Numerico));
            trama.Append(GetDato(CodigoConvenio.ToString(CultureInfo.InvariantCulture), 3, TipoDato.Numerico));
            trama.Append(GetDato(ValorServicio, 30, TipoDato.Alfanumerico));
            trama.Append(GetDato(Periodo, 8, TipoDato.Numerico));
            trama.Append(GetMontoConDecimales(Monto, 18));
            trama.Append(GetDato(NumeroFactura.ToString(CultureInfo.InvariantCulture), 20, TipoDato.Numerico));
            trama.Append(GetDato(Verificador.ToString(CultureInfo.InvariantCulture), 1, TipoDato.Numerico));
            trama.Append(GetFechaHora(FechaTransaccion));
            trama.Append(GetDato(CodigoRespuesta.ToString(CultureInfo.InvariantCulture), 2, TipoDato.Numerico));
            return trama.ToString();
        }
    }
}
