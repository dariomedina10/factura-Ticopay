using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Vouchers.Dto
{
    /// <summary>
    /// Clase que contiene los datos del comprobante electrónico / Contains the Electronic Voucher information
    /// </summary>
    public class VoucherDtoApi
    {
        /// <summary>
        /// Obtiene o Almacena el Identificador único del Comprobante / Gets the Electronic Voucher Id.
        /// </summary>
        /// <value>
        /// ID del Comprobante / Electronic Voucher Id.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Número de identificación del Emisor del documento / Gets Sender Identification number.
        /// </summary>
        /// <value>
        /// Número de Identificación del Emisor / Sender Identification number.
        /// </value>
        public string IdentificationSender { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre del emisor del documento / Gets the Sender Name.
        /// </summary>
        /// <value>
        /// Nombre del Emisor / Sender Name.
        /// </value>
        public string NameSender { get; set; }

        /// <summary>
        /// Obtiene o Almacena el correo electrónico del Emisor / Gets the Sender Email.
        /// </summary>
        /// <value>
        /// Correo electrónico del Emisor / Sender Email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre del Receptor / Gets the Receiver Name.
        /// </summary>
        /// <value>
        /// Nombre del Receptor / Receiver Name.
        /// </value>
        public string NameReceiver { get; set; }

        /// <summary>
        /// Obtiene o Almacena el número de identificación del Receptor / Gets the Receiver Identification number.
        /// </summary>
        /// <value>
        /// Número de identificación del Receptor / Receiver Identification number.
        /// </value>
        public string IdentificationReceiver { get; set; }

        /// <summary>
        /// Obtiene o Almacena el número de consecutivo de la factura / Gets the Invoice Consecutive number.
        /// </summary>
        /// <value>
        /// Número de consecutivo de la factura / Invoice Consecutive number.
        /// </value>
        public string ConsecutiveNumberInvoice { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha de la factura / Gets the Invoice Date.
        /// </summary>
        /// <value>
        /// Fecha de Factura / Invoice Date.
        /// </value>
        public DateTime DateInvoice { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Moneda utilizada en la Operación / Gets the Currency used in the operation.
        /// </summary>
        /// <value>
        /// Tipo de Moneda / Currency used.
        /// </value>
        public FacturaElectronicaResumenFacturaCodigoMoneda Coin { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Monto total del Documento / Gets the Document Total Amount.
        /// </summary>
        /// <value>
        /// Monto Total del Documento / Document Total Amount.
        /// </value>
        public decimal Totalinvoice { get; set; }

        /// <summary>
        /// Obtiene o Almacena el monto total del Impuesto del Documento / Gets the Document Total Tax Amount.
        /// </summary>
        /// <value>
        /// The total tax / Document Total Tax Amount.
        /// </value>
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Obtiene o Almacena la llave interna del comprobante / Gets the Document Voucher Key.
        /// </summary>
        /// <value>
        /// Llave interna del comprobante / Document Voucher Key.
        /// </value>
        public string VoucherKey { get; set; }

        /// <summary>
        /// Obtiene o Almacena la llave interna del documento / Gets the Voucher Key Reference.
        /// </summary>
        /// <value>
        /// Llave interna del documento / Voucher Key Reference.
        /// </value>
        public string VoucherKeyRef { get; set; }

        /// <summary>
        /// Obtiene o Almacena el número consecutivo del comprobante electrónico / Gets Voucher Consecutive number.
        /// </summary>
        /// <value>
        /// Número consecutivo del comprobante electrónico / Voucher Consecutive number.
        /// </value>
        public string ConsecutiveNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena la respuesta al documento a firmar / Gets the Document Response.
        /// </summary>
        /// <value>
        /// Respuesta al documento a firmar / Document Response.
        /// </value>
        public MessageVoucher Message { get; set; }

        /// <summary>
        /// Obtiene o Almacena el detalle de la respuesta al documento / Gets the Document Response Detail.
        /// </summary>
        /// <value>
        /// Detalle de la respuesta al documento / Document Response Detail.
        /// </value>
        public string DetailsMessage { get; set; }

        /// <summary>
        /// Obtiene o Almacena el XML Firmado del comprobante electrónico / Gets the Voucher Signed XML.
        /// </summary>
        /// <value>
        /// XML Firmado del comprobante electrónico / Voucher Signed XML.
        /// </value>
        public string ElectronicBill { get; set; }

        /// <summary>
        /// Obtiene o Almacena si el comprobante fue enviado a hacienda / Gets if the Voucher was send to Hacienda.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si el comprobante fue enviado a Hacienda; sino, <c>false</c> / <c>true</c> if the Voucher was send to Hacienda otherwise <c>false</c>.
        /// </value>
        public bool SendVoucher { get; set; }

        /// <summary>
        /// Obtiene o Almacena el estado del comprobante según Hacienda / Gets the Voucher Status.
        /// </summary>
        /// <value>
        /// Estado del comprobante según Hacienda / Voucher Status.
        /// </value>
        public StatusTaxAdministration StatusTribunet { get; set; }

        /// <summary>
        /// Obtiene o Almacena el tipo de firma del comprobante electrónico / Gets the voucher Signature Type.
        /// </summary>
        /// <value>
        /// Tipo de firma del comprobante electrónico / Voucher Signature Type.
        /// </value>
        public FirmType? TipoFirma { get; set; }

        /// <summary>
        /// Obtiene o Almacena el estatus de la firma del comprobante / Gets the Voucher Signature Status.
        /// </summary>
        /// <value>
        /// Estatus de la firma del comprobante / Voucher Signature Status.
        /// </value>
        public StatusFirmaDigital? StatusFirmaDigital { get; set; }

        /// <summary>
        /// Obtiene la fecha de creación del comprobante / Gets the Voucher Creation Date.
        /// </summary>
        /// <value>
        /// Fecha de creación del comprobante / Voucher Creation Date.
        /// </value>
        public DateTime CreationTime { get; set; }
    }
}
