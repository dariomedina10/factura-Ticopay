using System.Collections.Generic;

namespace Ticopay.BNModel
{
    /// <summary>
    ///  /// Trama N°2:  Respuesta a consulta de recibos.
    /// </summary>
    public class ServicioPendiente
    {
        /// <summary>
        /// Valor del primer servicio. Tamaño = 30
        /// </summary>
        public string LlaveAcceso { get; set; }

        public List<Recibo> Recibos { get; set; }

        public ServicioPendiente()
        {
            Recibos = new List<Recibo>();
        }
    }
}
