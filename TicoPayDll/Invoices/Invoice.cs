using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TicoPayDll.Clients;
using TicoPayDll.Services;

namespace TicoPayDll.Invoices
{
    public class Invoice
    {
        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public string TenantName { get; set; }

        [JsonProperty]
        public string ComercialName { get; set; }

        [JsonProperty]
        public Guid? ClientId { get; protected set; }

        [JsonProperty]
        public virtual Client Client { get; set; }

        [JsonProperty]
        public string ClientName { get; set; }

        [JsonProperty]
        public IdentificacionTypeTipo ClientIdentificationType { get; set; }

        [JsonProperty]
        public string ClientIdentification { get; set; }

        [JsonProperty]
        public string ClientPhoneNumber { get; set; }

        [JsonProperty]
        public string ClientMobilNumber { get; set; }

        [JsonProperty]
        public string ClientEmail { get; set; }

        [JsonProperty]
        public int Number { get; protected set; }

        [JsonProperty]
        public string Alphanumeric { get; protected set; }

        [JsonProperty]
        public string Note { get; protected set; }

        [JsonProperty]
        public string ConsecutiveNumber { get; set; }

        [JsonProperty]
        public byte[] QRCode { get; set; }

        [JsonProperty]
        public decimal SubTotal { get; protected set; }

        [JsonProperty]
        public decimal DiscountPercentaje { get; protected set; }

        [JsonProperty]
        public DateTime? PaymentDate { get; protected set; }

        [JsonProperty]
        public string Transaction { get; protected set; }

        [JsonProperty]
        public decimal DiscountAmount { get; protected set; }

        [JsonProperty]
        public decimal TotalTax { get; protected set; }

        [JsonProperty]
        public decimal Total { get; protected set; }

        [JsonProperty]
        public decimal Balance { get; protected set; }
                
        [JsonProperty]
        public string VoucherKey { get; set; }

        [JsonProperty]
        public DateTime DueDate { get; protected set; }

        [JsonProperty]
        public DateTime? ExpirationDate { get; protected set; }

        [JsonProperty]
        public Status Status { get; protected set; }

        [JsonProperty]
        public FirmType TipoFirma { get; set; }

        [JsonProperty]
        public StatusFirmaDigital StatusFirmaDigital { get; set; }
        
        [JsonProperty]
        public StatusTaxAdministration StatusTribunet { get; set; }

        [JsonProperty]
        public CodigoMoneda CodigoMoneda { get; set; }

        [JsonProperty]
        public DocumentType typeDocument { get; set; }

        [JsonProperty]
        public string SimbolCurrency { get; set; }

        [JsonProperty]
        public bool SendInvoice { get; set; }

        [JsonProperty]
        public virtual IList<InvoiceLineApiDto> InvoiceLines { get; set; }

        [JsonProperty]
        public virtual IList<PaymentInvoiceDto> InvoicePaymentTypes { get; protected set; }

        [JsonProperty]
        public virtual IList<NoteDto> Notes { get; protected set; }
                
        [JsonProperty]
        public string ExternalReferenceNumber { get; set; }
    }

    public class InvoiceLineApiDto
    {
        [JsonProperty]
        public int TenantId { get; set; }

        [JsonProperty]
        public decimal PricePerUnit { get; protected set; }

        [JsonProperty]
        public decimal TaxAmount { get; protected set; }

        [JsonProperty]
        public decimal Total { get; protected set; }

        [JsonProperty]
        public decimal DiscountPercentage { get; protected set; }

        [JsonProperty]
        public string Note { get; protected set; }

        [JsonProperty]
        public string Title { get; protected set; }

        [JsonProperty]
        public decimal Quantity { get; protected set; }

        [JsonProperty]
        public Guid InvoiceId { get; protected set; }

        [JsonProperty]
        public LineType LineType { get; protected set; }

        [JsonProperty]
        public Guid? ServiceId { get; protected set; }

        [JsonProperty]
        public Guid? ProductId { get; protected set; }

        [JsonProperty]
        public Guid? TaxId { get; protected set; }

        [JsonProperty]
        public int LineNumber { get; protected set; }

