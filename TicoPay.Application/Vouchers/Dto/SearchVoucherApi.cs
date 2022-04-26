using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Vouchers.Dto
{
    /// <summary>
    /// Clase utilizada para definir los parámetros de búsqueda de los comprobantes electrónicos / Contains the parameters for the Voucher Search
    /// </summary>
    public class SearchVoucherApi
    {
        /// <summary>
        /// Obtiene o Almacena el número de identificación del Emisor del Documento especifico a buscar / Gets or Sets the Sender Identification number.
        /// </summary>
        /// <value>
        /// Número de Identificación del Emisor / Sender Identification number.
        /// </value>
        public string Identification { get; set; }

        /// <summary>
        /// Obtiene o Almacena el número consecutivo de Factura especifica a buscar / Gets or Sets the Invoice Consecutive number.
        /// </summary>
        /// <value>
        /// Número consecutivo de Factura / Invoice Consecutive number.
        /// </value>
        public string ConsecutiveNumberInvoice { get; set; }

        /// <summary>
        /// Obtiene o Almacena el número consecutivo del comprobante especifico a buscar / Gets or Sets the Voucher Consecutive number.
        /// </summary>
        /// <value>
        /// Número consecutivo del Comprobante / Voucher Consecutive number.
        /// </value>
        public string ConsecutiveNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha a partir de la cual se buscaran los comprobantes / Gets or Sets the Search start Date.
        /// </summary>
        /// <value>
        /// Fecha de inicio / Search Start Date.
        /// </value>
        public DateTime? StartDueDate { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha hasta donde se buscaran los comprobantes / Gets or Sets the Search End Date.
        /// </summary>
        /// <value>
        /// Fecha fin / Search End Date.
        /// </value>
        public DateTime? EndDueDate { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre de un Emisor especifico para la búsqueda de comprobantes / Gets or Sets The Sender Name.
        /// </summary>
        /// <value>
        /// Nombre del Emisor / Sender Name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena el estatus con respecto a Hacienda de los comprobantes a buscar / Gets or Sets the Voucher Status according to Hacienda.
        /// </summary>
        /// <value>
        /// Estatus del comprobante con respecto a Hacienda / Voucher Status according to Hacienda.
        /// </value>
        public StatusTaxAdministration? StatusTribunet { get; set; }

        /// <summary>
        /// Almacena o Obtiene el Tipo de firma utilizada para el comprobante / Gets or Sets the Signature Type of the Vouchers.
        /// </summary>
        /// <value>
        /// Tipo de firma / Signature Type.
        /// </value>
        public FirmType? TipoFirma { get; set; }

        /// <summary>
        /// Obtiene o Almacena el estatus de la firma digital de los Comprobantes a buscar / Gets or Sets the Digital Signature Status.
        /// </summary>
        /// <value>
        /// Estatus firma digital / Digital Signature Status.
        /// </value>
        public StatusFirmaDigital? StatusFirmaDigital { get; set; }
    }
}
