using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using AutoMapper;
using Abp.AutoMapper;
using TicoPay.Groups.Dto;
using System;
using Abp.Application.Services.Dto;
using System.Linq;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using PagedList;
using TicoPay.Clients;
using TicoPay.Users;
using LinqKit;

namespace TicoPay.Groups
{
    public class GroupAppService : ApplicationService, IGroupAppService
    {
        //These members set in constructor using constructor injection.
        private readonly IRepository<Group, Guid> _groupRepository;
        private readonly IRepository<ClientGroup, Guid> _clientGroupRepository;
        private readonly IGroupManager _groupManager;
        public readonly UserManager _userManager;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public GroupAppService(IRepository<Group, Guid> groupRepository, IGroupManager groupManager, UserManager userManager,
            IRepository<ClientGroup, Guid> clientGroupRepository)
        {
            _groupRepository = groupRepository;
            _groupManager = groupManager;
            _userManager = userManager;
            _clientGroupRepository = clientGroupRepository;
        }

        public void Create(CreateGroupInput input)
        {
            var @group = Group.Create(AbpSession.GetTenantId(), input.Name, input.Description);
            _groupManager.Create(@group);
        }

        public GroupDto Create(GroupDto input)
        {
            var @group = Group.Create(AbpSession.GetTenantId(), input.Name, input.Description);
            _groupManager.Create(@group);

            return @group.MapTo<GroupDto>();
        }

        public void Delete(Guid input)
        {
            var @group = _groupRepository.Get(input);
            if (@group == null)
            {
                throw new UserFriendlyException("Could not found the group, maybe it's deleted.");
            }

            @group.IsDeleted = true;
            _groupRepository.Update(@group);
        }


        public ListResultDto<GroupDto> GetGroups()
        {
            var groups = _groupRepository.GetAllList();
            return new ListResultDto<GroupDto>(groups.MapTo<List<GroupDto>>());
        }

        public GroupDetailOutput GetDetail(Guid input)
        {
            var @group = _groupRepository.FirstOrDefault(input);

            if (@group == null)
            {
                throw new UserFriendlyException("Could not found the group, maybe it's deleted.");
            }

            return @group.MapTo<GroupDetailOutput>();
        }

        public GroupDto Get(Guid input)
        {
            try
            {
                var @group = _groupRepository.Get(input);               
                return Mapper.Map<GroupDto>(@group);
            }
            catch
            {
                return null;
            }
            
        }

        /// <summary>
        /// Searches for groups and returns page result
        /// </summary>
        /// <param name="searchInput"></param>
        /// <returns></returns>
        public IPagedList<GroupDto> SearchGroups(SearchGroupsInput searchInput)
        {
            if (searchInput.Query == null)
                searchInput.Query = "";
            else
                searchInput.Query = searchInput.Query.ToLower();
            //15
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value  : 1;

            //lets search by Identification or Name or Email or Code
            var groups = _groupRepository.GetAll().Where(c => c.Name.ToLower().Contains(searchInput.Query)
                                                              || c.Description.ToLower().Contains(searchInput.Query) || c.Name.ToLower().Equals(searchInput.Query)
                                                           ).OrderByDescending(p => p.Name).ToList();

            return groups.MapTo<List<GroupDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
        }

        public UpdateGroupInput GetEdit(Guid input)
        {
            var @tax = _groupRepository.Get(input);
            if (@tax == null)
            {
                throw new UserFriendlyException("Could not found the client, maybe it's deleted.");
            }
            UpdateGroupInput view = new UpdateGroupInput();
            view.Id = @tax.Id;
            view.Description = @tax.Description;
            view.Name = @tax.Name;
            view.TenantId = @tax.TenantId;
            //return Mapper.Map<UpdateTaxInput>(@tax);
            return view;
        }

        public void Update(UpdateGroupInput input)
        {
            var @group = _groupRepository.Get(input.Id);
            if (@group == null)
            {
                throw new UserFriendlyException("Could not found the group, maybe it's deleted.");
            }

            @group.Name = input.Name;
            @group.Description = input.Description;

        }

        public IList<User> GetAllUser()
        {
            var users = _userManager.Users.ToList();
            return users;
        }

        public IList<ClientGroup> GetAllClientGroups()
        {
            var clientGroups = _clientGroupRepository.GetAllList();
            return clientGroups;
        }

        public bool isAllowedDelete(Guid Id)
        {
            var incoiceList = _clientGroupRepository.GetAll().Where(a => a.GroupId == Id).Count();
            return (incoiceList <= 0)? true: false;


        }
    }
}
