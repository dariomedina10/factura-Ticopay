
using System;
using System.ComponentModel.DataAnnotations;
using TicoPay.Invoices;

namespace TicoPay.ReportStatusInvoices.Dto
{
  
    public class ReportStatusNoteDto
    {
        [Display(Name = "Fecha")]
        public DateTime CreationTime { get; set; }

        [Display(Name = "Monto")]
        public decimal Amount { get; set; }


        [Display(Name = "Impuesto")]
        public decimal TaxAmount { get; set; }

        [Display(Name = "Monto Total")]
        public decimal Total { get; set; }


        public NoteCodigoMoneda CodigoMoneda { get; set; }


        [Display(Name = "No. Nota")]
        public string ConsecutiveNumber { get; set; }

        public Guid InvoiceId { get; set; }

        [Display(Name = "Tipo")]
        public NoteType NoteType { get; set; }


        [Display(Name = "Enviado Hacienda")]
        public bool SendInvoice { get; set; }

        [Display(Name = "Estado Hacienda")]
        public StatusTaxAdministration StatusTribunet { get; set; }

        [Display(Name = "Acuse Recibido")]
        public bool IsNoteReceptionConfirmed { get; set; }

        public bool HasMessageTaxAdministration { get; set; }

        public int MinimumAmount { get; set; }
        public int MaxAmount { get; set; }

    }
}
