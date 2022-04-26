using System.Collections.Generic;
using Abp.Application.Services.Dto;
using TicoPay.Common;

namespace TicoPay.Taxes.Dto
{
    public class GetTaxesOutput : IDtoViewBaseFields
    {
        public List<TaxDto> Taxes { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }

}