        [JsonProperty]
        public CodigoTypeTipo CodeTypes { get; protected set; }

        [JsonProperty]
        public string DescriptionDiscount { get; protected set; }

        [JsonProperty]
        public decimal SubTotal { get; protected set; }

        [JsonProperty]
        public decimal LineTotal { get; protected set; }

        [JsonProperty]
        public Service Service { get; set; }

        [JsonProperty]
        public UnidadMedidaType UnitMeasurement { get; set; }

        [JsonProperty]
        public string UnitMeasurementOthers { get; set; }
    }

    public class PaymentInvoiceDto
    {
        [JsonProperty]
        public decimal Amount { get; protected set; }

        [JsonProperty]
        public DateTime PaymentDate { get; set; }

        [JsonProperty]
        public Guid InvoiceId { get; protected set; }

        [JsonProperty]
        public Guid? ExchangeRateId { get; protected set; }

        [JsonProperty]
        public virtual ExchangeRate ExchangeRate { get; protected set; }

        [JsonProperty]
        public CodigoMoneda CodigoMoneda { get; set; }

        [JsonProperty]
        public PaymentInvoiceType PaymentInvoiceType { get; set; }

        [JsonProperty]
        public PaymetnMethodType PaymetnMethodType { get; set; }

        [JsonProperty]
        public string Transaction { get; set; }

        [JsonProperty]
        public string Reference { get; set; }

        [JsonProperty]
        public int? CodigoBanco { get; set; }

        [JsonProperty]
        public int? CodigoAgencia { get; set; }

        [JsonProperty]
        public int? CodigoTransaccion { get; set; }

        [JsonProperty]
        public int? ConsecutivoTransaccion { get; set; }

        [JsonProperty]
        public int? NotaCredito { get; set; }

        [JsonProperty]
        public int? CodigoBancoEmisor { get; set; }

        [JsonProperty]
        public int? NumeroCuenta { get; set; }

        [JsonProperty]
        public int? NumeroCheque { get; set; }

        [JsonProperty]
        public Guid? ParentPaymentInvoiceId { get; set; }
    }

    public class ExchangeRate
    {
        [JsonProperty]
        public int TenantId { get; set; }

        [JsonProperty]
        public string FromCurrencyCode { get; protected set; }

        [JsonProperty]
        public string ToCurrencyCode { get; protected set; }

        [JsonProperty]
        public decimal AverageRate { get; protected set; }

        [JsonProperty]
        public decimal EndOfDayRate { get; protected set; }

        [JsonProperty]
        public DateTime CurrencyRateDate { get; protected set; }        
    }

    public class NoteDto
    {
        [JsonProperty]
        public DateTime CreationTime { get; set; }

        [JsonProperty]
        public decimal Amount { get; set; }

        [JsonProperty]
        public decimal TaxAmount { get; set; }

        [JsonProperty]
        public decimal Total { get; set; }

        [JsonProperty]
        public CodigoMoneda CodigoMoneda { get; set; }

        [JsonProperty]
        public string ConsecutiveNumber { get; set; }

        [JsonProperty]
        public NoteType NoteType { get; set; }

        [JsonProperty]
        public bool SendInvoice { get; set; }

        [JsonProperty]
        public StatusTaxAdministration StatusTribunet { get; set; }

        [JsonProperty]
        public bool IsNoteReceptionConfirmed { get; set; }
    }

    public enum Status
    {
        // Pagada
        Completed,
        // Provisional
        LayBy,
        // Contabilizada
        OnAccount,
        // Reversada
        Returned,
        // Pendiente
        Parked,
        // Anulada
        Voided
    }

    public enum CodigoTypeTipo
    {
        Codigo_Producto_Vendedor,
        Codigo_Producto_Comprador,
        Codigo_Producto_Industria,
        Codigo_UsoInterno,
        Otros,
    }

    public enum PaymetnMethodType
    {
        // Efectivo
        Cash,
        // Tarjeta
        Card,
        // Cheque
        Check,
        // Depósito
        Deposit,
        // Nota de Crédito
        PositiveBalance
    }

    public enum LineType
    {
        Service,
        Product
    }

