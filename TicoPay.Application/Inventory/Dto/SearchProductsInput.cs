using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using PagedList;
using TicoPay.Common;
using System.Web.Mvc;
using TicoPay.Inventory;
using System;
using TicoPay.Taxes;
using TicoPay.Invoices;
using static TicoPay.Inventory.Product;

namespace TicoPay.Inventory.Dto
{
    public class SearchProductsInput : IPagedResultRequest, IDtoViewBaseFields
    {
        public const int DefaultPageSize = 5;

        [StringLength(128)]
        public string SearchTerm { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public int? Page { get; set; }

        public IPagedList<ProductDto> Entities { get; set; }
        
        public int? ErrorCode { get ; set; }
        public string ErrorDescription { get ; set; }
        public string Action { get ; set ; }
        public string Control { get ; set ; }
        public string Query { get; set; }
        public string NameFilter { get; set; }
        public Guid? BrandFilter { get; set; }
        public decimal? PriceSinceFilter { get; set; }
        public decimal? PriceUntilFilter { get; set; }
        public Guid? TaxFilter { get; set; }

        public Estatus? EstadoFilter { get; set; }

        public Guid? TaxId { get; set; }
        public Guid? BrandId { get; set; }

        public IList<Tax> Taxes { get; set; }
        public IList<Brand> Brands { get; set; }


        public SearchProductsInput()
        {
            MaxResultCount = DefaultPageSize;
        }
        public LineType Tipo { get; set; } = LineType.Product;
    }
}
