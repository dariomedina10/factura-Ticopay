using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices.XSD;

namespace TicoPay.ReportStatusInvoices.Dto
{
    public class IntegrationSVContaZoho: ReportInvoicesNotes
    {
        public TypeDocument Type { get; set; }
        public FacturaElectronicaCondicionVenta ConditionSaleType { get; set; }
        public decimal TotalGravado { get; set; }

        public decimal TotalExento { get; set; }

        public ICollection<IntegrationSVContaTax> Tax { get; set; }

    }

    public class IntegrationSVContaTax
    {
        public Guid? TaxId { get; set; }

        public decimal? TaxRate { get; set; }
    }

    public class IntegrationZohoDataSet
    {
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string IdentificacionPasaporte { get; set; }
        public string Cliente { get; set; }
        public string FechaDocumento { get; set; }
        public string CondicionVenta { get; set; }
        public decimal Impuesto { get; set; }
        public string Moneda { get; set; }
        public decimal MontoTotalExento { get; set; }
        public decimal MontoTotalGravado { get; set; }
        public string FechaPago { get; set; }
        public string TipoPago { get; set; }
        public string MetodoPago { get; set; }
        public string Transaccion { get; set; }
        public decimal MontoPago { get; set; }
    }
}
