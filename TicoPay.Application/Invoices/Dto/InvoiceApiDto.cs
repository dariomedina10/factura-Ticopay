using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicoPay.Clients.Dto;
using TicoPay.Invoices.XSD;
using TicoPay.Services;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Invoices.Dto
{
    /// <summary>
    /// Clase que contiene la información de la Factura o tiquete / Contains the Invoice or ticket information
    /// </summary>
    /// <seealso cref="Abp.Application.Services.Dto.EntityDto{System.Guid}" />
    [AutoMapFrom(typeof(Invoice))]
    public class InvoiceApiDto : EntityDto<Guid>
    {
        /// <summary>
        /// Obtiene el Nombre del Tenant que creo la factura / Gets or Sets the Tenant that created the invoice.
        /// </summary>
        /// <value>
        /// Nombre del Tenant que creo la factura.
        /// </value>
        public string TenantName { get; set; }

        [JsonIgnore]
        public string ComercialName { get; set; }

        /// <summary>
        /// Obtiene el ID del Cliente al que se va a facturar , este campo puede ser null si es un tiquete / 
        /// Gets the Client Id , this field can be null it is Ticket.
        /// </summary>
        /// <value>
        /// ID de Cliente / Client Id.
        /// </value>
        public Guid? ClientId { get; protected set; }

        /// <summary>
        /// Obtiene el Cliente (Puede ser null si es un tiquete) / Gets the Client (can be null if it is a ticket).
        /// </summary>
        /// <value>
        /// El Cliente / The Client.
        /// </value>
        public virtual ClientDto Client { get; set; }

        /// <summary>
        /// Obtiene el Número de factura.
        /// </summary>
        /// <value>
        /// Número de factura.
        /// </value>
        [JsonIgnore]
        public int Number { get; protected set; }

        /// <summary>
        /// Obtiene el Alfanumérico.
        /// </summary>
        /// <value>
        /// Alfanumérico.
        /// </value>
        [JsonIgnore]
        [MaxLength(50)]
        public string Alphanumeric { get; protected set; }

        /// <summary>
        /// Obtiene la Nota (Sin uso).
        /// </summary>
        /// <value>
        /// Nota.
        /// </value>
        [JsonIgnore]
        [MaxLength(500)]
        public string Note { get; protected set; }

        /// <summary>
        /// Obtiene el Número Consecutivo de la Factura o tiquete / Gets the invoice or ticket Consecutive number.
        /// </summary>
        /// <value>
        /// Número Consecutivo de la Factura o tiquete / Invoice or Ticket Consecutive number.
        /// </value>
        public string ConsecutiveNumber { get; set; }

        /// <summary>
        /// Obtiene el Código QR de la Factura or tiquete/ Gets the QR Code of the invoice or ticket.
        /// </summary>
        /// <value>
        /// Código QR de la Factura o tiquete / Invoice or ticket QR Code.
        /// </value>
        public byte[] QRCode { get; set; }

        /// <summary>
        /// Obtiene el Sub Total de la Factura (En Desuso) / Gets the Invoice or ticket Net Sale (Deprecated).
        /// </summary>
        /// <value>
        /// Sub Total de la Factura (En Desuso) / Net Sale (Deprecated).
        /// </value>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal SubTotal {
            get
            {
                return NetaSale;
            }
            protected set { }
        }

        /// <summary>
        /// Obtiene el Porcentaje de Descuento Global de la factura / Gets the Invoice Global Discount Percentage.
        /// </summary>
        /// <value>
        /// Descuento Global de la factura / Global Discount Percentage.
        /// </value>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal DiscountPercentaje { get; protected set; }

        /// <exclude />
        [JsonIgnore]
        public DateTime? PaymentDate { get; protected set; }

        /// <exclude />
        [JsonIgnore]
        public string Transaction { get; protected set; }

        /// <summary>
        /// Obtiene el tipo de Moneda usada en la factura o tiquete / Gets the invoice or ticket Currency.
        /// </summary>
        /// <value>
        ///   Tipo de Moneda usada en la factura / Invoice or ticket Currency.
        /// </value>
        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }

        /// <exclude />
        [JsonIgnore]
        public decimal TotalServGravados { get; set; }

        /// <exclude />
        [JsonIgnore]
        public decimal TotalServExento { get; set; }

        /// <exclude />
        [JsonIgnore]
        public decimal TotalProductExento { get; set; }

        /// <exclude />
        [JsonIgnore]
        public decimal TotalProductGravado { get; set; }

        /// <exclude />
        [JsonIgnore]
        public decimal TotalGravado { get; set; }

        /// <exclude />
        [JsonIgnore]
        public decimal TotalExento { get; set; }

        /// <exclude />
        [JsonIgnore]
        public decimal SaleTotal { get; set; }

        /// <summary>
        /// Obtiene el Sub Total de la Factura o tiquete / Gets the Invoice or ticket Net Sale amount.
        /// </summary>
        /// <value>
        /// Sub Total de la Factura / Net sale .
        /// </value>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal NetaSale { get; set; }

        /// <summary>
        /// Obtiene el Simbolo de la moneda / Gets the Currency Symbol.
        /// </summary>
        /// <value>
        /// Simbolo de la moneda / Currency Symbol.
        /// </value>
        public string SimbolCurrency { get { return CodigoMoneda.ToString(); } }

        /// <summary>
        /// Obtiene si la factura fue enviada a Hacienda / Gets if the invoice or ticket was sent to Hacienda.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si fue enviada a Hacienda; Sino, <c>false</c> / <c>true</c> if the Invoice or Ticket was sent, otherwise <c>false</c>.
        /// </value>
        public bool SendInvoice { get; set; }

        /// <summary>
        /// Obtiene la Clave del comprobante / Gets the Voucher Key.
        /// </summary>
        /// <value>
        /// Clave del comprobante / Voucher Key.
        /// </value>
        [MaxLength(50)]
        public string VoucherKey { get; set; }

        /// <summary>
        /// Obtiene el Monto del Descuento Global / Gets the Global Discount Amount.
        /// </summary>
        /// <value>
        /// Monto del Descuento Global / Global Discount Amount.
        /// </value>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal DiscountAmount { get; protected set; }

        /// <summary>
        /// Obtiene el Total del Impuesto de la factura o tiquete / Gets the Total Tax of the Invoice or Ticket.
        /// </summary>
        /// <value>
        /// Total del Impuesto / Total Tax.
        /// </value>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalTax { get; protected set; }

        /// <summary>
        /// Obtiene el Total de la Factura o tiquete / Gets the Total Amount of the invoice or ticket.
        /// </summary>
        /// <value>
        /// Total de la Factura / Total Amount.
        /// </value>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Total { get; protected set; }

        /// <summary>
        /// Obtiene el Saldo Actual de la Factura o tiquete / Gets the Current balance of the invoice or ticket.
        /// </summary>
        /// <value>
        /// Saldo Actual de la Factura / Invoice or ticket Balance.
        /// </value>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Balance { get; protected set; }

        /// <summary>
        /// Obtiene la Fecha de la Factura o tiquete / Gets the Invoice or Ticket Date.
        /// </summary>
        /// <value>
        /// Fecha de la Factura or tiquete / Invoice or ticket Date.
        /// </value>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Obtiene la fecha de vencimiento de la factura si es a crédito / Gets the Expiration Date of the Credit Invoice or ticket
        /// </summary>
        /// <value>
        /// Fecha de vencimiento de la factura si es a crédito / Credit Invoice or Ticket Expiration Date.
        /// </value>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Obtiene el Estatus del Envió de la Factura a Hacienda / Gets the Delivery Status of the Invoice or ticket to Hacienda.
        /// </summary>
        /// <value>
        /// Estatus del Envió de la Factura a Hacienda / Status of Delivery to Hacienda.
        /// </value>
        public StatusTaxAdministration StatusTribunet { get; set; }

        /// <summary>
        /// Obtiene el Estatus interno de la Factura / Gets the Invoice or ticket internal Status.
        /// </summary>
        /// <value>
        /// Estatus de la Factura o tiquete / Invoice or ticket Status.
        /// </value>
        public Status Status { get; protected set; }

        /// <summary>
        /// Obtiene el Tipo de Firma de la Factura / Gets the Invoice or Ticket Signature Type.
        /// </summary>
        /// <value>
        /// Tipo de Firma de la Factura / Signature Type.
        /// </value>
        public FirmType TipoFirma { get; set; }

        /// <summary>
        /// Obtiene el Estatus de la Firma de la Factura / Gets the Invoice Signature Status.
        /// </summary>
        /// <value>
        /// Estatus de la Firma de la Factura / Signature Status.
        /// </value>
        public StatusFirmaDigital StatusFirmaDigital { get; set; }


        /// <summary>
        ///  Obtiene o Almacena el Tipo de documento 1: Factura o 4: Ticket Electrónico  / Gets the Document type.
        /// </summary>
        ///  /// <value>
        /// Tipo de documento / Document Type
        /// </value>
        public TypeDocumentInvoice TypeDocument { get; set; } = TypeDocumentInvoice.Invoice;

        /// <summary>
        /// Obtiene el Nombre y apellido del cliente (En caso de los tiquetes que no tiene cliente Creado) / Gets the Name and Last name of the Client (In case of Ticket that don't have a Client Created).
        /// </summary>
        /// <value>
        /// Nombre completo del cliente / Client Fullname
        /// </value>
        public string ClientName { get; set; }

        /// <summary>
        /// Obtiene el tipo de identificación del cliente (En caso de los tiquetes que no tiene cliente Creado) / Gets the Identification Type of the Client (In case of Ticket that don't have a Client Created)..
        /// </summary>
        /// <value>
        /// Tipo de identificación del cliente / Client Identification Type
        /// </value>
        public XSD.IdentificacionTypeTipo ClientIdentificationType { get; set; }

        /// <summary>
        /// Obtiene el numero de identificación del cliente (En caso de los tiquetes que no tiene cliente Creado) / Gets the Identification number of the Client (In case of Ticket that don't have a Client Created)..
        /// </summary>
        /// <value>
        /// Identificación del cliente / Identification Number
        /// </value>
        [StringLength(20)]
        public string ClientIdentification { get; set; }

        /// <summary>
        /// Obtiene el número telefónico del cliente (En caso de los tiquetes que no tiene cliente Creado) / Gets Phone number of the Client (In case of Ticket that don't have a Client Created)..
        /// </summary>
        /// <value>
        /// Número de telefóno fijo del cliente / Phone number
        /// </value>
        public string ClientPhoneNumber { get; set; }

        /// <summary>
        /// Obtiene el número movil del cliente (En caso de los tiquetes que no tiene cliente Creado) / Gets Cellphone number of the Client (In case of Ticket that don't have a Client Created)..
        /// </summary>
        /// <value>
        /// Número móvil del cliente / Cellphone number
        /// </value>
        public string ClientMobilNumber { get; set; }

        /// <summary>
        /// Obtiene la dirección de correo electrónico del cliente (En caso de los tiquetes que no tiene cliente Creado) / Gets Email of the Client (In case of Ticket that don't have a Client Created)..
        /// </summary>
        /// <value>Correo electrónico del cliente / Client Email</value>
        public string ClientEmail { get; set; }

        /// <summary>
        /// Obtiene las lineas de la factura / Gets the Invoice or ticket Lines.
        /// </summary>
        /// <value>
        /// Lineas de la factura / Invoice or Ticket Lines.
        /// </value>
        public virtual IList<InvoiceLineApiDto> InvoiceLines { get; set; }

        /// <summary>
        /// Obtiene los Pagos Recibidos de la factura / Gets the Payment Methods of the Invoice or Ticket.
        /// </summary>
        /// <value>
        /// Pagos Recibidos de la factura / Payment Methods.
        /// </value>
        public virtual IList<PaymentInvoiceDto> InvoicePaymentTypes { get; protected set; }


        /// <summary>
        /// Obtiene las Notas de Crédito o Débito de la Factura o tiquete / Gets the Credit and Debit Memos applied to the Invoice or ticket.
        /// </summary>
        /// <value>
        /// Notas de Crédito o Débito de la Factura / Credit or Debit Memos.
        /// </value>
        public virtual IList<NoteDto> Notes { get; protected set; }

        /// <summary>
        /// Obtiene el Numero de Referencia Externa (Numero de referencia de control de su sistema) / Gets the External Reference number of the Invoice or ticket (The internal reference number of your system).
        /// </summary>
        /// <value>
        /// Numero de Referencia Externa (Usuarios del Api) / External Reference number.
        /// </value>
        public string ExternalReferenceNumber { get; set; }

        // public virtual IList<InvoiceHistoryStatus> InvoiceHistoryStatuses { get; protected set; }
    }

 
}
