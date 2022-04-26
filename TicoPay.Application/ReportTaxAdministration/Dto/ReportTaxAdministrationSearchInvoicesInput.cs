using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.Common;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;

namespace TicoPay.ReportTaxAdministration.Dto
{
    public class ReportTaxAdministrationSearchInvoicesInput
    {
        [Display(Name = "Emisor")]
        public string NombreEmisor { get; set; }

        public string CedulaEmisor { get; set; }

        public int? EmisorId { get; set; }

        [Display(Name = "Receptor")]
        public string NombreReceptor { get; set; }

        public string CedulaReceptor { get; set; }

        public Guid? ReceptorId { get; set; }

        [Display(Name = "Número")]
        public int? NumeroFactura { get; set; }

        //public Guid? IdNotaCredito { get; set; }
        //public Guid? IdNotaDebito { get; set; }

        [Display(Name = "Medio Pago")]
        public PaymetnMethodType? MedioPago { get; set; }

        [Display(Name = "Condición Venta")]
        public FacturaElectronicaCondicionVenta? CondicionVenta { get; set; }

        [Display(Name = "Fecha Emisión (Désde)")]
        public DateTime? FechaEmisionDesde { get; set; }

        [Display(Name = "Fecha Emisión (Hásta)")]
        public DateTime? FechaEmisionHasta { get; set; }

        [Display(Name = "Monto")]
        [DataType(DataType.Currency)]
        public decimal? MontoDesde { get; set; }

        [Display(Name = "Monto")]
        [DataType(DataType.Currency)]
        public decimal? MontoHasta { get; set; }

        [Display(Name = "Total Impuestos")]
        [DataType(DataType.Currency)]
        public decimal? TotalImpuestosDesde { get; set; }

        [Display(Name = "Total Impuestos")]
        [DataType(DataType.Currency)]
        public decimal? TotalImpuestosHasta { get; set; }

        [Display(Name = "Estado Hacienda")]
        public StatusTaxAdministration? StatusTribunet { get; set; }

        [Display(Name = "Acuse Recibo")]
        public bool? RecepcionConfirmada { get; set; }

        [Display(Name = "Estado")]
        public Status? Status { get; set; }

        public List<ViewInvoiceDto> Invoices { get; set; }

        public IList<SelectListItem> StatusesInvoice
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(TicoPay.Invoices.Status.Completed, TicoPay.Invoices.Status.Parked, TicoPay.Invoices.Status.Voided);
            }
        }

        public IList<SelectListItem> StatusesTribunet
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(typeof(StatusTaxAdministration));
            }
        }
    }
}
