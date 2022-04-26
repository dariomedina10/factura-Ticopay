using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using PagedList;
using TicoPay.Common;
using TicoPay.Authorization.Roles;

namespace TicoPay.Users.Dto
{
    public class SearchUsersInput : IPagedResultRequest, IDtoViewBaseFields
    {

        public const int DefaultPageSize = 5;

        [StringLength(40)]
        public string SearchTerm { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public int? Page { get; set; }

        public IPagedList<UserListDto> Entities { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
        [MaxLength(50)]
        [StringLength(50)]
        public string NameFilter { get; set; }
        [MaxLength(50)]
        [StringLength(50)]
        public string SurnameFilter { get; set; }
        [MaxLength(100)]
        [StringLength(100)]
        public string EmailAddressFilter { get; set; }
        public IList<Role> Roles { get; set; }
        public int? IdRolFilter { get; set; }

        public SearchUsersInput()
        {
            MaxResultCount = DefaultPageSize;
        }
    }
}
