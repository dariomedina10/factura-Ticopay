using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using PagedList;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.Groups.Dto;
using TicoPay.Users;

namespace TicoPay.Groups
{
    public interface IGroupAppService : IApplicationService
    {
        ListResultDto<GroupDto> GetGroups();
        GroupDto Get(Guid input);
        GroupDetailOutput GetDetail(Guid input);
        UpdateGroupInput GetEdit(Guid input);
        void Update(UpdateGroupInput input);
        void Create(CreateGroupInput input);
        GroupDto Create(GroupDto input);
        void Delete(Guid input);
        IPagedList<GroupDto> SearchGroups(SearchGroupsInput searchInput);
        IList<User> GetAllUser();
        IList<ClientGroup> GetAllClientGroups();
        bool isAllowedDelete(Guid Id);
    }
}
