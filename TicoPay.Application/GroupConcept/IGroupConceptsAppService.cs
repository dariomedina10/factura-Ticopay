using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.GroupConcept.Dto;
using TicoPay.Users;

namespace TicoPay.GroupConcept {
    public interface IGroupConceptsAppService : IApplicationService
    {
        ListResultDto<GroupConceptsDto> GetGroupConcepts();
        GroupConceptsDto Get(Guid input);
        GroupConceptsDetailOutput GetDetail(Guid input);
        UpdateGroupConceptsInput GetEdit(Guid input);
        void Update(UpdateGroupConceptsInput input);
        GroupConceptsDto Create(CreateGroupConceptsInput input);
        void Delete(Guid input);
        IPagedList<GroupConceptsDto> SearchGroupConcepts(SearchGroupConceptsInput searchInput);
        IList<GroupConcepts> GetGroupConceptsEntities();
        IList<GroupConcepts> GetGroupConceptsEntities(int tenantId);
        bool isAllowedDelete(Guid Id);
    }
}
