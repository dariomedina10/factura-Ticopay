using System;
using System.Globalization;
using System.Text;
//using Asadas.Core.BL;

namespace Ticopay.BNModel
{
    public class RespuestaAplicacionNotaCreditoDebito : RespuestaBNConectivdad
    {
        /// <summary>
        /// El codigo para este mensaje es 510
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
        /// 210000 
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// identificador único de transacción utilizado para validaciones. Debe ser el mismo que se envió en la transacción de aplicación.
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        /// Número que identifica al convenio y es asignado por el Banco Nacional.
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// Fecha correspondiente a las notas de crédito y débito con las que se realiza los movimientos.
        /// </summary>
        public DateTime FechaNota { get; set; }

        /// <summary>
        /// Total de pagos recaudados.
        /// </summary>
        public int TotalPagos { get; set; }

        /// <summary>
        /// Monto total recaudado. El punto decimal no debe ser especificado.
        /// </summary>
        public double TotalMontoPagos { get; set; }

        /// <summary>
        /// 00 : Aplicación recibida satisfactoriamente, 07 : El total de pagos no coincide, 08 : El total recaudado por los pagos no coincide, 09 : El monto de la comisión debitada no coincide, 10 : Datos inconsistentes en la transacción
        ///</summary>
        public int CodigoRespuesta { get; set; }


        public RespuestaAplicacionNotaCreditoDebito(AplicacionNotaCreditoDebito aplicacion)
        {
            TipoMensaje = 510;
            CodigoBanco = aplicacion.CodigoBanco;
            CodigoAgencia = aplicacion.CodigoAgencia;
            CodigoTransaccion = 210000;
            ConsecutivoTransaccion = aplicacion.ConsecutivoTransaccion;
            CodigoConvenio = aplicacion.CodigoConvenio;
            FechaNota = aplicacion.FechaNota;
            TotalPagos = aplicacion.TotalPagos;
            TotalMontoPagos = aplicacion.TotalMontoPagos;
            CodigoRespuesta = 0;
        }


        /// <summary>
        /// Convierte el objeto en una trama de respuesta
        /// </summary>
        /// <returns>Trama de respuesta</returns>
        public override string ToString()
        {
            var trama = new StringBuilder();
            trama.Append(GetDato(TipoMensaje.ToString(CultureInfo.InvariantCulture), 4, TipoDato.Numerico));
            trama.Append(GetDato(CodigoBanco.ToString(CultureInfo.InvariantCulture), 5, TipoDato.Numerico));
            trama.Append(GetDato(CodigoAgencia.ToString(CultureInfo.InvariantCulture), 6, TipoDato.Numerico));
            trama.Append(GetDato(CodigoTransaccion.ToString(CultureInfo.InvariantCulture), 6, TipoDato.Numerico));
            trama.Append(GetDato(ConsecutivoTransaccion.ToString(CultureInfo.InvariantCulture), 6, TipoDato.Numerico));
            trama.Append(GetDato(CodigoConvenio.ToString(CultureInfo.InvariantCulture), 3, TipoDato.Numerico));
            trama.Append(GetFecha(FechaNota));
            trama.Append(GetDato(TotalPagos.ToString(CultureInfo.InvariantCulture), 6, TipoDato.Numerico));
            trama.Append(GetMontoConDecimales(TotalMontoPagos, 20));
            trama.Append(GetDato(CodigoRespuesta.ToString(CultureInfo.InvariantCulture), 2, TipoDato.Numerico));
            return trama.ToString();
        }
    }
}
