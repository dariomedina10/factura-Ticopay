using Abp.Application.Editions;
using Abp.Application.Services.Dto;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.BranchOffices;
using TicoPay.Common;
using TicoPay.Editions;

namespace TicoPay.Drawers.Dto
{
    public class SearchDrawerInput : IPagedResultRequest, IDtoViewBaseFields
    {
        public const int DefaultPageSize = 5;

        public int TenantId { get; set; }

        public IPagedList<DrawerDto> Entities { get; set; }

        public int SkipCount { get ; set ; }
        public int MaxResultCount { get ; set ; }

        public int? Page { get; set; }

        public string CodeFilter { get; set; }
        public Guid BranchOfficeFilter { get; set; }

        public int? ErrorCode { get ; set ; }
        public string ErrorDescription { get ; set ; }
        public string Action { get ; set ; }
        public string Control { get ; set; }
        public string Query { get ; set ; }

        public BranchOffice BranchOffice { get; set; }
        public List<BranchOffice> BranchOffices { get; set; }

        public bool isEnabled { get; set; }

        public Edition Edition { get; set; }
        public List<EditionManager> Editions { get; set; }
    }
}
