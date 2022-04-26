using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
//using Asadas.Core.BL;
//using Asadas.Core.WCFModel;

namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama N°2:  Respuesta a consulta de recibos.
    /// </summary>
    public class RespuestaConsultaRecibo : RespuestaBNConectivdad
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
        ///800100. Tamaño = 6
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// Identificador único de transacción utilizado para validaciones. Debe ser el mismo que se envió en la consulta( Mensaje 200, código de transacción 310000). Tamaño = 6
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        /// Número que identifica al convenio y es asignado por el Banco Nacional. Tamaño = 3
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// Se define en conjunto con empresa emisora del convenio. 00  - Respuesta satisfactoria. Tamaño = 2
        /// </summary>
        public int CodigoRespuesta { get; set; }

        /// <summary>
        /// En caso de que se envíe la cédula o identificación, sino se debe enviar el tipo del  servicio. Tamaño = 3
        /// </summary>
        public string TipoIdentificacion { get; set; }

        /// <summary>
        /// En caso de que se envíe la cédula o identificación, sino se debe enviar el valor del  servicio. Tamaño = 30
        /// </summary>
        public string IdentificacionCliente { get; set; }

        /// <summary>
        /// Nombre del cliente en el siguiente orden : nombres. primer apellido(si aplica). segundo apellido(si aplica). Tamaño = 50
        /// </summary>
        public string NombreCliente { get; set; }

        /// <summary>
        /// Número de servicios enviados y que poseen recibos(Es utilizado en caso de que se consulte por cédula o identificación). Tamaño = 2
        /// </summary>
        public int CantidadServicios { get; set; }

        /// <summary>
        /// Numero de cuotas consultadas. En caso de una solicitud de cancelación, se enviará el total de pendientes. Se utiliza en convenios en los que no se maneja facturas si no cuotas. Tamaño = 1
        /// </summary>
        public int NumeroCuotas { get; set; }

        public List<ServicioPendiente> Servicios { get; set; }

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
            trama.Append(GetDato(CodigoRespuesta.ToString(CultureInfo.InvariantCulture), 2, TipoDato.Numerico));
            trama.Append(GetDato(TipoIdentificacion, 3, TipoDato.Alfanumerico));
            trama.Append(GetDato(IdentificacionCliente, 30, TipoDato.Alfanumerico));
            trama.Append(GetDato(NombreCliente, 50, TipoDato.Alfanumerico));
            trama.Append(GetDato(CantidadServicios.ToString(CultureInfo.InvariantCulture), 2, TipoDato.Numerico));

            foreach (var servicio in Servicios)
            {
                trama.Append(GetDato(servicio.LlaveAcceso, 30, TipoDato.Alfanumerico));
                trama.Append(GetDato(servicio.Recibos.Count().ToString(CultureInfo.InvariantCulture), 2, TipoDato.Numerico));
                foreach (var recibo in servicio.Recibos)
                {
                    trama.Append(GetPeriodo(recibo.Periodo, 8));
                    trama.Append(GetMontoConDecimales(recibo.Monto, 18));
                    trama.Append(GetFecha(recibo.Vencimiento));
                    trama.Append(GetDato(recibo.NumeroFactura.ToString(CultureInfo.InvariantCulture), 20, TipoDato.Numerico));
                    trama.Append(GetDato(recibo.Verificador.ToString(CultureInfo.InvariantCulture), 1, TipoDato.Numerico));
                }
            }
            trama.Append(GetDato(NumeroCuotas.ToString(CultureInfo.InvariantCulture), 1, TipoDato.Numerico));
            return trama.ToString();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RespuestaConsultaRecibo()
        {
            NombreCliente = string.Empty;
            Servicios = new List<ServicioPendiente>();
        }

        public RespuestaConsultaRecibo(ConsultaRecibo consultaRecibo)
        {
            NombreCliente = string.Empty;
            Servicios = new List<ServicioPendiente>();
            TipoMensaje = 210;
            CodigoBanco = 151;
            CodigoAgencia = consultaRecibo.CodigoAgencia;
            CodigoTransaccion = 800100;
            ConsecutivoTransaccion = consultaRecibo.ConsecutivoTransaccion;
            CodigoConvenio = consultaRecibo.CodigoConvenio;
            CodigoRespuesta = 0;
            IdentificacionCliente = consultaRecibo.LlaveAcceso;
            CantidadServicios = 0;
            TipoIdentificacion = consultaRecibo.TipoAcceso;
        }
    }
}
