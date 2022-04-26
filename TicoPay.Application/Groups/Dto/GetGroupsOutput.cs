using System.Collections.Generic;
using Abp.Application.Services.Dto;
using TicoPay.Common;

namespace TicoPay.Groups.Dto
{
    public class GetGroupsOutput : IDtoViewBaseFields
    {
        public List<GroupDto> Groups { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }

}
