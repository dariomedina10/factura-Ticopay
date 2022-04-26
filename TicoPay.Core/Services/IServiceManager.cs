using System.Collections.Generic;
using Abp.Domain.Services;
using TicoPay.Taxes;
using System;

namespace TicoPay.Services
{
    public interface IServiceManager : IDomainService
    {
        void AssignTaxToService(Tax tax, Service service);
        IList<Tax> GetAllListTaxes();
        IList<ClientService> GetAllBillingServices(Service service);
        void UpdateClientService(ClientService cService);
        Service Get(Guid ServiceId);
    }
}


