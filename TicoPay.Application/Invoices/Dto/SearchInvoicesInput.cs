using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using PagedList;
using TicoPay.Clients.Dto;
using TicoPay.Common;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TicoPay.Clients;
using TicoPay.Drawers;
using TicoPay.BranchOffices;

namespace TicoPay.Invoices.Dto
{
    public class SearchInvoicesInput : IPagedResultRequest, IDtoViewBaseFields
    {
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

        public Guid? ClientId { get; set; }

        public const int DefaultPageSize = 10;

        [StringLength(40)]
        public string SearchTerm { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public Guid? InvoiceSelect { get; set; }

        public ClientDto ClientInfo { get; set; }

        public int? Page { get; set; }

        public Guid? GroupsId { get; set; }

        [Display(Name = "Fecha Inicio")]
        public DateTime? StartDueDate { get; set; }

        [Display(Name = "Fecha Fin")]
        public DateTime? EndDueDate { get; set; }

        [Display(Name = "Estado")]
        public Status? Status { get; set; }

        public Guid? RegisterId { get; set; }

        [Display(Name = "Caja")]
        public string RegisterCode { get; set; }

        [Display(Name = "Cliente")]
        public string ClientName { get; set; }

        [Display(Name = "Nro. Factura")]
        public string ConsecutiveNumber { get; set; }

        [Display(Name = "Sucursal")]
        public Guid? BranchOfficeId { get; set; }

        [Display(Name = "Caja")]
        public Guid? DrawerId { get; set; }

        [Display(Name = "Sucursal")]
        public List<BranchOffice> BranchOffice { get; set; }

        //SelectListItem
        public IList<SelectListItem> DrawerUser { get; set; }

        public IPagedList<InvoiceDto> Entities { get; set; }

        public TypeDocumentInvoice? TypeDocument { get; set; }

        public IList<SelectListItem> StatusesInvoice
        {
            get
            {
                
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(Invoices.Status.Completed, Invoices.Status.Parked, Invoices.Status.Voided, Invoices.Status.Returned, Invoices.Status.LayBy, Invoices.Status.OnAccount);
            }
        }

        public IList<SelectListItem> ListTypeDocument
        {
            get
            {
                return TicoPay.Application.Helpers.EnumHelper.GetSelectList(TypeDocumentInvoice.Invoice, TypeDocumentInvoice.Ticket);
            }
        }


        public ICollection<Group> Groups { get; set; }

        public Drawer Drawer { get; set; }

        public SearchInvoicesInput()
        {
            MaxResultCount = DefaultPageSize;
        }

    }
}