    public enum PaymentInvoiceType
    {
        Payment,
        Refund
    }

    public enum CodigoMoneda
    {

        /// <comentarios/>
        AED,

        /// <comentarios/>
        AFN,

        /// <comentarios/>
        ALL,

        /// <comentarios/>
        AMD,

        /// <comentarios/>
        ANG,

        /// <comentarios/>
        AOA,

        /// <comentarios/>
        ARS,

        /// <comentarios/>
        AUD,

        /// <comentarios/>
        AWG,

        /// <comentarios/>
        AZN,

        /// <comentarios/>
        BAM,

        /// <comentarios/>
        BBD,

        /// <comentarios/>
        BDT,

        /// <comentarios/>
        BGN,

        /// <comentarios/>
        BHD,

        /// <comentarios/>
        BIF,

        /// <comentarios/>
        BMD,

        /// <comentarios/>
        BND,

        /// <comentarios/>
        BOB,

        /// <comentarios/>
        BOV,

        /// <comentarios/>
        BRL,

        /// <comentarios/>
        BSD,

        /// <comentarios/>
        BTN,

        /// <comentarios/>
        BWP,

        /// <comentarios/>
        BYR,

        /// <comentarios/>
        BZD,

        /// <comentarios/>
        CAD,

        /// <comentarios/>
        CDF,

        /// <comentarios/>
        CHE,

        /// <comentarios/>
        CHF,

        /// <comentarios/>
        CHW,

        /// <comentarios/>
        CLF,

        /// <comentarios/>
        CLP,

        /// <comentarios/>
        CNY,

        /// <comentarios/>
        COP,

        /// <comentarios/>
        COU,

        /// <comentarios/>
        CRC,

        /// <comentarios/>
        CUC,

        /// <comentarios/>
        CUP,

        /// <comentarios/>
        CVE,

        /// <comentarios/>
        CZK,

        /// <comentarios/>
        DJF,

        /// <comentarios/>
        DKK,

        /// <comentarios/>
        DOP,

        /// <comentarios/>
        DZD,

        /// <comentarios/>
        EGP,

        /// <comentarios/>
        ERN,

        /// <comentarios/>
        ETB,

        /// <comentarios/>
        EUR,

        /// <comentarios/>
        FJD,

        /// <comentarios/>
        FKP,

        /// <comentarios/>
        GBP,

        /// <comentarios/>
        GEL,

        /// <comentarios/>
        GHS,

        /// <comentarios/>
        GIP,

        /// <comentarios/>
        GMD,

        /// <comentarios/>
        GNF,

        /// <comentarios/>
        GTQ,

        /// <comentarios/>
        GYD,

        /// <comentarios/>
        HKD,

        /// <comentarios/>
        HNL,

        /// <comentarios/>
        HRK,

        /// <comentarios/>
        HTG,

        /// <comentarios/>
        HUF,

        /// <comentarios/>
        IDR,

        /// <comentarios/>
        ILS,

        /// <comentarios/>
        INR,

        /// <comentarios/>
        IQD,

        /// <comentarios/>
        IRR,

        /// <comentarios/>
        ISK,

        /// <comentarios/>
        JMD,

        /// <comentarios/>
        JOD,

        /// <comentarios/>
        JPY,

        /// <comentarios/>
        KES,

        /// <comentarios/>
        KGS,

        /// <comentarios/>
        KHR,

        /// <comentarios/>
        KMF,

        /// <comentarios/>
        KPW,

        /// <comentarios/>
        KRW,

        /// <comentarios/>
        KWD,

        /// <comentarios/>
        KYD,

        /// <comentarios/>
        KZT,

        /// <comentarios/>
        LAK,

        /// <comentarios/>
        LBP,

        /// <comentarios/>
        LKR,

        /// <comentarios/>
        LRD,

        /// <comentarios/>
        LSL,

        /// <comentarios/>
        LYD,

        /// <comentarios/>
        MAD,

        /// <comentarios/>
        MDL,

        /// <comentarios/>
        MGA,

        /// <comentarios/>
        MKD,

        /// <comentarios/>
        MMK,

