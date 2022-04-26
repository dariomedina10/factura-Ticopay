using System;
using System.Collections.Generic;
using System.Text;
using TicoPayDll.Clients;
using TicoPayDll.Services;

namespace TicoPayDll.Invoices
{
    public class CreateInvoice
    {
        public CreateInvoice()
        {
            TypeDocument = DocumentType.Invoice;
            TipoFirma = FirmType.Llave;
            ExternalReferenceNumber = "N/A";
        }

        public Guid? ClientId { get; set; }

        public string ClientName { get; set; }

        public IdentificacionTypeTipo ClientIdentificationType { get; set; }

        public string ClientIdentification { get; set; }

        public string ClientPhoneNumber { get; set; }

        public string ClientMobilNumber { get; set; }

        public string ClientEmail { get; set; }

        public virtual List<ItemInvoice> InvoiceLines { get; set; }

        public List<PaymentInvoce> ListPaymentType { get; set; }

        public decimal? DiscountGeneral { get; set; }

        public int? TypeDiscountGeneral { get; set; }

        public int? CreditTerm { get; set; }

        public DateTime ExpirationDate { get; set; }

        public CodigoMoneda? CodigoMoneda { get; set; }

        public FirmType? TipoFirma { get; set; }

        public DocumentType TypeDocument { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Numero de Referencia Externa (Usuarios del Api).
        /// </summary>
        /// <value>
        /// Numero de Referencia Externa (Usuarios del Api).
        /// </value>
        public string ExternalReferenceNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena una nota general del documento.
        /// </summary>
        /// <value>
        /// Nota general del documento.
        /// </value>
        public string GeneralObservation { get; set; }
    }

    public class ItemInvoice
    {
        public ItemInvoice()
        {
            Tipo = LineType.Service;
        }

        public int ID { get; set; }

        public string IdService { get; set; }

        public decimal Cantidad { get; set; }

        public decimal Descuento { get; set; }

        public Guid IdImpuesto { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Precio { get; set; }

        public string Servicio { get; set; }

        public decimal Total { get; set; }

        public decimal TotalDescuento { get; set; }

        public decimal TotalImpuesto { get; set; }

        public UnidadMedidaType? UnidadMedida { get; set; }

        public string UnidadMedidaOtra { get; set; }
                
        public string Note { get; set; }

        public LineType Tipo { get; set; }

    }

    public class PaymentInvoce
    {
        public int TypePayment { get; set; }

        public string Trans { get; set; }

        public decimal Balance { get; set; }

    }
}
