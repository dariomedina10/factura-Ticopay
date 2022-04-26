namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama N°7:  Aplicación de notas de crédito (depósito) y débito (comisión). Recaudaciones
    /// </summary>
   public class Recaudacion
    {
       /// <summary>
        /// Código del grupo de rubros que sumariza el depósito.
       /// </summary>
       public int Codigo { get; set; }

       /// <summary>
       /// Número de la primera nota de crédito por recaudación.
       /// </summary>
       public int NotaCredito { get; set; }

       /// <summary>
       /// Monto de la primera nota de crédito por recaudación de los rubros del grupo.
       /// </summary>
       public int Monto { get; set; }

       /// <summary>
       /// Cuenta bancaria de la primera nota de crédito por recaudación de los rubros del grupo.
       /// </summary>
       public int CuentaBancaria { get; set; }
    }
}
