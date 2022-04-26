using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.BranchOffices;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;

namespace TicoPay.ReportAccountsReceivable.Dto
{
    public class ReportAccountsReceivableInputDto : IDtoViewBaseFields
    {
        public IList<Invoice> InvoicesList { get; set; }

        public IList<Client> ClientList { get; set; }

        public ICollection<Group> Groups { get; set; }

        public DateTime? InitialDate { get; set; }

        public DateTime? FinalDate { get; set; }

        public Status? Status { get; set; }

        public PaymetnMethodType? PaymetnMethodType { get; set;}

        public FacturaElectronicaResumenFacturaCodigoMoneda? CodigoMoneda { get; set; }

        public FacturaElectronicaResumenFacturaCodigoMoneda MonedaTenant { get; set; }

        public string ConsecutiveNumber { get; set; }

        public Guid? ClientId { get; set; }

        public Guid? ServiceId { get; set; }
        
        public Guid? GroupsId { get; set; }

        public TypeDocumentInvoice? TypeDocument { get; set; }

        public IList<SelectListItem> ListTypeDocument
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(TypeDocumentInvoice.Invoice, TypeDocumentInvoice.Ticket);
            }
        }

        [Display(Name = "Sucursal")]
        public Guid? BranchOfficeId { get; set; }

        [Display(Name = "Caja")]
        public Guid? DrawerId { get; set; }

        [Display(Name = "Sucursal")]
        public List<BranchOffice> BranchOffice { get; set; }

        //SelectListItem
        public IList<SelectListItem> DrawerUser { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
