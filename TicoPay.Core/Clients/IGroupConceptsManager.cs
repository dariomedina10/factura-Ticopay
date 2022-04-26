using System;
using Abp.Domain.Services;
using TicoPay.MultiTenancy;
using System.Collections.Generic;

namespace TicoPay.Clients
{
    public interface IGroupConceptsManager : IDomainService
    {
        GroupConcepts Get(Guid id);

        void Create(GroupConcepts client);
        IList<ClientGroupConcept> GetAllBillingGroupConcepts(GroupConcepts group);
    }
}
