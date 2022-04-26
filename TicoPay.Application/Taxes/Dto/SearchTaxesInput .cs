using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
//using PagedList;
using TicoPay.Common;
using TicoPay.Services;
using PagedList;

namespace TicoPay.Taxes.Dto
{
    public class SearchTaxesInput : IPagedResultRequest, IDtoViewBaseFields
    {
       public Guid? UserId { get; set; }

        public const int DefaultPageSize = 5;

        public int? Page { get; set; }

        public int MaxResultCount { get; set; }

        public IPagedList<TaxDto> Entities { get; set; }

        public IList<Service> Services { get; set; }

        public SearchTaxesInput()
        {
            MaxResultCount = DefaultPageSize;
        }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    
        public int SkipCount { get; set; }
      }
}
