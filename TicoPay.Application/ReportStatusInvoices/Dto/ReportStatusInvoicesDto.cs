
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;

namespace TicoPay.ReportStatusInvoices.Dto
{
    
    public class ReportStatusInvoicesDto
    {

       
        [Display(Name = "No. Factura")]
        public string ConsecutiveNumber { get; set; }

    
        public Guid? ClientId { get; set; }

        [Display(Name = "Cliente")]
        public string ClientName { get; set; }

        [Display(Name = "Código")]
        public int ClientCode { get; set; }
     

        [Display(Name = "Fecha")]
        public DateTime DueDate { get; set; }
      
        public Status Status { get; set; }       
        
        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }

        [Display(Name = "Total")]
        public decimal Total { get; set; }

        public bool SendInvoice { get; set; }

        public ICollection<ReportStatusNoteDto> Notes { get;  set; }
                
        [Display(Name = "Estado Hacienda")]
        public StatusTaxAdministration StatusTribunet { get; set; }

        [Display(Name = "Estado Recepción")]
        public bool IsInvoiceReceptionConfirmed
        {
            get; set;
        }

        public string MessageTaxAdministration { get; set; }
        public bool HasElectronicBill { get; set; }

        public TypeDocumentInvoice TypeDocument { get; set; }

        public Guid Id { get; set; }

        public int MinimumAmount { get; set; }
        public int MaxAmount { get; set; }
    }

   
}
