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
using TicoPay.Users;

namespace TicoPay.ReportClosing.Dto
{
    public class ReportClosingInputDto<T> : IDtoViewBaseFields
    {

        public IList<T> InvoicesList { get; set; }

        public IList<Client> ClientList { get; set; }

        public ICollection<Group> Groups { get; set; }

        public Guid? GroupsId { get; set; }

        public DateTime? InitialDate { get; set; }

        public DateTime? FinalDate { get; set; }

        public FacturaElectronicaResumenFacturaCodigoMoneda? CodigoMoneda { get; set; }

        [Display(Name = "Sucursal")]
        public Guid? BranchOfficeId { get; set; }

        [Display(Name = "Caja")]
        public Guid? DrawerId { get; set; }

        [Display(Name = "Sucursal")]
        public List<BranchOffice> BranchOffice { get; set; }

        [Display(Name = "Usuarios")]
        public int? UserId { get; set; }

        [Display(Name = "Usuarios")]
        public List<User> Usuarios { get; set; }

        public string UserRol { get; set; }

        //SelectListItem
        public IList<SelectListItem> DrawerUser { get; set; }

        public string ConsecutiveNumber { get; set; }

        public PaymentOrigin? PaymentOrigin { get; set; }
        public IList<SelectListItem> ListPaymentOrigin
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(Invoices.PaymentOrigin.ticopay, Invoices.PaymentOrigin.BNpay);
            }
        }

        public TypeDocumentInvoice? TypeDocument { get; set; }

        public IList<SelectListItem> ListTypeDocument
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(TypeDocumentInvoice.Invoice, TypeDocumentInvoice.Ticket);
            }
        }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

        public DateTime CreationTime { get; set; }

        public decimal RateDay { get; set; }

        public virtual IList<Invoice> RateD { get; set; }
    }
}
