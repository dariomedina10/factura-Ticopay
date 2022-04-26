using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;

namespace TicoPay.ReportStatusInvoices.Dto
{
    public class ReportInvoicesNotes
    {
        [Display(Name = "Tipo de documento")]
        public TypeDocument Type { get; set; }

        [Display(Name = "No. Documento")]
        public string ConsecutiveNumber { get; set; }

        public string ConsecutiveNumberReference { get; set; }

        public Guid? ClientId { get; set; }

        [Display(Name = "Nombre Cliente")]
        public string ClientName { get; set; }

        [Display(Name = "Nº Identificación")]
        public string ClientIdentification { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Date { get; set; }

        [Display(Name = "Estatus")]
        public Status Status { get; set; }

        public string CodigoMoneda { get; set; }

        [Display(Name = "Monto")]
        public decimal Total { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Descuento { get; set; }

        [Display(Name = "Estatus Hacienda")]
        public StatusTaxAdministration StatusTribunet { get; set; }

        [Display(Name = "Estado Recepción")]
        public bool IsInvoiceReceptionConfirmed {get; set;}

        public ICollection<ReportInvoicesNotesList> Notes { get; set; }


        public ICollection<ReportPaymentInvoice> PaymentInvoices { get; set; }

        public IdentificacionTypeTipo? IdentificationType { get; set; }

        public string IdentificacionExtranjero { get; set; }



    }

    public class ReportInvoicesNotesList
    {
        [Display(Name = "Tipo de documento")]
        public TypeDocument Type { get; set; }

        [Display(Name = "No. Documento")]
        public string ConsecutiveNumber { get; set; }

        public string ConsecutiveNumberReference { get; set; }

        public Guid? ClientId { get; set; }

        [Display(Name = "Nombre Cliente")]
        public string ClientName { get; set; }

        [Display(Name = "Nº Identificación")]
        public string ClientIdentification { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Date { get; set; }

        public string CodigoMoneda { get; set; }
        //public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }
        [Display(Name = "Monto")]
        public decimal Total { get; set; }



    }

    public enum TypeDocument
    {
        [Description("Nota de Débito")]
        NoteDebito = 0,
        [Description("Nota de Crédito")]
        NoteCredito,
        [Description("Factura")]
        Invoice,
        [Description("Tiquete Electrónico")]
        Ticket

    }
}
