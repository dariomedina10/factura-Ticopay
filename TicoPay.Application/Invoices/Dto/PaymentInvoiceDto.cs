using Abp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices.XSD;

namespace TicoPay.Invoices.Dto
{
    /// <summary>
    /// Clase que contiene la información de los medios de pago / Contains the Payment Methods information
    /// </summary>
    [AutoMapFrom(typeof(PaymentInvoice))]
    public class PaymentInvoiceDto
    {
        /// <summary>
        /// Obtiene o Almacena el Monto Pagado / Gets or Sets the Amount Paid.
        /// </summary>
        /// <value>
        /// Monto Pagado / Amount Paid.
        /// </value>
        public decimal Amount { get; protected set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha del Pago / Gets or sets the payment date.
        /// </summary>
        /// <value>
        /// Fecha del Pago / Payment date.
        /// </value>
        public DateTime PaymentDate { get; set; }


        /// <summary>
        /// Obtiene o Almacena el Id de la Factura o ticket / Gets or sets the invoice or ticket Id.
        /// </summary>
        /// <value>
        /// Id de la Factura / Invoice Id.
        /// </value>
        public Guid InvoiceId { get; protected set; }

        /// <summary>
        /// Obtiene o Almacena el Id de la Tasa de Cambio / Gets or sets the exchange rate identifier.
        /// </summary>
        /// <value>
        /// Id de la Tasa de Cambio / The exchange rate identifier.
        /// </value>
        public Guid? ExchangeRateId { get; protected set; }

        /// <summary>
        /// Obtiene o Almacena La Tasa de Cambio / Gets or sets the exchange rate.
        /// </summary>
        /// <value>
        /// Tasa de Cambio / The exchange rate.
        /// </value>
        public virtual ExchangeRate ExchangeRate { get; protected set; }

        /// <summary>
        /// Obtiene o Almacena el Código de la Moneda / Gets or sets the Currency Code
        /// </summary>
        /// <value>
        /// Código moneda / Currency Code.
        /// </value>
        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }


        /// <summary>
        /// Obtiene o Almacena el Tipo de Pago / Gets or sets the type of the payment invoice.
        /// </summary>
        /// <value>
        /// The type of the payment invoice / Payment Type.
        /// </value>
        public PaymentType PaymentInvoiceType { get; set; }

        /// <summary>
        /// Obtiene o Almacena la forma de Pago (0 Efectivo, 1 Tarjeta, 2 Cheque, 3 Deposito o Transferencia) /
        /// Gets or Sets the Payment method Type (0 Cash, 1 Credit or Debit Card , 2 Check , 3 Deposit or Transference).
        /// </summary>
        /// <value>
        /// Forma de Pago / Payment Method Type .
        /// </value>
        public PaymetnMethodType PaymetnMethodType { get; set; }

        public string Transaction { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de comprobante o Deposito / Gets or Sets the Reference number of the Deposit or Transfer.
        /// </summary>
        /// <value>
        /// Número de comprobante o Deposito / Reference number of the Deposit or Transfer.
        /// </value>
        public string Reference { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Banco del deposito o transferencia (Puede ser null) / Gets or Sets the Bank Id (Can be null)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? CodigoBanco { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? CodigoAgencia { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? CodigoTransaccion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ConsecutivoTransaccion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? NotaCredito { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? CodigoBancoEmisor { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? NumeroCuenta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? NumeroCheque { get; set; }


        [JsonIgnore]
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public long? DeleterUserId { get; set; }
        [JsonIgnore]
        public bool? IsPaymentReversed { get; set; }
        [JsonIgnore]
        public bool? IsPaymentUsed { get; set; }
        [JsonIgnore]
        /// <summary>Gets or sets the InvoiceId. </summary>
        public Guid? ParentPaymentInvoiceId { get; set; }
        [JsonIgnore]
        public DateTime? DeletionTime { get; set; }
    }
}
