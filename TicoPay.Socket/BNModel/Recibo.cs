using System;

namespace Ticopay.BNModel
{
    /// <summary>
    ///  /// Trama N°2:  Respuesta a consulta de recibos.
    /// </summary>
   public class Recibo
    {
       /// <summary>
        /// Periodo del recibo N del primer servicio. Debe enviarse un formato de periodo válido ya sea (AAAAMMDD), (AAAAMM), (AAAA). Tamaño = 8
       /// </summary>
       public DateTime Periodo { get; set; }

       /// <summary>
       /// Monto total a cobrar para el recibo N del primer servicio. El punto decimal no debe ser especificado. Tamaño = 18(2)
       /// </summary>
       public double Monto { get; set; }

       /// <summary>
       /// Fecha de vencimiento del recibo N del primer servicio. Tamaño = 8
       /// </summary>
       public DateTime Vencimiento { get; set; }

       /// <summary>
       /// Número de factura del recibo N del primer servicio. Es un campo requerido. Tamaño = 20
       /// </summary> 
       public string NumeroFactura { get; set; }

       /// <summary>
       /// Dígito verificador del recibo N del primer servicio. Tamaño = 1
       /// </summary>
       public int Verificador { get; set; }
    }
}
