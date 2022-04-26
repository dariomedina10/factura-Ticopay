using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using PagedList;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.Common;
using TicoPay.Invoices.Dto;
using TicoPay.MultiTenancy;

namespace TicoPay.ConsultaRecibos.Dto
{
    public class SearchConsultaRecibosInput : IPagedResultRequest, IDtoViewBaseFields
    {
        public const int DefaultPageSize = 10;

        [StringLength(40)]
        public string SearchTerm { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public int? Page { get; set; }

        public long TenantId { get; set; }

        public IList<Tenant> Tenants { get; set; }

        public IList<InvoiceDto> Entities { get; set; }

        public string Identification { get; set; }

        public ClientDto ClientInfo { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

        public SearchConsultaRecibosInput()
        {
            MaxResultCount = DefaultPageSize;
        }
    }
}
