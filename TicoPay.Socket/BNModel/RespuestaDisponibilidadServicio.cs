using System.Globalization;
using System.Text;
//using Asadas.Core.BL;

namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama N° 12: Respuesta a consulta de disponibilidad de servicio (Echo)
    /// </summary>
    public class RespuestaDisponibilidadServicio : RespuestaBNConectivdad
    {
        /// <summary>
        /// El codigo para este mensaje es 810. Tamaño = 4
        /// </summary>
        public int TipoMensaje { get; set; }

        /// <summary>
        /// Código de la SUGEF asignado al Banco Nacional. Actualmente es 151. Tamaño = 5
        /// </summary>
        public int CodigoBanco { get; set; }

        /// <summary>
        /// 100100 : calendarización por vencimiento de recibos. Tamaño = 6
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// Identificador único de transacción utilizado para validaciones. Debe ser el que se envió en la consulta. Tamaño = 6
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        /// 00 :  Estado normal en entidad externa, 11 :  Problemas en entidad externa. Tamaño = 2
        /// </summary>
        public int CodigoRespuesta { get; set; }


        public RespuestaDisponibilidadServicio(DisponibilidadServicio disponibilidadServicio)
        {
            TipoMensaje = 810;
            CodigoBanco = 151;
            CodigoTransaccion = 301001;
            ConsecutivoTransaccion = disponibilidadServicio.ConsecutivoTransaccion;
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
            trama.Append(GetDato(CodigoTransaccion.ToString(CultureInfo.InvariantCulture), 6, TipoDato.Numerico));
            trama.Append(GetDato(ConsecutivoTransaccion.ToString(CultureInfo.InvariantCulture), 6, TipoDato.Numerico));
            trama.Append(GetDato(CodigoRespuesta.ToString(CultureInfo.InvariantCulture), 2, TipoDato.Numerico));
            return trama.ToString();
        }

    }
}
