using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Invoices;
using TicoPayDll.Notes;
using TicoPayDll.Services;
using TicoPayDll.Taxes;

namespace TicopayUniversalConnectorService.ConexionesExternas.QuickBooks.Dto
{
    public class QuickbooksNote
    {
        public string CreditMemoId { get; set; }

        public string AffectedInvoiceId { get; set; }

        public string QbClientId { get; set; }

        public string EditSequence { get; set; }

        public string NumeroReferencia { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID de la Nota.
        /// </summary>
        /// <value>
        /// ID de la Nota.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID de la factura que afecta la nota.
        /// </summary>
        /// <value>
        /// ID de la factura Afectada.
        /// </value>
        public Guid InvoiceId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Referencia de la factura afectada.
        /// </summary>
        /// <value>
        /// Número de Referencia de la factura afectada.
        /// </value>
        public string NumberInvoiceRef { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Cliente al que pertenece la nota.
        /// </summary>
        /// <value>
        /// Id del Cliente al que pertenece la nota.
        /// </value>
        public Guid? ClientId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre del Cliente al que pertenece la nota.
        /// </summary>
        /// <value>
        /// Nombre del Cliente al que pertenece la nota.
        /// </value>
        public string ClientName { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha de la nota.
        /// </summary>
        /// <value>
        /// Fecha de la nota.
        /// </value>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto sin impuesto de la Nota.
        /// </summary>
        /// <value>
        /// Monto sin impuesto de la Nota.
        /// </value>
        public decimal Amount { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto del Impuesto de la nota.
        /// </summary>
        /// <value>
        /// Monto del Impuesto de la nota.
        /// </value>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto de Descuento de la Nota.
        /// </summary>
        /// <value>
        /// Monto de Descuento de la Nota.
        /// </value>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto Total de la Nota.
        /// </summary>
        /// <value>
        /// Total de la Nota.
        /// </value>
        public decimal Total { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Moneda de la Nota.
        /// </summary>
        /// <value>
        /// Moneda de la Nota.
        /// </value>
        public CodigoMoneda CodigoMoneda { get; set; }

        /// <summary>
        /// Obtiene en Número Consecutivo de la nota (Asignado por Ticopay).
        /// </summary>
        /// <value>
        /// Número Consecutivo de la nota.
        /// </value>
        public string ConsecutiveNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Nota.
        /// </summary>
        /// <value>
        /// Tipo de Nota.
        /// </value>
        public NoteType NoteType { get; set; }

        /// <summary>
        /// Obtiene confirmación de que fue enviada a Hacienda.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si fue enviada; sino, <c>false</c>.
        /// </value>
        public bool SendInvoice { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estado de la Nota con respecto a Hacienda.
        /// </summary>
        /// <value>
        /// Estado de la Nota con respecto a Hacienda.
        /// </value>
        public StatusTaxAdministration StatusTribunet { get; set; }

        /// <summary>
        /// Obtiene confirmación si la nota fue recibida.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si la nota fue recibida; sino, <c>false</c>.
        /// </value>
        public bool IsNoteReceptionConfirmed { get; set; }


        /// <summary>
        /// Obtiene o Almacena la Razón de la Nota.
        /// </summary>
        /// <value>
        /// Razón de la Nota.
        /// </value>
        public NoteReason NoteReasons { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción de la razón de la nota (En caso de NoteReasons sea Otros).
        /// </summary>
        /// <value>
        /// Descripción de la razón de la nota.
        /// </value>
        public string NoteReasonsOthers { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Firma de la nota.
        /// </summary>
        /// <value>
        /// Tipo de Firma de la nota.
        /// </value>
        public FirmType? TipoFirma { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estatus de la Firma Digital.
        /// </summary>
        /// <value>
        /// Estatus de la Firma Digital.
        /// </value>
        public StatusFirmaDigital? StatusFirmaDigital { get; set; }

        /// <summary>
        /// Obtiene o Almacena las Lineas de la Nota.
        /// </summary>
        /// <value>
        /// Lineas de la Nota.
        /// </value>
        public virtual IList<QuickbooksNoteLine> NotesLines { get; set; }

        /// <summary>
        /// Obtiene confirmación que la nota fue validada con Hacienda.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si se valido con hacienda ; sino, <c>false</c>.
        /// </value>
        public bool ValidateHacienda { get; set; }


        /// <summary>
        /// Obtiene o Almacena el Estatus de la Nota.
        /// </summary>
        /// <value>
        /// Estatus Nota.
        /// </value>
        public Status Status { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha de vencimiento de la Nota.
        /// </summary>
        /// <value>
        /// Estatus Nota.
        /// </value>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Obtiene o Almacena los días de crédito de la nota (Condición de venta a crédito).
        /// </summary>
        /// <value>
        /// Días de crédito de la nota.
        /// </value>
        public int CreditTerm { get; set; }


        /// <summary>
        /// Obtiene o Almacena la condición de venta de la nota (Crédito o Contado).
        /// </summary>
        /// <value>
        /// Condición de venta de la nota.
        /// </value>
        public FacturaElectronicaCondicionVenta ConditionSaleType { get; set; }
    }

    public class QuickbooksNoteLine
    {
        public decimal TaxRate { get; set; }
        /// <summary>
        /// Obtiene o Almacena el ID del Tenant al que pertenece la nota.
        /// </summary>
        /// <value>
        /// ID del Tenant al que pertenece la nota.
        /// </value>
        public int TenantId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Precio por Unidad.
        /// </summary>
        /// <value>
        /// Precio por Unidad.
        /// </value>
        public decimal PricePerUnit { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto del Impuesto de la linea.
        /// </summary>
        /// <value>
        /// Monto del Impuesto.
        /// </value>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Total de la Linea.
        /// </summary>
        /// <value>
        /// Total de la Linea.
        /// </value>
        public decimal Total { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Porcentaje de Descuento de la linea.
        /// </summary>
        /// <value>
        /// Porcentaje de Descuento de la linea.
        /// </value>
        public decimal DiscountPercentage { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Detalle o Nota de la linea.
        /// </summary>
        /// <value>
        /// Detalle o Nota de la linea.
        /// </value>
        public string Note { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción de la linea.
        /// </summary>
        /// <value>
        /// Descripción de la linea.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Cantidad.
        /// </summary>
        /// <value>
        /// Cantidad.
        /// </value>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id de la Nota a la que pertenece la linea.
        /// </summary>
        /// <value>
        /// Id de la Nota a la que pertenece la linea.
        /// </value>
        public Guid NoteId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Linea.
        /// </summary>
        /// <value>
        /// Tipo de Linea.
        /// </value>
        public LineType LineType { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Servicio (En caso de ser Tipo Servicio).
        /// </summary>
        /// <value>
        /// Id del Servicio.
        /// </value>
        public Guid? ServiceId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id de Producto (En caso de ser Tipo Producto).
        /// </summary>
        /// <value>
        /// Id de Producto.
        /// </value>
        public Guid? ProductId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Linea.
        /// </summary>
        /// <value>
        /// Número de Linea.
        /// </value>
        public int LineNumber { get; set; }

        public CodigoTypeTipo CodeTypes { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción del Descuento.
        /// </summary>
        /// <value>
        /// Descripción del Descuento.
        /// </value>
        public string DescriptionDiscount { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Sub Total de la linea.
        /// </summary>
        /// <value>
        /// Sub Total de la linea.
        /// </value>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Total de la Linea.
        /// </summary>
        /// <value>
        /// Total de la Linea.
        /// </value>
        public decimal LineTotal { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID de Exsoneración.
        /// </summary>
        /// <value>
        /// ID de Exsoneración.
        /// </value>
        public Guid? ExonerationId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Servicio de la linea.
        /// </summary>
        /// <value>
        /// Servicio de la linea.
        /// </value>
        public Service Service { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Impuesto de la linea.
        /// </summary>
        /// <value>
        /// Impuesto de la linea.
        /// </value>
        public virtual Tax Tax { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Impuesto de la linea.
        /// </summary>
        /// <value>
        /// Id del Impuesto.
        /// </value>
        public Guid? TaxId { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Unidad de Medida.
        /// </summary>
        /// <value>
        /// Unidad de Medida.
        /// </value>
        public UnidadMedidaType? UnitMeasurement { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción de la Unidad de Medida Otros
        /// </summary>
        /// <value>
        /// Descripción de la Unidad de Medida Otros.
        /// </value>
        public string UnitMeasurementOthers { get; set; }

        public string ItemId { get; set; }

        public TipoItem ItemType { get; set; }
    }
}
