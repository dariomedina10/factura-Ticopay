using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.BranchOffices;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;

namespace TicoPay.ReportClosing.Dto
{
    public class DashboardDto : IDtoViewBaseFields
    {
        public int?  ClientCount { get; set; }
        public int? UserCount { get; set; }
        public decimal? TotalInvoice { get; set; }
        public decimal? TotalPayment { get; set; }
        public IList<ResultTotalDto> InvoicesList { get; set; }
        public IList<ResultTotalDto> PaymentList { get; set; }
        public IList<ResultTotalDto> NoteCreditList { get; set; }
        public IList<ResultTotalDto> NoteDebitList { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }

        public FacturaElectronicaResumenFacturaCodigoMoneda Moneda { get; set; }
        public bool IsUSDOfCRC { get; set; }


        public bool IsTutorialCompania { get; set; }
        public bool IsTutotialServices { get; set; }
        public bool IsTutorialClients { get; set; }
        public bool IsTutorialProduct { get; set; }

    
        public Guid? BranchOfficeId { get; set; }       
        public List<BranchOffice> BranchOffice { get; set; }
        public Guid? DrawerId { get; set; }
        public IList<SelectListItem> DrawerUser { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
     
        

    }
}
