using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;
using TicoPay.Clients;
using TicoPay.ReportStatusInvoices.Dto;
using TicoPay.Invoices.Dto;
using BCR;

namespace TicoPay.ReportClosing.Dto
{
    
    public class ReportClosingDto
    {

        public IList<Invoice> InvoicesList { get; set; }

        public IList<Client> ClientList { get; set; }

        public ICollection<Group> Groups { get; set; }

        public Guid? GroupsId { get; set; }

        public DateTime? InitialDate { get; set; }

        public DateTime? FinalDate { get; set; }

        public FacturaElectronicaResumenFacturaCodigoMoneda? CodigoMoneda { get; set; }

        public FacturaElectronicaResumenFacturaCodigoMoneda? CodigoMonedaTenant { get; set; }
        public FacturaElectronicaResumenFacturaCodigoMoneda? CodigoMonedaFiltro { get; set; }

        [Display(Name = "No. Factura")]
        public string ConsecutiveNumber { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

        public DateTime CreationTime { get; set; }

        public decimal RateDay { get; set; }

        public virtual IList<Invoice> RateD { get; set; }

        public int Number { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? InvoiceId { get; set; }
        public PaymetnMethodType InvoicePaymentTypes { get; set; }

        [Display(Name = "Cliente")]
        public string ClientName { get; set; }

        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Display(Name = "Usuarios")]
        public string UserName { get; set; }

        [Display(Name = "Código")]
        public int ClientCode { get; set; }

        [Display(Name = "Fecha")]
        public DateTime DueDate { get; set; }
      
        public Status Status { get; set; }       
        
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        public decimal TotalC { get; set; }

        public decimal TasaConversion { get; set; }

        public bool ShowConversion { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public bool SendInvoice { get; set; }

        //public ICollection<ReportStatusNoteDto> Notes { get;  set; }
                
        [Display(Name = "Estado Hacienda")]
        public StatusTaxAdministration StatusTribunet { get; set; }

        public ICollection<ReportStatusNoteDto> Notes { get; set; }

        public ICollection<InvoiceLineDto> InvoiceLines { get; set; }
        public IEnumerable<ResultRateDto> Tasa { get; internal set; }

        public IEnumerable<ReportPaymentInvoice>  PaymentInvoices { get; internal set; }

        public TypeDocumentInvoice? TypeDocument { get; set; }
    }

   
}
