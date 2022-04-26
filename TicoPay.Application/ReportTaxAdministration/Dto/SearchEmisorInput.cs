using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using PagedList;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.Common;

namespace TicoPay.ReportTaxAdministration.Dto
{
    public class SearchEmisorInput : IPagedResultRequest, IDtoViewBaseFields
    {
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

        public Guid ClientId { get; set; }

        public const int DefaultPageSize = 3;

        [StringLength(40)]
        public string SearchTerm { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public Guid? InvoiceSelect { get; set; }

        public int? Page { get; set; }

        public IPagedList<EmisorDto> Entities { get; set; }

        public SearchEmisorInput()
        {
            MaxResultCount = DefaultPageSize;
        }

    }
}
