using System.Globalization;
using System.Text;
//using Asadas.Core.BL;

namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama # 2 : Respuesta: Afiliación - Desafiliación
    /// </summary>
    public class RespuestaAfiliacionDesafiliacion : RespuestaBNConectivdad
    {
        /// <summary>
        /// El codigo para este mensaje es 110. Tamaño 4
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
        /// 100100 : Afiliación. 100101 : Desafiliación. 100102 : Consulta.  Tamaño = 6
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// Identificador único de transacción utilizado para validaciones. Debe ser el mismo que se envió en el mensaje 100. Tamaño = 6
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        /// Código de convenio acordado por la empresa y el  Banco Nacional de Costa Rica para ofrecer el servicio PAR. Tamaño = 3
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// 0 : Solicitud procesada satisfactoriamente Caso contrario, se envía el código correspondiente al error sucitado. Tamaño = 2
        /// </summary>
        public int CodigoRespuesta { get; set; }

        /// <summary>
        /// En caso de que se envié la cédula o identificación, sino se debe enviar el tipo del servicio. 1 : cédula física 2 : cédula jurídica 3 : cédula de residencia. Tamaño = 3
        /// </summary>
        public string TipoIdentificacion { get; set; }

        /// <summary>
        /// Identificación del cliente. Tamaño = 30
        /// </summary>
        public string IdentificacionCliente { get; set; }

        //TODO:ngonzalez revisar que el nombre este en orden
        /// <summary>
        /// Nombre del cliente en el siguiente orden: 1. Nombres 2. Primer apellido(si aplica) 3. Segundo apellido (si aplica). Tamaño = 50
        /// </summary>
        public string NombreCliente { get; set; }

        /// <summary>
        /// Día en que se debe realizar el pago del servicio ó  ciclo de facturación. Tamaño = 3
        /// </summary>
        public int DiaPagoCiclo { get; set; }

        /// <summary>
        /// Monto promedio del consumo del servicio por parte  del cliente (En caso de desafiliación no se debe enviar ésta información). Tamaño = 18 
        /// </summary>
        public double MontoPromedio { get; set; }


        public RespuestaAfiliacionDesafiliacion(AfiliacionDesafiliacion afiliacionDesafiliacion)
        {
            TipoMensaje = 110;
            CodigoBanco = 151;
            CodigoAgencia = afiliacionDesafiliacion.CodigoAgencia;
            ConsecutivoTransaccion = afiliacionDesafiliacion.ConsecutivoTransaccion;
            CodigoConvenio = afiliacionDesafiliacion.CodigoConvenio;
            CodigoRespuesta = 0;
            TipoIdentificacion = afiliacionDesafiliacion.TipoAcceso;
            IdentificacionCliente = afiliacionDesafiliacion.LlaveServicio;
            NombreCliente = "";
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
            trama.Append(GetDato(CodigoRespuesta.ToString(CultureInfo.InvariantCulture), 2, TipoDato.Numerico));
            trama.Append(GetDato(TipoIdentificacion, 3, TipoDato.Alfanumerico));
            trama.Append(GetDato(IdentificacionCliente, 30, TipoDato.Alfanumerico));
            trama.Append(GetDato(NombreCliente, 50, TipoDato.Alfanumerico));
            trama.Append(GetDato(DiaPagoCiclo.ToString(CultureInfo.InvariantCulture), 3, TipoDato.Numerico));
            trama.Append(GetDato(MontoPromedio.ToString(CultureInfo.InvariantCulture), 18, TipoDato.Numerico));
            return trama.ToString();
        }
    }
}
