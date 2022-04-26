using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using PagedList;
using TicoPay.Common;
using System.Web.Mvc;
using TicoPay.Taxes;
using System;
using TicoPay.Invoices;

namespace TicoPay.Services.Dto
{
    public class SearchServicesInput : IPagedResultRequest, IDtoViewBaseFields
    {
        public const int DefaultPageSize = 5;

        [StringLength(128)]
        public string SearchTerm { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public int? Page { get; set; }

        public IPagedList<ServiceDto> Entities { get; set; }

        public IList<ClientService> ClientServices { get; set; }

        public IList<InvoiceLine> InvoiceLine { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        public string NameFilter { get; set; }
        public decimal? PriceSinceFilter { get; set; }
        public decimal? PriceUntilFilter { get; set; }
        public Guid? TaxId { get; set; }

        public IList<Tax> Taxes { get; set; }
        public int? RecurrentId { get; set; }
        public IEnumerable<SelectListItem> Recurrents { get; set; }

        public SearchServicesInput()
        {
            MaxResultCount = DefaultPageSize;
        }
        public LineType Tipo { get; set; } = LineType.Service;
    }
}
