using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Common;

namespace TicoPay.Roles.Dto
{
    public class SearchRolesInput : IPagedResultRequest, IDtoViewBaseFields
    {
        public const int DefaultPageSize = 5;

        public int? Page { get; set; }

        public int MaxResultCount { get; set; }

        public IPagedList<RolesDTO> Entities { get; set; }
        public IList<UserRole> UserRoles { get; set; }

        public SearchRolesInput()
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
