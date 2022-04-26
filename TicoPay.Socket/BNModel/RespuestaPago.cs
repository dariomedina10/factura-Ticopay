using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
//using Asadas.Core.BL;

namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama N°4: Respuesta a aplicación del pago
    /// </summary>
    public class RespuestaPago : RespuestaBNConectivdad
    {
        /// <summary>
        /// El codigo para este mensaje es 210. Tamaño = 4
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
        /// 200100 Tamaño = 6
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// Identificador único de transacción utilizado para validaciones. Debe ser el mismo que se envió en la transacción de aplicación de pago. Tamaño = 6
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        ///Número que identifica al convenio y es asignado por el Banco Nacional. Tamaño = 3
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// En caso de que se envíe la cédula o identificación, sino se debe enviar el tipo del  servicio. Tamaño = 3
        /// </summary>
        public string TipoIdentificacion { get; set; }

        /// <summary>
        /// En caso de que se envíe la cédula o identificación, sino se debe enviar el valor del  servicio. Tamaño = 30
        /// </summary>
        public string IdentificacionCliente { get; set; }

        /// <summary>
        /// Nombre del cliente en el siguiente orden : Nombre(s). primer apellido(si aplica). segundo apellido(si aplica). Tamaño = 50
        /// </summary>
        public string NombreCliente { get; set; }

        /// <summary>
        /// Valor del servicio al que pertenece el recibo cancelado. Tamaño = 30
        /// </summary>
        public string ValorServicio { get; set; }

        /// <summary>
        /// Periodo del recibo cancelado. Debe enviarse un formato de periodo válido ya sea (AAAAMMDD), (AAAAMM), (AAAA). Tamaño = 8
        /// </summary>
        public string Periodo { get; set; }

        /// <summary>
        /// Monto total pagado para el recibo. El punto decimal no debe ser especificado. Tamaño = 18(2)
        /// </summary>
        public double Monto { get; set; }

        /// <summary>
        /// Monto de comisión por recaudación. 15(2)
        /// </summary>
        public int Comision { get; set; }

        /// <summary>
        /// Fecha de vencimiento del recibo cancelado. Tamaño = 8
        /// </summary>
        public DateTime FechaVencimiento { get; set; }

        /// <summary>
        /// Número de factura del recibo del  cancelado. Campo requerido. Tamaño = 20
        /// </summary>
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Dígito verificador del  recibo cancelado. Tamaño = 1
        /// </summary>
        public int Verificador { get; set; }

        /// <summary>
        /// Fecha y hora de la transacción, utilizando el formato AAAAMMDDHH24MISS. Tamaño = 14
        /// </summary>
        public DateTime FechaTransaccion { get; set; }

        /// <summary>
        /// 00 : Pago recibido satisfactoriamente, 01 : El recibo ya está pagado, 02 : El recibo no existe, 03 : El recibo está vencido. (Cuando no se permite pagos extemporáneos), 04 : Datos inconsistentes en la transacción. Tamaño = 2
        /// </summary>
        public int CodigoRespuesta { get; set; }

        public List<Rubro> Rubros { get; set; }

        public RespuestaPago()
        {
            NombreCliente = string.Empty;
            Rubros = new List<Rubro>();
        }

        public RespuestaPago(AplicacionPago aplicacionPago)
        {
            NombreCliente = string.Empty;
            Rubros = new List<Rubro>();
            TipoMensaje = 210;
            CodigoBanco = 151;
            CodigoAgencia = aplicacionPago.CodigoAgencia;
            CodigoTransaccion = 200100;
            ConsecutivoTransaccion = aplicacionPago.ConsecutivoTransaccion;
            CodigoConvenio = aplicacionPago.CodigoConvenio;
            TipoIdentificacion = "126";
            IdentificacionCliente = aplicacionPago.ValorServicio;
            ValorServicio = aplicacionPago.ValorServicio;
            Periodo = aplicacionPago.Periodo;
            Monto = aplicacionPago.Monto;
            Comision = 0;
            FechaVencimiento = aplicacionPago.FechaVencimiento;
            NumeroFactura = aplicacionPago.NumeroFactura;
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
            trama.Append(GetDato(TipoIdentificacion, 3, TipoDato.Alfanumerico));
            trama.Append(GetDato(IdentificacionCliente, 30, TipoDato.Alfanumerico));
            trama.Append(GetDato(NombreCliente, 50, TipoDato.Alfanumerico));
            trama.Append(GetDato(ValorServicio, 30, TipoDato.Alfanumerico));
            trama.Append(GetDato(Periodo, 8, TipoDato.Numerico));
            trama.Append(GetMontoConDecimales(Monto, 18));
            trama.Append(GetMontoConDecimales(Comision, 15));
            trama.Append(GetFecha(FechaVencimiento));
            trama.Append(GetDato(NumeroFactura.ToString(CultureInfo.InvariantCulture), 20, TipoDato.Numerico));
            trama.Append(GetDato(Verificador.ToString(CultureInfo.InvariantCulture), 1, TipoDato.Numerico));
            trama.Append(GetFechaHora(FechaTransaccion));
            trama.Append(GetDato(CodigoRespuesta.ToString(CultureInfo.InvariantCulture), 2, TipoDato.Numerico));
            trama.Append(GetDato(Rubros.Count.ToString(CultureInfo.InvariantCulture), 2, TipoDato.Numerico));

            
            foreach (var rubro in Rubros)
            {
                trama.Append(GetDato(rubro.Codigo.ToString(CultureInfo.InvariantCulture), 4, TipoDato.Numerico));
                trama.Append(GetDato(rubro.Monto, 18, TipoDato.Alfanumerico));
            }
            return trama.ToString();
        }
    }
}
