using System;

namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama # 6 : Respuesta Fechas Vencimiento Recibos
    /// </summary>
    public class Registro
    {
        /// <summary>
        /// Llave del servicio.
        /// </summary>
        public string LlaveServicio { get; set; }

        /// <summary>
        /// Periodo del primer recibo del  servicio. Debe enviarse un formato de periodo válido ya sea (AAAAMMDD), (AAAAMM), (AAAA).
        /// </summary>
        public DateTime Periodo { get; set; }

        /// <summary>
        /// Monto total a cobrar para el primer recibo del servicio. El punto decimal no debe ser especificado.
        /// </summary>
        public double Monto { get; set; }

        /// <summary>
        /// Fecha de vencimiento del primer recibo del  servicio.
        /// </summary>
        public DateTime FechaVencimiento { get; set; }

        /// <summary>
        /// Número de factura del primer recibo del  servicio.
        /// </summary>
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Dígito verificador del primer recibo del  servicio.
        /// </summary>
        public int Verificador { get; set; }
    }
}
