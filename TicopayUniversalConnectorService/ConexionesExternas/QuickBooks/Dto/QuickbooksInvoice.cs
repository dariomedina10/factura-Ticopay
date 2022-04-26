using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Invoices;
using TicoPayDll.Services;

namespace TicopayUniversalConnectorService.ConexionesExternas.QuickBooks.Dto
{
    public class QuickbooksInvoice
    {
        public string invoiceID { get; set; }

        public string ClientId { get; set; }

        public string EditSequence { get; set; }

        public TipoFactura Tipo { get; set; }

        public virtual List<QuickbooksItemInvoice> InvoiceLines { get; set; }

        public List<QuickbooksPaymentInvoce> ListPaymentType { get; set; }

        public decimal? DiscountGeneral { get; set; }

        public int? TypeDiscountGeneral { get; set; }

        public int? CreditTerm { get; set; }

        public DateTime ExpirationDate { get; set; }

        public CodigoMoneda? CodigoMoneda { get; set; }

        public string NumeroReferencia { get; set; }

        public string GeneralObservation { get; set; }
    }

    public class QuickbooksItemInvoice
    {
        public int ID { get; set; }

        public string IdService { get; set; }

        public decimal Cantidad { get; set; }

        public decimal Descuento { get; set; }

        public Guid IdImpuesto { get; set; }

        public decimal Impuesto { get; set; }

        public decimal TasaImpuesto { get; set; }

        public decimal Precio { get; set; }

        public string Servicio { get; set; }

        public decimal Total { get; set; }

        public decimal TotalDescuento { get; set; }

        public decimal TotalImpuesto { get; set; }

        public UnidadMedidaType? UnidadMedida { get; set; }

        public string UnidadMedidaOtra { get; set; }

        public string Note { get; set; }

        public string ItemId { get; set; }

        public TipoItem ItemType { get; set; }

    }

    public class QuickbooksPaymentInvoce
    {
        public int TypePayment { get; set; }

        public string Trans { get; set; }

        public decimal Balance { get; set; }

    }

    public enum TipoFactura
    {
        Contado,
        Credito,
    }

    public enum TipoItem
    {
        Servicio,
        Producto,
    }
}
