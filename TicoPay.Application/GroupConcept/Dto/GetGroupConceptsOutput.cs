using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Common;

namespace TicoPay.GroupConcept.Dto
{
    public class GetGroupConceptsOutput : IDtoViewBaseFields
    {
        public List<GroupConceptsDto> GroupConcepts { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
