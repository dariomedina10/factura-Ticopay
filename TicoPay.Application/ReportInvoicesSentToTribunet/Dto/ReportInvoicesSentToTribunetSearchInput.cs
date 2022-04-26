using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;

namespace TicoPay.ReportInvoicesSentToTribunet.Dto
{
    /// <summary>
    /// Clase que define los parámetros de búsqueda para el reporte de facturas de hacienda / Defines the Search Parameters for the report
    /// </summary>
    public class ReportInvoicesSentToTribunetSearchInput
    {
        /// <summary>
        /// Obtiene o Almacena el Nombre de Cliente / Gets or Sets the Client Name.
        /// </summary>
        /// <value>
        /// Nombre de Cliente.
        /// </value>
        [Display(Name = "Cliente")]
        public string NombreCliente { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de Identificación del cliente de las facturas a buscar / Gets or Sets the Client Identification number.
        /// </summary>
        /// <value>
        ///  Número de Identificación del cliente / Client Identification number.
        /// </value>
        public string CedulaCliente { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id de Cliente de las facturas a buscar / Gets or Sets the Client Id.
        /// </summary>
        /// <value>
        /// Id de Cliente de las facturas a buscar / Client Id.
        /// </value>
        public Guid? ClienteId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número consecutivo de la factura a buscar / Gets or Sets the Consecutive number of the invoice.
        /// </summary>
        /// <value>
        /// Número consecutivo de la factura / Consecutive number.
        /// </value>
        [Display(Name = "Número")]
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Medio de Pago de las facturas a buscar / Gets or Sets Payment type of the invoices to search.
        /// </summary>
        /// <value>
        /// Medio de Pago de las facturas.
        /// </value>
        [Display(Name = "Medio Pago")]
        public PaymetnMethodType? MedioPago { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Condición de venta del Documento / Gets or Sets the Sales Condition.
        /// </summary>
        /// <value>
        /// Condición de venta del Documento.
        /// </value>
        [Display(Name = "Condición Venta")]
        public FacturaElectronicaCondicionVenta? CondicionVenta { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha emisión desde la cual buscar / Gets or Sets the minimum Issue Date of the invoice.
        /// </summary>
        /// <value>
        /// Fecha emisión desde.
        /// </value>
        [Display(Name = "Fecha Emisión (Désde)")]
        public DateTime? FechaEmisionDesde { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha emisión hasta la cual buscar / Gets or Sets the maximum Issue Date of the invoice.
        /// </summary>
        /// <value>
        /// Fecha emisión hasta.
        /// </value>
        [Display(Name = "Fecha Emisión (Hásta)")]
        public DateTime? FechaEmisionHasta { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estado según Hacienda de la factura a buscar / Gets or Sets the Hacienda status of the invoice.
        /// </summary>
        /// <value>
        /// Estado según Hacienda de la factura.
        /// </value>
        [Display(Name = "Estado Hacienda")]
        public StatusTaxAdministration? StatusTribunet { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Indicador para buscar facturas que fueron recibidas / Gets or Sets if the Invoice was acknowledge as received by the client .
        /// </summary>
        /// <value>
        /// Indicador de si la factura fue recibida / Invoice was acknowledge as received by the client.
        /// </value>
        [Display(Name = "Acuse Recibo")]
        public bool? RecepcionConfirmada { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estatus de Factura según Ticopay a buscar / Gets or Sets the Invoice Status.
        /// </summary>
        /// <value>
        /// Estatus de Factura según Ticopay.
        /// </value>
        [Display(Name = "Estado")]
        public Status? Status { get; set; }

        /// <exclude />
        [JsonIgnore]
        public List<ReportInvoicesSentToTribunetDto> Invoices { get; set; }

        public TypeDocumentInvoice? TypeDocument { get; set; }
        
        /// <exclude />
        [JsonIgnore]
        public IList<SelectListItem> ListTypeDocument
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(TypeDocumentInvoice.Invoice, TypeDocumentInvoice.Ticket);
            }
        }

        /// <exclude />
        [JsonIgnore]
        public IList<SelectListItem> StatusesInvoice
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(TicoPay.Invoices.Status.Completed, TicoPay.Invoices.Status.Parked, TicoPay.Invoices.Status.Voided);
            }
        }

        /// <exclude />
        [JsonIgnore]
        public IList<SelectListItem> StatusesTribunet
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(StatusTaxAdministration.Recibido, StatusTaxAdministration.Procesando, StatusTaxAdministration.Aceptado, StatusTaxAdministration.Rechazado, StatusTaxAdministration.Error);
            }
        }
    }
}
