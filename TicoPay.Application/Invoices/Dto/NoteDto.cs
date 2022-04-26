using Abp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.Drawers;
using TicoPay.Invoices.XSD;
using TicoPay.Services.Dto;
using TicoPay.Taxes;
using TicoPay.Taxes.Dto;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Invoices.Dto
{
    /// <summary>
    /// Clase que almacena los datos de la Nota de Crédito o Débito / Contains the Credit or Debit Memo information
    /// </summary>
    [AutoMapFrom(typeof(Note))]
    public class NoteDto
    {
        /// <summary>
        /// Obtiene o Almacena el ID de la Nota (Mandarlo en null cuando se cree la nota) / Gets or Sets the Memo Id (can be sent null when creating the Memo).
        /// </summary>
        /// <value>
        /// ID de la Nota / Memo Id.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID de la factura que afecta la nota / Gets or Sets the Invoice or Ticket Id of the document to be affected.
        /// </summary>
        /// <value>
        /// ID de la factura Afectada / Invoice or Ticket Id to be affected.
        /// </value>
        [Required]
        public Guid? InvoiceId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Consecutivo de la factura afectada / Gets or Sets the Consecutive Number of the Affected invoice or Ticket.
        /// </summary>
        /// <value>
        /// Número de Consecutivo de la factura afectada / Invoice Consecutive number .
        /// </value>
        [Required]
        public string NumberInvoiceRef { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Cliente al que pertenece la nota (Puede ser enviado en null para tiquete) /
        /// Gets or Sets the Client Id of the Invoice to be affected (can be sent null for Tickets).
        /// </summary>
        /// <value>
        /// Id del Cliente al que pertenece la nota / Client Id of the affected Invoice.
        /// </value>
        // [Required] Eliminado para poder hacer notas de Tiquete
        public Guid? ClientId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre del Cliente al que pertenece la nota (Para Tiquete) / Gets or Sets the Client Name in case of Tickets.
        /// </summary>
        /// <value>
        /// Nombre del Cliente al que pertenece la nota / Client Name.
        /// </value>
        public string ClientName { get; set; }


        /// <summary>
        /// Obtiene el Vaucher key o Clave de la nota (Enviar en null al crear la nota) / 
        /// Gets or Sets the Memo Voucher Key (must be Sent in null when creating the Memo).
        /// </summary>
        /// <value>
        /// Vaucher key o Clave de la nota / Memo Voucher key.
        /// </value>
        public string VoucherKey { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha de la nota (Puede enviarse en null al crear la nota) / Gets or Sets the Memo Date (can be sent in null when creating the Memo).
        /// </summary>
        /// <value>
        /// Fecha de la nota / Memo Date.
        /// </value>
        [Display(Name = "Fecha")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto sin impuesto de la Nota / Gets or Sets the Net Total of the Memo.
        /// </summary>
        /// <value>
        /// Monto sin impuesto de la Nota / Net Total.
        /// </value>
        [Required]
        [Display(Name = "Monto")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto del Impuesto de la nota / Gets or Sets the Tax Amount Total.
        /// </summary>
        /// <value>
        /// Monto del Impuesto de la nota / Tax Amount Total.
        /// </value>
        [Required]
        [Display(Name = "Impuesto")]
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto de Descuento de la Nota / Gets or Sets the Discount Total.
        /// </summary>
        /// <value>
        /// Monto de Descuento de la Nota / Discount Total.
        /// </value>
        [Required]
        [Display(Name = "Descuento")]
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto Total de la Nota / Gets or Sets the Memo Total Amount.
        /// </summary>
        /// <value>
        /// Total de la Nota / Memo Total Amount.
        /// </value>
        [Required]
        [Display(Name = "Monto Total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Moneda de la Nota / Gets or Sets the Currency Code.
        /// </summary>
        /// <value>
        /// Moneda de la Nota / Currency Code.
        /// </value>
        [Required]
        public NoteCodigoMoneda CodigoMoneda { get; set; }             

        /// <summary>
        /// Obtiene o Almacena el Tipo de Nota / Gets or Sets the Memo Type.
        /// </summary>
        /// <value>
        /// Tipo de Nota / Memo Type.
        /// </value>
        [Required]
        [Display(Name = "Tipo")]
        public NoteType NoteType { get; set; }

        /// <summary>
        /// Obtiene confirmación de que fue enviada a Hacienda / Gets or Sets if the Invoice was originally sent to Hacienda.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si fue enviada; sino, <c>false</c>.
        /// </value>
        [Display(Name = "Estado")]
        public bool SendInvoice { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estado de la Nota con respecto a Hacienda / Gets or Sets the Status of the Memo According to Hacienda.
        /// </summary>
        /// <value>
        /// Estado de la Nota con respecto a Hacienda / Memo Status According to Hacienda.
        /// </value>
        [Display(Name = "Estado Hacienda")]
        public StatusTaxAdministration StatusTribunet { get; set; }

        /// <summary>
        /// Obtiene confirmación si la nota fue recibida / Gets Confirmation that the Note Email was Received.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si la nota fue recibida; sino, <c>false</c>.
        /// </value>
        [Display(Name = "Estado Recepción")]
        public bool IsNoteReceptionConfirmed { get; set; }


        /// <summary>
        /// Obtiene o Almacena la Razón de la Nota / Gets or Sets the Memo Reason.
        /// </summary>
        /// <value>
        /// Razón de la Nota / Memo Reason.
        /// </value>
        [Required]
        public NoteReason NoteReasons { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción de la razón de la nota (En caso de NoteReasons sea Otros) / Gets or Sets the Memo Description (In Case Memo Reason is Other).
        /// </summary>
        /// <value>
        /// Descripción de la razón de la nota / Memo Other Reason Description.
        /// </value>
        [MaxLength(160)]
        public string NoteReasonsOthers { get; set; }

        /// <exclude />
        [JsonIgnore]
        public IList<SelectListItem> NoteReasonsList {
            get
            {
                //return TicoPay.Application.Helpers.EnumHelper.GetSelectList(typeof(NoteReason));
                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new SelectListItem { Value = NoteReason.Corregir_Monto_Factura.ToString(), Text = TicoPay.Application.Helpers.EnumHelper.GetDescription(NoteReason.Corregir_Monto_Factura) });
                list.Add(new SelectListItem { Value = NoteReason.Reversa_documento.ToString(), Text = TicoPay.Application.Helpers.EnumHelper.GetDescription(NoteReason.Reversa_documento) });
                list.Add(new SelectListItem { Value = NoteReason.Otros.ToString(), Text = TicoPay.Application.Helpers.EnumHelper.GetDescription(NoteReason.Otros) });
                return list;
            }
        }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Firma de la nota / Gets or Sets the Memo Signature Type.
        /// </summary>
        /// <value>
        /// Tipo de Firma de la nota / Memo Signature Type.
        /// </value>
        public FirmType? TipoFirma { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estatus de la Firma Digital / Gets or Sets the Memo Signature Status (Digital Signature).
        /// </summary>
        /// <value>
        /// Estatus de la Firma Digital / Memo Signature Status.
        /// </value>
        public StatusFirmaDigital? StatusFirmaDigital { get; set; }

        /// <summary>
        /// Obtiene o Almacena las Lineas de la Nota / Gets or Sets the Memo Lines.
        /// </summary>
        /// <value>
        /// Lineas de la Nota / Memo Lines.
        /// </value>
        [Required]
        public virtual IList<NoteLineDto> NotesLines { get; set; }

        /// <summary>
        /// Obtiene confirmación que la nota fue validada con Hacienda / Gets or Sets Confirmation if the Memo was sent to Hacienda.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si se valido con hacienda ; sino, <c>false</c>.
        /// </value>
        public bool ValidateHacienda { get; set; }


        /// <summary>
        /// Obtiene o Almacena el Estatus de la Nota / Gets or Sets the Memo Status.
        /// </summary>
        /// <value>
        /// Estatus Nota / Memo Status.
        /// </value>
        public Status Status { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha de vencimiento de la Nota / Gets or Sets the Memo Expiration Date.
        /// </summary>
        /// <value>
        /// Fecha de Vencimiento de la Nota / Memo Expiration Date.
        /// </value>
        [Display(Name = "Fecha vencimiento")]
        public DateTime DueDate { get; set; }
        
        /// <summary>
        /// Obtiene o Almacena los días de crédito de la nota (Condición de venta a crédito) / Gets or Sets the Memo Credit Days.
        /// </summary>
        /// <value>
        /// Días de crédito de la nota / Memo Credit Days.
        /// </value>
        public int CreditTerm { get; set; }


        /// <summary>
        /// Obtiene o Almacena la condición de venta de la nota (Crédito o Contado) / Gets or Sets the Condition of Sale Type.
        /// </summary>
        /// <value>
        /// Condición de venta de la nota / Condition of Sale Type.
        /// </value>
        public FacturaElectronicaCondicionVenta ConditionSaleType { get; set; }

        /// <summary>
        /// Obtiene o Almacena si la factura o tiquete es realizado en sustitución de un comprobante provisional / Gets or Sets if the Memo is replacing a Temporary Document.
        /// </summary>
        /// <value>Indica si la factura o tiquete es realizado en sustitución de un comprobante provisional</value>
        public bool IsContingency { get; set; } = false;

        /// <summary>
        /// Obtiene o Almacena el número consecutivo del comprobante provisional / Gets or Sets the Temporary Document Consecutive Number.
        /// </summary>
        /// <value>Número consecutivo del comprobante provisional / Temporary Document Consecutive number</value>        
        [MaxLength(50)]
        public string ConsecutiveNumberContingency { get; set; }

        /// <summary>
        /// Obtiene o Almacena el motivo del comprobante provisional / Gets or Sets the Replace Reason.
        /// </summary>
        /// <value>Motivo del comprobante provisional / Replace document reason </value>        
        [MaxLength(180)]
        public string ReasonContingency { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha del comprobante provisional / Gets or Sets the Temporary document Date.
        /// </summary>
        /// <value>Fecha del comprobante provisional / Temporary Document Date</value>
        public DateTime? DateContingency { get; set; }

        [JsonIgnore]
        public IList<SelectListItem> FirmTypes
        {
            get; set;
            //get
            //{
            //    return TicoPay.Application.Helpers.EnumHelper.GetSelectList(typeof(FirmType));
            //}
        }

        /// <exclude />
        [JsonIgnore]
        public IList<SelectListItem> CoinType {
            get
            {
                // var list = EnumHelper.GetSelectListValues(typeof(FacturaElectronicaResumenFacturaCodigoMoneda));

                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new SelectListItem { Value = FacturaElectronicaResumenFacturaCodigoMoneda.CRC.ToString(), Text = FacturaElectronicaResumenFacturaCodigoMoneda.CRC.ToString() });
                list.Add(new SelectListItem { Value = FacturaElectronicaResumenFacturaCodigoMoneda.USD.ToString(), Text = FacturaElectronicaResumenFacturaCodigoMoneda.USD.ToString() });
                return list;
            }
        }

        /// <summary>
        /// Obtiene o Almacena los datos del documento asociado a la nota.
        /// </summary>
        /// <value>
        /// Datos del documento asociado a la nota.
        /// </value>    
        [JsonIgnore]
        public Document DocumentRef { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int? ErrorCode { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string ErrorDescription { get; set; }

        /// <exclude />
        [JsonIgnore]
        public Drawer Drawer { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Numero de Referencia Externa (Usuarios del Api) / Gets or Sets the External Reference number (Your system reference number).
        /// </summary>
        /// <value>
        /// Numero de Referencia Externa (Usuarios del Api) / External Reference number.
        /// </value>
        public string ExternalReferenceNumber { get; set; }


        /// <exclude />
        [JsonIgnore]
        public int IsPos { get; set; }
    }

    /// <summary>
    /// Clase que almacena los datos de la Linea de una Nota / Contains the Memo Line Information
    /// </summary>
     [AutoMapFrom(typeof(NoteLine))]
    public class NoteLineDto
    {
        /// <exclude />
        [JsonIgnore]
        public const int cantidadDecimal = MultiTenancy.Tenant.quantityDecimal;

        /// <summary>
        /// Obtiene o Almacena el ID del Tenant al que pertenece la nota / Gets or Sets the Tenant Id to which the Note belongs.
        /// </summary>
        /// <value>
        /// ID del Tenant al que pertenece la nota / Tenant id.
        /// </value>
        public int TenantId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Precio por Unidad / Gets or Sets the Price per Unit.
        /// </summary>
        /// <value>
        /// Precio por Unidad / Price per Unit.
        /// </value>
        [Required]
        public decimal PricePerUnit { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el Monto del Impuesto de la linea / Gets or Sets the Line Tax Amount.
        /// </summary>
        /// <value>
        /// Monto del Impuesto / Tax Amount.
        /// </value>
        [Required]
        public decimal TaxAmount { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el Total de la Linea / Gets or Sets the Line Total.
        /// </summary>
        /// <value>
        /// Total de la Linea / Line Total.
        /// </value>
        [Required]
        public decimal Total { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el Porcentaje de Descuento de la linea / Gets or Sets the Discount Percentage of the Line.
        /// </summary>
        /// <value>
        /// Porcentaje de Descuento de la linea / Line Discount Percentage.
        /// </value>
        [Required]
        public decimal DiscountPercentage { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el Detalle o Nota de la linea / Gets or Sets Line Detail note.
        /// </summary>
        /// <value>
        /// Detalle o Nota de la linea / Line Detail note.
        /// </value>
        [MaxLength(200)]
        public string Note { get;  set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción de la linea / Gets or Sets the Line Description .
        /// </summary>
        /// <value>
        /// Descripción de la linea / Line Description.
        /// </value>
        [Required]
        [MaxLength(160)]
        public string Title { get;  set; }

        /// <summary>
        /// Obtiene o Almacena la Cantidad / Gets or Sets the Item Quantity.
        /// </summary>
        /// <value>
        /// Cantidad / Quantity.
        /// </value>
        [Required]
        public decimal Quantity { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el Id de la Nota a la que pertenece la linea (debe enviarse en null) / Gets or Sets the Memo Id (must be sent in null).
        /// </summary>
        /// <value>
        /// Id de la Nota a la que pertenece la linea / Memo Id.
        /// </value>
        public Guid NoteId { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Linea / Gets or Sets the Line Type.
        /// </summary>
        /// <value>
        /// Tipo de Linea / Line Type.
        /// </value>
        [Required]
        public LineType LineType { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Servicio (Puede ser enviado en null) / Gets or Sets the Service Id (Can be Sent in null).
        /// </summary>
        /// <value>
        /// Id del Servicio / Service Id.
        /// </value>
        public Guid? ServiceId { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el Id de Producto (Puede ser enviado en null) / Gets or Sets the Product Id (Can be sent in null).
        /// </summary>
        /// <value>
        /// Id de Producto.
        /// </value>
        public Guid? ProductId { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Linea / Gets or Sets the Line number.
        /// </summary>
        /// <value>
        /// Número de Linea / Line Number.
        /// </value>
        public int LineNumber { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el tipo de producto o servicio / Gets or sets the Product or Service Type.
        /// </summary>
        /// <value>
        /// Tipo de Producto / Product Type.
        /// </value>
        public CodigoTypeTipo CodeTypes { get;  set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción del Descuento / Gets or Sets the Discount Description.
        /// </summary>
        /// <value>
        /// Descripción del Descuento / Discount Description.
        /// </value>
        [MaxLength(80, ErrorMessage = "La descripción del descuento no puede tener más de 80 caracteres.")]
        public string DescriptionDiscount { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el Sub Total de la linea / Gets or Sets the Line Net Total.
        /// </summary>
        /// <value>
        /// Sub Total de la linea / Line Net Total.
        /// </value>
        [Required]
        public decimal SubTotal { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el Total de la Linea / Gets or Sets the Line Total.
        /// </summary>
        /// <value>
        /// Total de la Linea / Line Total.
        /// </value>
        [Required]
        public decimal LineTotal { get;  set; }

        /// <exclude />
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        /// <exclude />
        [JsonIgnore]
        public long? DeleterUserId { get; set; }

        /// <exclude />
        [JsonIgnore]
        public DateTime? DeletionTime { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID de Exsoneración (Puede ser enviado en null) / Gets or Sets the Exoneration Id (Can be sent in null).
        /// </summary>
        /// <value>
        /// ID de Exsoneración.
        /// </value>
        public Guid? ExonerationId { get;  set; }

        /// <summary>
        /// Obtiene o Almacena el Servicio de la linea / Gets or Sets the Service .
        /// </summary>
        /// <value>
        /// Servicio de la linea / Service.
        /// </value>
        public ServiceDto Service { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Impuesto de la linea / Gets or Sets the Line Tax.
        /// </summary>
        /// <value>
        /// Impuesto de la linea / Line Tax.
        /// </value>
        public virtual TaxDto Tax { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Impuesto de la linea / Gets or Sets the Tax Id.
        /// </summary>
        /// <value>
        /// Id del Impuesto / Tax Id.
        /// </value>
        [Required]
        public Guid? TaxId { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Unidad de Medida / Gets or Sets the Measurement Unit.
        /// </summary>
        /// <value>
        /// Unidad de Medida / Measurement Unit.
        /// </value>
        [Required]
        public UnidadMedidaType? UnitMeasurement { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción de la Unidad de Medida Otros / Gets or Sets the Measurement unit description if other
        /// </summary>
        /// <value>
        /// Descripción de la Unidad de Medida Otros / Measurement unit description if other.
        /// </value>
        public string UnitMeasurementOthers { get; set; }
        
    }

    /// <summary>
    /// Contiene la información del document / Contains the Document information
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Id de la factura o Nota a aplicar / Id of the Invoice or Memo to Apply
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tipo de documento de referencia  / Reference Document Type
        /// /// </summary>
        public string TypeDocument { get; set; }

        /// <summary>
        /// Código del tipo de documento de referencia / Document Type Reference
        /// </summary>
        public int TypeDocumentCodigo { get; set; }

        /// <summary>
        /// Número del consecutivo del documento de referencia 
        /// </summary>
        public string ConsecutiveNumber { get; set; }

        /// <summary>
        /// Indica si la nota reversa totalmente el documento o no
        /// </summary>
        public bool IsReverseTotal { get; set; }
    }
}
