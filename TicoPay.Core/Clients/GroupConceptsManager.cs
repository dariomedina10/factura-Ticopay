using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Events.Bus;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace TicoPay.Clients
{
    public class GroupConceptsManager : DomainService, IGroupConceptsManager
    {
        private readonly IRepository<GroupConcepts, Guid> _groupConceptsRepository;
        private readonly IRepository<ClientGroupConcept, Guid> _clientGroupConceptsRepository;

        public IEventBus EventBus { get; set; }

        public GroupConceptsManager(IRepository<GroupConcepts, Guid> groupConceptsRepository, IRepository<ClientGroupConcept, Guid> clientGroupConceptsRepository)
        {
            _groupConceptsRepository = groupConceptsRepository;
            EventBus = NullEventBus.Instance;
            _clientGroupConceptsRepository = clientGroupConceptsRepository;
        }

        public void Create(GroupConcepts groupConcepts)
        {
            _groupConceptsRepository.Insert(groupConcepts);
        }

        public GroupConcepts Get(Guid id)
        {
            var groupConcepts = _groupConceptsRepository.FirstOrDefault(id);
            if (groupConcepts == null)
            {
                throw new UserFriendlyException("Grupo de Servicios no encontrado.");
            }
            return groupConcepts;
        }

        public IList<ClientGroupConcept> GetAllBillingGroupConcepts(GroupConcepts group)
        {
            try
            {
                var clientServices = _clientGroupConceptsRepository.GetAll().Where(a => a.GroupId == group.Id && a.IsDeleted == false).Include(a => a.Group.Services);
                return clientServices.ToList();
            }
            catch (Exception)
            {

                return null;
            }
            
        }
    }
}
