using System.Collections.Generic;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using PagedList;
using TicoPay.Common;
using TicoPay.Invoices;
using System.Web.Mvc;

namespace TicoPay.Clients.Dto
{
    public class SearchClientsInput : IPagedResultRequest, IDtoViewBaseFields
    {
        public const int DefaultPageSize = 5;

        [StringLength(40)]
        public string SearchTerm { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public int? Page { get; set; }

        public IPagedList<ClientDto> Entities { get; set; }

        public IList<Invoice> InvoicesList { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        [MaxLength(10)]
        [StringLength(10)]
        public string CodeFilter { get; set; }
        [MaxLength(80)]
        [StringLength(80)]
        public string NameFilter { get; set; }
        [MaxLength(20)]
        [StringLength(20)]
        public string IdentificationFilter { get; set; }
        [MaxLength(100)]
        [StringLength(100)]
        public string EmailFilter { get; set; }
        public string GroupId { get; set; }

        public SelectList Groups { get; set; }


        public SearchClientsInput()
        {
            MaxResultCount = DefaultPageSize;
        }
    }
}
