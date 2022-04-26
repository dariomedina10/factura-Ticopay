using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using AutoMapper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.GroupConcept.Dto;
using TicoPay.Services;
using TicoPay.Users;

namespace TicoPay.GroupConcept
{
    public class GroupConceptsAppService : ApplicationService, IGroupConceptsAppService
    {
        private readonly IRepository<GroupConcepts, Guid> _groupRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IGroupConceptsManager _groupConceptsManager;
        private readonly UserManager _userManager;

        public GroupConceptsAppService(IRepository<GroupConcepts, Guid> groupConceptsRepository, IRepository<Service, Guid> serviceRepository, IGroupConceptsManager groupConceptsManager, UserManager userManager)
        {
            _groupRepository = groupConceptsRepository;
            _serviceRepository = serviceRepository;
            _groupConceptsManager = groupConceptsManager;
            _userManager = userManager;
        }

        public GroupConceptsDto Create(CreateGroupConceptsInput input)
        {
            var groupConcepts = GroupConcepts.Create(AbpSession.GetTenantId(), input.Name, input.Description, input.CronExpression);
            UpdateService(input.Services, groupConcepts);
            _groupConceptsManager.Create(groupConcepts);
            return Mapper.Map<GroupConceptsDto>(groupConcepts);
        }
       

        public void Delete(Guid input)
        {
            var groupConcepts = _groupRepository.Get(input);
            if (groupConcepts == null)
            {
                throw new UserFriendlyException("Grupo de concepto no encontrado.");
            }

            groupConcepts.IsDeleted = true;
            _groupRepository.Update(groupConcepts);
        }

        public ListResultDto<GroupConceptsDto> GetGroupConcepts()
        {
            var groups = _groupRepository.GetAllList();
            return new ListResultDto<GroupConceptsDto>(groups.MapTo<List<GroupConceptsDto>>());
        }

        public GroupConceptsDetailOutput GetDetail(Guid id)
        {
            var groupConcepts = _groupRepository.GetAll()
                .Where(i => i.Id == id)
                .Include(s => s.Services)
                .FirstOrDefault();
            if (groupConcepts == null)
            {
                throw new UserFriendlyException("Grupo de concepto no encontrado.");
            }

            var result = groupConcepts.MapTo<GroupConceptsDetailOutput>();
            result.UpdateAuditInfoIfNecesary(_userManager, groupConcepts);
            return result;
        }

        public GroupConceptsDto Get(Guid id)
        {
            try
            {
                var groupConcepts = _groupRepository.GetAll()
                .Where(i => i.Id == id)
                .Include(s => s.Services)
                .FirstOrDefault();               
                return Mapper.Map<GroupConceptsDto>(groupConcepts);
            }
            catch 
            {
                return null;
            }
            
        }

        public IPagedList<GroupConceptsDto> SearchGroupConcepts(SearchGroupConceptsInput searchInput)
        {
             if (searchInput.Query != "")
                searchInput.Query = searchInput.Query.ToLower();

            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value  : 1;

            var groups = _groupRepository.GetAll()
                .Where(c =>
                    c.Name.ToLower().Contains(searchInput.Query) ||
                    c.Description.ToLower().Contains(searchInput.Query) ||
                    c.Name.ToLower().Equals(searchInput.Query))
                .OrderByDescending(p => p.Name).ToList();

            return groups.MapTo<List<GroupConceptsDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
        }

        public UpdateGroupConceptsInput GetEdit(Guid id)
        {
            var groupConcepts = _groupRepository.GetAll()
                .Where(i => i.Id == id)
                .Include(s => s.Services)
                .FirstOrDefault();
            if (groupConcepts == null)
            {
                throw new UserFriendlyException("Grupo de concepto no encontrado.");
            }
            UpdateGroupConceptsInput view = new UpdateGroupConceptsInput();
            view.Id = groupConcepts.Id;
            view.Description = groupConcepts.Description;
            view.CronExpression = groupConcepts.CronExpression;
            view.Name = groupConcepts.Name;
            view.TenantId = groupConcepts.TenantId;
            view.Services = groupConcepts.Services.Select(s => s.Id.ToString()).ToList();
            view.UpdateAuditInfoIfNecesary(_userManager, groupConcepts);
            return view;
        }

        public void Update(UpdateGroupConceptsInput input)
        {
            var groupConcepts = _groupRepository.GetAll()
                .Where(i => i.Id == input.Id)
                .Include(s => s.Services)
                .FirstOrDefault();
            if (groupConcepts == null)
            {
                throw new UserFriendlyException("Grupo de concepto no encontrado.");
            }
            UpdateService(input.Services, groupConcepts);
            groupConcepts.Name = input.Name;
            groupConcepts.Description = input.Description;
            groupConcepts.CronExpression = input.CronExpression;
            input.UpdateAuditInfoIfNecesary(_userManager, groupConcepts);
        }
      

        private void UpdateService(List<string> services, GroupConcepts groupConcepts)
        {
            if (services != null)
            {
                groupConcepts.Services.Clear();
                foreach (var serviceId in services)
                {
                    Service service = _serviceRepository.Get(Guid.Parse(serviceId));
                    if (service != null)
                    {
                        groupConcepts.Services.Add(service);
                    }
                }
            }
        }

        public IList<GroupConcepts> GetGroupConceptsEntities()
        {
            return _groupRepository.GetAll()
                .Include(s => s.Services)
                .Include(s => s.Clients)
                .ToList();
        }

        public IList<GroupConcepts> GetGroupConceptsEntities(int tenantId)
        {
            return _groupRepository.GetAll().Where(s=>s.TenantId == tenantId)
                .Include(s => s.Services)              
                .ToList();
        }

        public bool isAllowedDelete(Guid Id)
        {
            var isdelete = false;
            var ClientList = _groupRepository.GetAll().Include(s => s.Clients).Where(a => a.Id == Id).FirstOrDefault();
            if (ClientList != null)
                isdelete = ClientList.Clients != null ? (ClientList.Clients.Count() <= 0) : true;

            return isdelete;


        }
    }
}
