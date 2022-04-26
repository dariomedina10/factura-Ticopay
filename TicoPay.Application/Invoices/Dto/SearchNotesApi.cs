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
    /// Clase que contiene los Parámetros para la búsqueda de Notas / Contains the Memo Search Parameters
    /// </summary>
    public class SearchNotesApi
    {

        /// <summary>
        /// Obtiene o Almacena la Fecha de Inicio de Búsqueda / Gets or Sets the Start Search Date.
        /// </summary>
        /// <value>
        /// Fecha de Inicio de Búsqueda / Start Search Date.
        /// </value>
        [Display(Name = "Fecha Inicio")]
        public DateTime? StartDueDate { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha final de Búsqueda / Gets or Sets the End Search Date.
        /// </summary>
        /// <value>
        /// Fecha final de Búsqueda / End Search Date.
        /// </value>
        [Display(Name = "Fecha Fin")]
        public DateTime? EndDueDate { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id de la Factura a la que pertenece la nota / Gets or Sets the Invoice or Ticket Id to Which the Memo belongs.
        /// </summary>
        /// <value>
        /// Id de la Factura a la que pertenece la nota / Invoice or Ticket Id to which the Memo belongs.
        /// </value>
        public Guid? InvoiceId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id de la Nota especifica a buscar / Gets or Sets the Memo Id.
        /// </summary>
        /// <value>
        /// Id de la Nota especifica a buscar / Memo Id.
        /// </value>
        public Guid? NoteId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estatus de la Nota según Hacienda / Gets or Sets the Memo Status according to Hacienda.
        /// </summary>
        /// <value>
        /// Estatus de la Nota según Hacienda / Memo Status according to Hacienda.
        /// </value>
        [Display(Name = "Estatus")]
        [Range(0, 5, ErrorMessage = "El estatus de la nota debe estar entre 0 y 5")]
        public StatusTaxAdministration? Status { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int? Page { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int? PageSize { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estatus de la Firma Digital de las Notas a buscar / Gets or Sets the Signature Status.
        /// </summary>
        /// <value>
        /// Estatus de la Firma Digital / Signature Status.
        /// </value>
        public StatusFirmaDigital? EstatusFirma { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Firma de la Notas a buscar / Gets or Sets the Signature Type.
        /// </summary>
        /// <value>
        /// Tipo de Firma / Signature Type.
        /// </value>
        public FirmType? TipoFirma { get; set; }
    }
}
