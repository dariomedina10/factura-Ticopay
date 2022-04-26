using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Invoices.BN
{
    public class AplicacionPago : BNConectividad
    {
        /// <summary>
        /// El codigo para este mensaje es 200, Tamaño = 4
        /// </summary>
        public int TipoMensaje { get; set; }

        /// <summary>
        /// Código de la SUGEF asignado al Banco Nacional. Actualmente 151. Tamaño = 5
        /// </summary>
        public int CodigoBanco { get; set; }

        /// <summary>
        /// Código de la agencia desde donde se envió el pago. Tamaño = 6
        /// </summary>
        public int CodigoAgencia { get; set; }

        /// <summary>
        /// 800000 Tamaño = 6
        /// </summary>
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// Identificador único de transacción utilizado para validaciones. Debe ser el mismo que se envíe en la respuesta. Tamaño = 6
        /// </summary>
        public int ConsecutivoTransaccion { get; set; }

        /// <summary>
        /// Número que identifica al convenio y es asignado por el Banco Nacional. Tamaño = 3
        /// </summary>
        public int CodigoConvenio { get; set; }

        /// <summary>
        /// Valor del servicio al que pertenece el recibo a pagar. Tamaño = 30
        /// </summary>
        public string ValorServicio { get; set; }

        /// <summary>
        /// Periodo del recibo a pagar. Debe enviarse un formato de periodo válido ya sea (AAAAMMDD), (AAAAMM), (AAAA). Tamaño = 8
        /// </summary>
        public string Periodo { get; set; }

        /// <summary>
        /// Monto total del recibo a pagar. El punto decimal no debe ser especificado. Tamaño = 18(2)
        /// </summary>
        public double Monto { get; set; }

        /// <summary>
        /// Fecha de vencimiento del recibo a pagar. Tamaño = 8
        /// </summary>
        public DateTime FechaVencimiento { get; set; }

        /// <summary>
        /// Número de factura del recibo a pagar. Campo requerido. Tamaño = 20
        /// </summary>
        public int NumeroFactura { get; set; }

        /// <summary>
        /// Dígito verificador del recibo a pagar. Tamaño = 1
        /// </summary>
        public int Verificador { get; set; }

        /// <summary>
        /// Fecha y hora de la transacción, utilizando el formato AAAAMMDDHH24MISS. Tamaño = 14
        /// </summary>
        public DateTime FechaTransaccion { get; set; }

        /// <summary>
        /// Número de la nota de débito con que se realizará el depósito de lo recaudado(en caso de que todo lo recaudado sea depositado a una sola cuenta). Tamaño = 12
        /// </summary>
        public int NotaCredito { get; set; }

        /// <summary>
        /// Código, según SUGEF, del banco emisor del cheque. Tamaño = 4
        /// </summary>
        public int CodigoBancoEmisor { get; set; }

        /// <summary>
        /// Número de cuenta del cheque. Tamaño = 19
        /// </summary>
        public int NumeroCuenta { get; set; }

        /// <summary>
        /// Número del cheque con que se realiza el pago. Tamaño = 15
        /// </summary>
        public int NumeroCheque { get; set; }

        /// <summary>
        /// Número de cuotas a pagar.Se utiliza en convenios en los que no se maneja facturas si no cuotas. Tamaño = 4
        /// </summary>
        public int NumeroCuotas { get; set; }

        /// <summary>
        /// Datos en formato XML, con los valores de los campos adicionales definidos para el convenio.
        /// </summary>
        public string CamposAdicionales { get; set; }

        public static AplicacionPago ParserAplicacionPago(string trama)
        {
            var aplicacionPago = new AplicacionPago
            {
                TipoMensaje = 200,
                CodigoBanco = 151,
                CodigoAgencia = GetCodigoAgencia(trama),
                CodigoTransaccion = 800000,
                ConsecutivoTransaccion = GetConsecutivoTransaccion(trama),
                CodigoConvenio = GetCodigoConvenio(trama),
                ValorServicio = GetLlaveAcceso(trama, 30),
                Periodo = GetCadena(trama, 60, 8),
                Monto = GetMontoFromTrama(trama, 68, 18),
                FechaVencimiento = GetFechaFromTrama(trama, 86),
                NumeroFactura = GetNumero(trama, 94, 20),
                Verificador = GetNumero(trama, 114, 1),
                FechaTransaccion = GetFechaHoraFromTrama(trama, 115),
                NotaCredito = GetNumero(trama, 129, 12),
                CodigoBancoEmisor = GetNumero(trama, 141, 4),
                NumeroCuenta = GetNumero(trama, 145, 19),
                NumeroCheque = GetNumero(trama, 164, 15),
                NumeroCuotas = GetNumero(trama, 179, 4)
            };
            return aplicacionPago;
        }
    }
}
