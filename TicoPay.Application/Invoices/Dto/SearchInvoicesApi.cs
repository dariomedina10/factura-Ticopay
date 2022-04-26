using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Invoices.Dto
{
    /// <summary>
    /// Clase que contiene los parámetros utilizados para la búsqueda de facturas / Contains the Search Parameters for the Invoice or ticket Search
    /// </summary>
    public class SearchInvoicesApi
    {

        /// <summary>
        /// Obtiene o Almacena la Fecha de Inicio de Búsqueda / Gets or Sets the Start Search Date.
        /// </summary>
        /// <value>
        /// Fecha de Inicio / Start Search Date.
        /// </value>
        [Display(Name = "Fecha Inicio")]
        public DateTime? StartDueDate { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha Tope de Búsqueda / Gets or Sets the End Search Date.
        /// </summary>
        /// <value>
        /// Fecha Tope / End Search Date.
        /// </value>
        [Display(Name = "Fecha Fin")]
        public DateTime? EndDueDate { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Cliente (Buscar facturas de un cliente Especifico) / 
        /// Gets or Sets the Client Id (To search invoices or ticket of a Client).
        /// </summary>
        /// <value>
        /// Id del Cliente / Client Id.
        /// </value>
        public Guid? ClientId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Numero de Referencia Externo (Buscar facturas por numero de referencia Externo) / 
        /// Gets or Sets the External Reference (To Search Invoices or tickets by your external Reference).
        /// </summary>
        /// <value>
        /// Numero de Referencia Externo / External Reference Number.
        /// </value>
        public string ExternalReferenceNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID de Factura (Buscar un factura especifica) / 
        /// Gets or Sets the Invoice or Ticket Id (To Search a Specific Invoice or ticket).
        /// </summary>
        /// <value>
        /// ID de Factura or Tiquete / Invoice or Ticket Id.
        /// </value>
        public Guid? InvoiceId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estatus de Factura (Buscar facturas por estatus) / Gets or Sets the Invoice or Ticket Status.
        /// </summary>
        /// <value>
        /// Estatus de Factura o Tiquete / Invoice or Ticket Status.
        /// </value>
        [Display(Name = "Estatus")]
        [Range(0, 5, ErrorMessage = "El estatus de la factura debe estar entre 0 y 5")]
        public Status? Status { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int? Page { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int? PageSize { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estatus de Firma (Buscar facturas por estatus de firma) / Gets or Sets the Signature Status.
        /// </summary>
        /// <value>
        /// Estatus de Firma / Signature Status.
        /// </value>
        public StatusFirmaDigital? EstatusFirma { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Firma (Buscar facturas por Tipo de Firma) / Gets or Sets the Signature Type.
        /// </summary>
        /// <value>
        /// Tipo de Firma / Signature Type.
        /// </value>
        public FirmType? TipoFirma { get; set; }

        /// <summary>
        /// Si la busqueda se origina de un POS retorna solo los datos basicos para la impresion / Get basic data for POS printing
        /// </summary>
        /// <value>
        /// Busqueda por POS / POS Search.
        /// </value>
        public bool? IsPOSSearch { get; set; }
    }
}