        /// <comentarios/>
        MNT,

        /// <comentarios/>
        MOP,

        /// <comentarios/>
        MRO,

        /// <comentarios/>
        MUR,

        /// <comentarios/>
        MVR,

        /// <comentarios/>
        MWK,

        /// <comentarios/>
        MXN,

        /// <comentarios/>
        MXV,

        /// <comentarios/>
        MYR,

        /// <comentarios/>
        MZN,

        /// <comentarios/>
        NAD,

        /// <comentarios/>
        NGN,

        /// <comentarios/>
        NIO,

        /// <comentarios/>
        NOK,

        /// <comentarios/>
        NPR,

        /// <comentarios/>
        NZD,

        /// <comentarios/>
        OMR,

        /// <comentarios/>
        PAB,

        /// <comentarios/>
        PEN,

        /// <comentarios/>
        PGK,

        /// <comentarios/>
        PHP,

        /// <comentarios/>
        PKR,

        /// <comentarios/>
        PLN,

        /// <comentarios/>
        PYG,

        /// <comentarios/>
        QAR,

        /// <comentarios/>
        RON,

        /// <comentarios/>
        RSD,

        /// <comentarios/>
        RUB,

        /// <comentarios/>
        RWF,

        /// <comentarios/>
        SAR,

        /// <comentarios/>
        SBD,

        /// <comentarios/>
        SCR,

        /// <comentarios/>
        SDG,

        /// <comentarios/>
        SEK,

        /// <comentarios/>
        SGD,

        /// <comentarios/>
        SHP,

        /// <comentarios/>
        SLL,

        /// <comentarios/>
        SOS,

        /// <comentarios/>
        SRD,

        /// <comentarios/>
        SSP,

        /// <comentarios/>
        STD,

        /// <comentarios/>
        SVC,

        /// <comentarios/>
        SYP,

        /// <comentarios/>
        SZL,

        /// <comentarios/>
        THB,

        /// <comentarios/>
        TJS,

        /// <comentarios/>
        TMT,

        /// <comentarios/>
        TND,

        /// <comentarios/>
        TOP,

        /// <comentarios/>
        TRY,

        /// <comentarios/>
        TTD,

        /// <comentarios/>
        TWD,

        /// <comentarios/>
        TZS,

        /// <comentarios/>
        UAH,

        /// <comentarios/>
        UGX,

        /// <comentarios/>
        USD,

        /// <comentarios/>
        USN,

        /// <comentarios/>
        UYI,

        /// <comentarios/>
        UYU,

        /// <comentarios/>
        UZS,

        /// <comentarios/>
        VEF,

        /// <comentarios/>
        VND,

        /// <comentarios/>
        VUV,

        /// <comentarios/>
        WST,

        /// <comentarios/>
        XAF,

        /// <comentarios/>
        XAG,

        /// <comentarios/>
        XAU,

        /// <comentarios/>
        XBA,

        /// <comentarios/>
        XBB,

        /// <comentarios/>
        XBC,

        /// <comentarios/>
        XBD,

        /// <comentarios/>
        XCD,

        /// <comentarios/>
        XDR,

        /// <comentarios/>
        XOF,

        /// <comentarios/>
        XPD,

        /// <comentarios/>
        XPF,

        /// <comentarios/>
        XPT,

        /// <comentarios/>
        XSU,

        /// <comentarios/>
        XTS,

        /// <comentarios/>
        XUA,

        /// <comentarios/>
        XXX,

        /// <comentarios/>
        YER,

        /// <comentarios/>
        ZAR,

        /// <comentarios/>
        ZMW,

        /// <comentarios/>
        ZWL,
    }

    public enum NoteType
    {
        // Débito
        Débito,
        // Crédito
        Crédito
    }

    public enum StatusTaxAdministration
    {
        // No Enviado
        NoEnviada = 0,
        // Recibido
        Recibido,
        // Procesando
        Procesando,
        // Aceptado
        Aceptado,
        // Rechazado
        Rechazado,
        // Error
        Error
    }

    public enum DocumentType
    {
        Invoice = 1,
        Ticket = 4
    }
}
