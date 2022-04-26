using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TicoPay.Invoices.XSD;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Invoices.Dto
{

    /// <summary>
    /// Clase que contiene los datos para la creación de una nueva factura / Contains the Invoice or ticket information
    /// </summary>
    public class CreateInvoiceDTO : ICustomValidate, IUnityInvoice
    {
        /// <summary>
        /// Obtiene o Almacena el ID del Cliente al que se va a facturar , este campo puede enviarse en null si se esta enviando un tiquete / Gets or Sets the Client Id , this field can be null if creating a Ticket.
        /// </summary>
        /// <value>
        /// ID del Cliente / Client Id.
        /// </value>
        
        public Guid? ClientId { get; set; }
        /// <summary>
        /// Obtiene o Almacena las Lineas de la factura o tiquete / Gets or Sets the Invoice or Ticket Lines.
        /// </summary>
        /// <value>
        /// Lineas de la factura o tiquete/ Invoice or ticket Lines.
        /// </value>
        [Required]
        public virtual IList<ItemInvoice> InvoiceLines { get; set; }


        /// <summary>
        /// Obtiene o Almacena la lista de Medios de Pago de la factura o tiquete , este campo debe ser null en caso de ser una factura de crédito / Gets or Sets the Payment methods of the invoice or ticket, this field must be null if payment method is credit.
        /// </summary>
        /// <value>
        /// Medios de Pago de la factura or tiquete / Invoice or ticket Payment methods.
        /// </value>
        public List<PaymentInvoceDto> ListPaymentType { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto del Descuento General , este campo puede ser null , o contener el monto de descuento o porcentaje de descuento de acuerdo al valor de TypeDiscountGeneral
        /// / Gets or Sets the General Discount amount , this field can be null, or can contain the Discount amount or Discount Percentage according to the TypeDiscountGenreal Field.
        /// </summary>
        /// <value>
        /// Monto del Descuento General / General Discount Account.
        /// </value>
        public decimal? DiscountGeneral { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Descuento General, este campo debe enviarse null si no hay descuento general o con los valores (0 Monto especifico, 1 para % de Descuento) 
        /// / Gets or Sets the General Discount Type , must be sent in null if no General Discount is applied , if not the values are 0 for a specific discount amount or 1 for a percentage discount.
        /// </summary>
        /// <value>
        /// Tipo de Descuento General / General Discount Type.
        /// </value>
        public int? TypeDiscountGeneral { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de días de crédito de la factura, puede ser enviado en null si la factura no es de crédito 
        /// / Gets or Sets the Number of days the Client has to pay the invoice , must be sent in null if the invoice or ticket payment method is not credit.
        /// </summary>
        /// <value>
        /// Número de días de crédito / Number of Credit days.
        /// </value>
        public int? CreditTerm { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha de Vencimiento de Factura.
        /// </summary>
        /// <value>
        /// Fecha de Vencimiento.
        /// </value>
        [JsonIgnore]
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Firma de la factura (Se usa por defecto Llave Criptográfica si se envía en null) / Gets or Sets the Invoice Signature Type (Encrypted key by default if sent null).
        /// </summary>
        /// <value>
        /// Tipo de Firma de la factura / Invoice Signature Type.
        /// </value>
        public FirmType? TipoFirma { get; set; } = FirmType.Llave;

        /// <summary>
        /// Obtiene o Almacena el Tipo de Moneda de la factura (Si se envía null , se usa la moneda por defecto del sub dominio) 
        /// / Gets or Sets the Currency Type of the invoice or ticket (if sent null the Tenant Default Currency is used).
        /// </summary>
        /// <value>
        /// Tipo de Moneda de la factura o tiquete/ Currency Type of the invoice or ticket.
        /// </value>
        public FacturaElectronicaResumenFacturaCodigoMoneda? CodigoMoneda { get; set; }

        /// <summary>
        ///  Obtiene o Almacena el Tipo de documento (1: Factura o 4: Ticket Electrónico) / Document type (Invoice 1 , Ticket 4).
        /// </summary>
        ///  /// <value>
        /// Tipo de documento / Document type
        /// </value>
        [JsonIgnore]
        public TypeDocumentInvoice TypeDocument { get; set; } = TypeDocumentInvoice.Invoice;

        /// <summary>
        /// Obtiene o Almacena el Nombre y apellido del cliente (Cuando se usa el Tiquete Electrónico y no deseamos rellenar todos los datos del Cliente) 
        /// / Gets or Sets the Client Name and Last name (When using ticket if you don't want to create the client).
        /// </summary>
        /// <value>
        /// Nombre completo del cliente / Client Full Name
        /// </value>
        public string ClientName { get; set; }

        /// <summary>
        /// Obtiene o Almacena el tipo de identificación del cliente (Cuando se usa el Tiquete Electrónico y no deseamos rellenar todos los datos del Cliente) 
        /// / Gets or Sets the Client Identification type (When using ticket if you don't want to create the client) .
        /// </summary>
        /// <value>
        /// Tipo de identificación del cliente / Client Identification Type
        /// </value>
        public XSD.IdentificacionTypeTipo? ClientIdentificationType { get; set; }

        /// <summary>
        /// Obtiene o Almacena la identificación del cliente (Cuando se usa el Tiquete Electrónico y no deseamos rellenar todos los datos del Cliente) 
        /// / Gets or Sets the Client Identification number (When using ticket if you don't want to create the client).
        /// </summary>
        /// <value>
        /// Identificación del cliente  / Client Identification Number
        /// </value>
        [StringLength(20)]
        public string ClientIdentification { get; set; }

        /// <summary>
        /// Obtiene o Almacena el número telefónico del cliente (Cuando se usa el Tiquete Electrónico y no deseamos rellenar todos los datos del Cliente) /
        /// Gets or Sets the Client Phone Number (When using ticket if you don't want to create the client).
        /// </summary>
        /// <value>
        /// Número de telefóno fijo del cliente / Client Phone Number
        /// </value>
        public string ClientPhoneNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el número movil del cliente (Cuando se usa el Tiquete Electrónico y no deseamos rellenar todos los datos del Cliente) /
        /// Gets or Sets the Client Cellphone Number (When using ticket if you don't want to create the client).
        /// </summary>
        /// <value>
        /// Número móvil del cliente / Client Cellphone number
        /// </value>
        public string ClientMobilNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena la dirección de correo electrónico del cliente (Cuando se usa el Tiquete Electrónico y no deseamos rellenar todos los datos del Cliente) /
        /// Gets or Sets the Client Email (When using ticket if you don't want to create the client).
        /// </summary>
        /// <value>Correo electrónico del cliente / Client Email
        /// </value>
        public string ClientEmail { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Numero de Referencia Externa (El numero de control de documento de su sistema) / 
        /// Gets or Sets the External Reference number (Your own system document reference number).
        /// </summary>
        /// <value>
        /// Numero de Referencia Externa / External reference number.
        /// </value>
        public string ExternalReferenceNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena si la factura o tiquete es realizado en sustitución de un comprobante provisional / 
        /// Gets or Sets if the Invoice or ticket is a Replacing a temporary document.
        /// </summary>
        /// <value>Indica si la factura o tiquete es realizado en sustitución de un comprobante provisional / <c>true</c> if the Invoice or ticket replaces a temporary document</value>
        public bool IsContingency { get; set; } = false;

        /// <summary>
        /// Obtiene o Almacena el número consecutivo del comprobante provisional / Gets or Sets the Temporary Document consecutive number.
        /// </summary>
        /// <value>Número consecutivo del comprobante provisional / Temporary Document Consecutive number</value>
        public string ConsecutiveNumberContingency { get; set; }

        /// <summary>
        /// Obtiene o Almacena el motivo del comprobante provisional / Gets or Sets the Temporary Document Reason.
        /// </summary>
        /// <value>Motivo del comprobante provisional / Temporary Document Reason</value>
        public string ReasonContingency { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha del comprobante provisional / Gets or Sets the Temporary Document Date.
        /// </summary>
        /// <value>Fecha del comprobante provisional / Temporary Document Date </value>
        public DateTime? DateContingency { get; set; }

        /// <summary>
        /// Obtiene o Almacena las Observaciones Generales de la factura (Nota) de la factura or tiquete / 
        /// Gets or Sets a General observation (Memo) to the invoice or ticket.
        /// </summary>
        /// <value>
        ///  Observaciones Generales de la factura (Nota) / General Observation (Memo).
        /// </value>
        public string GeneralObservation { get; set; }

        /// <exclude />
        public void AddValidationErrors(CustomValidationContext context)
        {

            var totalInvoice = InvoiceLines.Sum(x => x.Total);            

            var subTotalInvoice = InvoiceLines.Sum(x => ((x.Cantidad * x.Precio) - (((x.Cantidad * x.Precio) * x.Descuento) / 100)));

            var totalInvoiceTax = InvoiceLines.Sum(x => x.Impuesto);

            var totalPaymetn = ListPaymentType!=null? ListPaymentType.Sum(x=> x.Balance):0;

            var lineszero = InvoiceLines.Where(x => x.Precio == 0);

            if (lineszero.Count()>0)
                context.Results.Add(new ValidationResult("Existen servicios con monto cero. No se pueden facturar servicios con monto en cero.", new string[] { "InvoiceLines" }));

            if (InvoiceLines.Count==0)
                context.Results.Add(new ValidationResult("Debe ingresar por lo menos un servicio a la factura", new string[] { "InvoiceLines" }));

            if ((ListPaymentType == null) && (CreditTerm == 0))
            {
                context.Results.Add(new ValidationResult("El total pagado debe ser igual al monto de la factura, en caso de ser Credito Creditterm debe ser mayor que 0.", new string[] { "ListPaymetnType" }));
            }

            if ((ListPaymentType != null) && (CreditTerm == 0))
            {
                if (TypeDiscountGeneral > 0)
                {
                    if (TypeDiscountGeneral == 1)
                    {
                        var discount = (subTotalInvoice * DiscountGeneral) / 100;
                        var totalInvoiceWithDiscount = (subTotalInvoice - discount) + totalInvoiceTax;
                        totalInvoiceWithDiscount = Math.Round((Decimal)totalInvoiceWithDiscount, 2);
                        if ((ListPaymentType != null) && (totalPaymetn != totalInvoiceWithDiscount))
                            context.Results.Add(new ValidationResult("El total pagado debe ser igual al monto de la factura.", new string[] { "ListPaymetnType" }));
                    }
                    if (TypeDiscountGeneral == 2)
                    {
                        var totalInvoiceWithDiscount = (subTotalInvoice - DiscountGeneral) + totalInvoiceTax;
                        totalInvoiceWithDiscount = Math.Round((Decimal)totalInvoiceWithDiscount, 2);
                        if ((ListPaymentType != null) && (totalPaymetn != totalInvoiceWithDiscount))
                            context.Results.Add(new ValidationResult("El total pagado debe ser igual al monto de la factura.", new string[] { "ListPaymetnType" }));
                    }
                }
                else
                {
                    if ((ListPaymentType != null) && (totalPaymetn != totalInvoice))
                        context.Results.Add(new ValidationResult("El total pagado debe ser igual al monto de la factura.", new string[] { "ListPaymetnType" }));
                }
            }

            

        }
    }

   
}
