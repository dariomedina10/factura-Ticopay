using Abp.Application.Services.Dto;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Services;

namespace TicoPay.GroupConcept.Dto
{
    public class SearchGroupConceptsInput : IPagedResultRequest, IDtoViewBaseFields
    {
        public const int DefaultPageSize = 5;

        [StringLength(40)]
        public string SearchTerm { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public int? Page { get; set; }

        public IPagedList<GroupConceptsDto> Entities { get; set; }

        public IList<Client> Clients { get; set; }
        public IList<Service> Services { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

        public SearchGroupConceptsInput()
        {
            MaxResultCount = DefaultPageSize;
        }
    }
}
