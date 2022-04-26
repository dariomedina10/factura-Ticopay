using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Events.Bus;
using TicoPay.Taxes;

namespace TicoPay.Services
{
    public class ServiceManager : DomainService, IServiceManager
    {
        public IEventBus EventBus { get; set; }
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<Tax, Guid> _taxRepository;
        private readonly IRepository<ClientService, Guid> _clientServiceRepository;


        public ServiceManager(IRepository<Service, Guid> serviceRepository, IRepository<ClientService, Guid> clientServiceRepository, IRepository<Tax, Guid> taxRepository)
        {
            _serviceRepository = serviceRepository;
            _clientServiceRepository = clientServiceRepository;
            _taxRepository = taxRepository;
            EventBus = NullEventBus.Instance;
        }

        //public ServiceManager

        public void AssignTaxToService(Tax tax, Service service)
        {
            if (tax == null || service.TaxId == tax.Id)
            {
                return;
            }
            service.ChangeTax(tax);
        }

        public void UpdateClientService(ClientService cService)
        {
            _clientServiceRepository.Update(cService);
        }

        public IList<ClientService> GetAllBillingServices(Service service)
        {
            var clientServices = _clientServiceRepository.GetAll().Where(a => a.ServiceId == service.Id && a.IsDeleted == false && a.Client != null).Include(a => a.Client).Include(a => a.Service.Tax);
            return clientServices.ToList();
        }

        public IList<Tax> GetAllListTaxes()
        {
            return _taxRepository.GetAll().Where(a => a.IsDeleted == false).ToList();
        }

        public Service Get(Guid ServiceId)
        {
            return _serviceRepository.GetAll().Include(a => a.Tax).Where(a => a.Id == ServiceId).FirstOrDefault();
        }

    }
}
