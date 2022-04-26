using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using PagedList;
using TicoPay.Clients;
using TicoPay.Common;

namespace TicoPay.Groups.Dto
{
    public class SearchGroupsInput : IPagedResultRequest, IDtoViewBaseFields
    {
        public const int DefaultPageSize = 5;

        [StringLength(40)]
        public string SearchTerm { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public int? Page { get; set; }

        public IPagedList<GroupDto> Entities { get; set; }

        public IList<ClientGroup> ClientGroups { get; set; } 

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

        public SearchGroupsInput()
        {
            MaxResultCount = DefaultPageSize;
        }
    }
}
