using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using PagedList;
using TicoPay.Common;

namespace TicoPay.Invoices.Dto
{
    public class SearchRegistersInvoicesInput : IPagedResultRequest, IDtoViewBaseFields
    {
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

        public Guid RegisterId { get; set; }

        public string RegisterCode { get; set; }
        public string Name { get; set; }

        public const int DefaultPageSize = 3;

        [StringLength(40)]
        public string SearchTerm { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public Guid? InvoiceSelect { get; set; }

        public int? Page { get; set; }

        public IPagedList<Register> Entities { get; set; }

        public SearchRegistersInvoicesInput()
        {
            MaxResultCount = DefaultPageSize;
        }

    }
}
