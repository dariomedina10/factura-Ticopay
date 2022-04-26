using System;

namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama N° 11: Consulta de disponibilidad de servicio ( “Echo”)
    /// </summary>
    public class DisponibilidadServicio : BNConectividad
    {
        /// <summary>
        /// Tipo Mensaje = 800. Tamaño = 4
        /// </summary>
        public int TipoMensaje { get; set; }

        /// <summary>
        /// Código de la SUGEF asignado al Banco Nacional. Actualmente 151. Tamaño = 5
        /// </summary>
        public int CodigoBanco { get; set; }

        /// <summary>
        /// 301000 . Tamaño = 6
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// Identificador único de transacción utilizado para validaciones. Debe ser el mismo que se envíe en la respuesta. Tamaño = 6
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        /// Fecha del sistema formato AAAAMMDDHH24MISS. Tamaño = 14
        /// </summary>
        public DateTime Fecha { get; set; }

        public static DisponibilidadServicio ParserDisponibilidadServicio(string trama)
        {
            var disponibilidadServicio = new DisponibilidadServicio
            {
                TipoMensaje = 800,
                CodigoBanco = 151,
                CodigoTransaccion = 301000,
                ConsecutivoTransaccion = GetNumero(trama, 15, 6),
                Fecha = GetFechaHoraFromTrama(trama, 21),
            };
            return disponibilidadServicio;
        }
    }
}
