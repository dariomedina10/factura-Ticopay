namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama N°4: Respuesta a aplicación del pago
    /// </summary>
    public class Rubro
    {
        public Rubro(int codigo, string monto)
        {
            Codigo = codigo;
            Monto = monto;
        }

        /// <summary>
        /// Código del rubro del recibo. Los códigos de rubro son asignados por común acuerdo. En caso de transmisión de uno que no se tenga registrado, se tomará como “Otros” para efectos de impresión.
        /// </summary>
        public int Codigo { get; set; }

        /// <summary>
        /// Monto o valor para el rubro del recibo. Este campo puede venir como un valor numérico(ceros a la izquierda) o como un valor alfanumérico(espacios a la derecha)
        /// </summary>
        public string Monto { get; set; }
    }
}
