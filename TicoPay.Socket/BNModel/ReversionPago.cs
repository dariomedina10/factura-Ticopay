using System;

namespace Ticopay.BNModel
{
    /// <summary>
    /// Trama N°5:  Reversión de pagos.
    /// </summary>
    public class ReversionPago : BNConectividad
    {
        /// <summary>
        /// Tipo de Mensaje = 420. Tamaño = 4
        /// </summary>
        public int TipoMensaje { get; set; }

        /// <summary>
        /// Código de la SUGEF asignado al Banco Nacional. Actualmente 151. Tamaño = 5
        /// </summary>
        public int CodigoBanco { get; set; }

        /// <summary>
        /// Código de la agencia de donde se envió la reversión. Tamaño = 6
        /// </summary>
        public int CodigoAgencia { get; set; }

        /// <summary>
        /// 800000. Tamaño = 6
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// identificador único de transacción utilizado para validaciones. Debe ser el mismo que se envíe en la respuesta. Tamaño = 6
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        /// Número que identifica al convenio y es asignado por el Banco Nacional. Tamaño = 3
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// Valor del servicio. Tamaño = 30
        /// </summary>
        public string ValorServicio { get; set; }

        /// <summary>
        /// Periodo del recibo a reversar. Debe enviarse un formato de periodo válido ya sea (AAAAMMDD), (AAAAMM), (AAAA). Tamaño = 8 
        /// </summary>
        public string Periodo { get; set; }

        /// <summary>
        /// Monto total a reversar correspondiente al recibo. El punto decimal no debe ser especificado. Tamaño = 18(2)
        /// </summary>
        public double Monto { get; set; }

        /// <summary>
        /// Número de factura del recibo a reversar. Campo requerido. Tamaño = 20 
        /// </summary>
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Dígito verificador del recibo a reversar. Tamaño = 1
        /// </summary>
        public int Verificador { get; set; }

        /// <summary>
        /// Fecha y hora de la transacción, utilizando el formato AAAAMMDDHH24MISS. Tamaño = 14
        /// </summary>
        public DateTime FechaTransaccion { get; set; }

        /// <summary>
        /// Número de la nota de crédito con que se realizará el depósito de lo recaudado. Tamaño = 12
        /// </summary>
        public int NotaCredito { get; set; }

        public static ReversionPago ParserReversionPago(string trama)
        {
            var reversionpago = new ReversionPago
            {
                TipoMensaje = 430,
                CodigoBanco = GetCodigoBanco(trama),
                CodigoAgencia = GetCodigoAgencia(trama),
                CodigoTransaccion = 800000,
                ConsecutivoTransaccion = GetConsecutivoTransaccion(trama),
                CodigoConvenio = GetCodigoConvenio(trama),
                ValorServicio = GetLlaveAcceso(trama, 30),
                Periodo = GetCadena(trama, 60, 8),
                Monto = GetMontoFromTrama(trama, 68, 18),
                NumeroFactura = GetCadena(trama, 86, 20),
                Verificador = GetNumero(trama, 106, 1),
                FechaTransaccion = GetFechaHoraFromTrama(trama, 107),
                NotaCredito = GetNumero(trama, 121, 12),
            };
            return reversionpago;
        }

    }
}
