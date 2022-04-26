using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices;

namespace TicoPay.ReportStatusInvoices.Dto
{
    public class ReportPaymentInvoice
    {
        [Display(Name = "Monto de Pago")]
        public decimal Amount { get; set; }

        [Display(Name = "Fecha de Pago")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Fecha de Devolución")]
        public DateTime? LastModificationTime { get; set; }

        [Display(Name = "Tipo de Pago")]
        public PaymentType PaymentInvoiceType { get; set; }

        [Display(Name = "Método de Pago")]
        public PaymetnMethodType PaymetnMethodType { get; set; }

        [Display(Name = "Transacción")]
        public string Transaction { get; set; }

        public Guid InvoiceId { get; set; }
    }
}
