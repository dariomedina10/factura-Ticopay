using Abp.Application.Services.Dto;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Common;

namespace TicoPay.BranchOffices.Dto
{
    public class SearchBranchOfficesInput : IPagedResultRequest, IDtoViewBaseFields
    {
        public const int DefaultPageSize = 5;

       
        public string SearchTerm { get; set; }
        public string NameFilter { get; set; }
        public string CodeFilter { get; set; }

        public bool isEnabled { get; set; }

        public int MaxResultCount { get; set; }
           
      
        public int SkipCount { get; set; }

        public int? Page { get; set; }

        public IPagedList<BranchOfficesDto> Entities { get; set; }

       // public IList<ClientService> ClientServices { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get ; set ; }
        public string Action { get ; set ; }
        public string Control { get ; set ; }
        public string Query { get ; set ; }
    }
}
