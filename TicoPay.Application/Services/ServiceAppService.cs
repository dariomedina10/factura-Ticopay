using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using AutoMapper;
using PagedList;
using TicoPay.Services.Dto;
using TicoPay.Taxes;
using TicoPay.Users;
using TicoPay.MultiTenancy;
using Abp.Domain.Uow;
using System.Linq.Expressions;
using TicoPay.Invoices;

namespace TicoPay.Services
{
    public class ServiceAppService : ApplicationService, IServiceAppService
    {
        //These members set in constructor using constructor injection.
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<Tax, Guid> _taxRepository;
        private readonly IRepository<ClientService, Guid> _clientServiceRepository;

        private readonly IServiceManager _serviceManager;
        public readonly UserManager _userManager;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly TenantManager _tenantManager;

        private readonly IRepository<InvoiceLine, Guid> _invoiceLineRepository;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public ServiceAppService(IRepository<Service, Guid> serviceRepository, IRepository<Tax, Guid> taxRepository, TenantManager tenantManager, IRepository<Tenant, int> tenantRepository,
            IServiceManager serviceManager, UserManager userManager, IRepository<InvoiceLine, Guid> invoiceLinesRepository, IRepository<ClientService, Guid> clientServiceRepository)
        {
            _serviceRepository = serviceRepository;
            _serviceManager = serviceManager;
            _taxRepository = taxRepository;
            _clientServiceRepository = clientServiceRepository;
            _userManager = userManager;
            _tenantManager = tenantManager;
            _tenantRepository = tenantRepository;
            _invoiceLineRepository = invoiceLinesRepository;
        }

        //public ServiceAppService()
        //{
        //    _serviceRepository = new ServiceRepository();
        //    _serviceManager = serviceManager;
        //    _taxRepository = taxRepository;
        //    _clientServiceRepository = clientServiceRepository;
        //    _userManager = userManager;
        //}
        [UnitOfWork]
        public ServiceDto Create(CreateServiceInput input)
        {
            decimal temp = Convert.ToDecimal(input.Price);
            var currentTenant =  _tenantManager.Get(AbpSession.GetTenantId());

            Service @service = Service.Create(AbpSession.GetTenantId(), input.Name, temp, input.CronExpression, input.UnitMeasurement.Value, input.UnitMeasurementOthers, input.IsRecurrent, input.Quantity, input.DiscountPercentage);
            if (input.TaxId != null)
            {
                Tax @tax = _taxRepository.Get(input.TaxId.Value);
                _serviceManager.AssignTaxToService(@tax, @service);
            }
            _serviceRepository.Insert(@service);

            if (!currentTenant.IsTutotialServices) { 
                currentTenant.IsTutotialServices = true;
                currentTenant.IsTutorialProduct = true;
                _tenantRepository.Update(currentTenant);
            }

            return Mapper.Map<ServiceDto>(@service);
        }

        [UnitOfWork]
        [Abp.Runtime.Validation.DisableValidation]
        public ServiceDto Create(ServiceDto input)
        {
            decimal temp = Convert.ToDecimal(input.Price);
            //var currentTenant = _tenantManager.Get(AbpSession.GetTenantId());

            Service @service = Service.Create(AbpSession.GetTenantId(), input.Name, temp, input.CronExpression, input.UnitMeasurement, input.UnitMeasurementOthers, input.IsRecurrent, input.Quantity, input.DiscountPercentage);
            if (input.TaxId != null)
            {
                Tax @tax = _taxRepository.Get(input.TaxId.Value);
                _serviceManager.AssignTaxToService(@tax, @service);
            }
            _serviceRepository.Insert(@service);

            return Mapper.Map<ServiceDto>(@service);


        }

        public ListResultDto<ServiceDto> GetServices()
        {
            var services = _serviceRepository.GetAll().Include(a => a.Tax).ToList();

            if (services == null)
            {
                throw new UserFriendlyException("Could not found the services, maybe it's deleted.");
            }
            return new ListResultDto<ServiceDto>(services.MapTo<List<ServiceDto>>());
        }


        public IList<Service> GetServicesEntities()
        {
            var services = _serviceRepository.GetAll().Where(a => a.IsDeleted == false).Include(a => a.Tax).ToList();

            if (services == null)
            {
                throw new UserFriendlyException("Could not found the services, maybe it's deleted.");
            }
            return services;
        }

        public IList<Service> GetServicesEntities(int TenantId)
        {
            var services = _serviceRepository.GetAll().Where(a => a.IsDeleted == false && a.TenantId == TenantId).Include(a => a.Tax).ToList();

            if (services == null)
            {
                throw new UserFriendlyException("Could not found the services, maybe it's deleted.");
            }
            return services;
        }


        public ServiceDetailOutput GetDetail(Guid input)
        {
            var @service = _serviceRepository.GetAll().Include(a=>a.Tax).Where(a=>a.Id==input).FirstOrDefault();

            if (@service == null)
            {
                throw new UserFriendlyException("Could not found the service, maybe it's deleted.");
            }

            return @service.MapTo<ServiceDetailOutput>();
        }

        public ServiceDto Get(Guid input)
        {
            try
            {
                var @service = _serviceRepository.GetAll().Include(a => a.Tax).Where(a => a.Id == input).First();               
                               
                return Mapper.Map<ServiceDto>(@service);
            }
            catch 
            {

                return null;
            }
           
        }

        public UpdateServiceInput GetEdit(Guid input)
        {
            var @service = _serviceRepository.GetAll().Include(a => a.Tax).Where(a => a.Id == input).ToList().FirstOrDefault();
            if (@service == null)
            {
                throw new UserFriendlyException("Could not found the service, maybe it's deleted.");
            }
            return Mapper.Map<UpdateServiceInput>(@service);
        }

        /// <summary>
        /// Searches for services and returns page result
        /// </summary>
        /// <param name="searchInput"></param>
        /// <returns></returns>
        public IPagedList<ServiceDto> SearchServices(SearchServicesInput searchInput)
        {
            if (searchInput.NameFilter == null || searchInput.NameFilter == "")
                searchInput.NameFilter = "";
            else
                searchInput.NameFilter = searchInput.NameFilter.ToLower();

            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value  : 1;

            var services = _serviceRepository.GetAll()
                    .Include(a => a.Tax)
                    .Where(c => c.Name.ToLower().Contains(searchInput.NameFilter)
                    || c.Name.ToLower().Equals(searchInput.NameFilter));

            if (searchInput.PriceSinceFilter != null)
                services = services.Where(c => c.Price >= searchInput.PriceSinceFilter);

            if (searchInput.PriceUntilFilter != null)
                services = services.Where(c => c.Price <= searchInput.PriceUntilFilter);

            if (searchInput.TaxId != null)
                services = services.Where(c => c.TaxId == searchInput.TaxId);

            if (searchInput.RecurrentId != null)
            {
                bool recurrentId = (searchInput.RecurrentId == 0) ? true : false;
                services = services.Where(c => c.IsRecurrent == recurrentId);
            }

            if (services == null)
            throw new UserFriendlyException("Could not found the services, maybe it's deleted.");

            return services.MapTo<List<ServiceDto>>().OrderBy(p => p.Name).ToList().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
        }


        public IPagedList<ServiceDto> getServices(SearchServicesInput searchInput)
        {
            if (searchInput.NameFilter == null || searchInput.NameFilter == "")
                searchInput.NameFilter = "";
            else
                searchInput.NameFilter = searchInput.NameFilter.ToLower();

            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value : 1;

            var services = _serviceRepository.GetAll()
                    .Include(a => a.Tax)
                    .Where(c => c.Name.ToLower().Contains(searchInput.NameFilter)
                    || c.Name.ToLower().Equals(searchInput.NameFilter));

            if (searchInput.PriceSinceFilter != null)
                services = services.Where(c => c.Price >= searchInput.PriceSinceFilter);

            if (searchInput.PriceUntilFilter != null)
                services = services.Where(c => c.Price <= searchInput.PriceUntilFilter);

            if (searchInput.TaxId != null)
                services = services.Where(c => c.TaxId == searchInput.TaxId);

            if (searchInput.RecurrentId != null)
            {
                bool recurrentId = (searchInput.RecurrentId == 0) ? true : false;
                services = services.Where(c => c.IsRecurrent == recurrentId);
            }

            var listervice = (from s in services select new ServiceDto
            {
                Id =s.Id,
                Name =s.Name,
                CronExpression = s.CronExpression,
                IsRecurrent =  s.IsRecurrent,
                Price= s.Price,              
                TaxId=s.TaxId,
                Tax= new Taxes.Dto.TaxDto { Id= s.Tax.Id, Name= s.Tax.Name, Rate=s.Tax.Rate, TaxTypes=s.Tax.TaxTypes},
                UnitMeasurement=s.UnitMeasurement,
                UnitMeasurementOthers=s.UnitMeasurementOthers,

                }).OrderBy(p => p.Name).ToPagedList(currentPageIndex, searchInput.MaxResultCount);
            return listervice;
        }

        public void Update(UpdateServiceInput input)
        {
            var @service = _serviceRepository.Get(input.Id);
            if (@service == null)
            {
                throw new UserFriendlyException("Could not found the service, may be it's deleted.");
            }

            if (!input.IsRecurrent)
                input.CronExpression = null;

            @service.Name = input.Name;
            decimal temp = Convert.ToDecimal(input.Price);
            @service.Price = temp;
            @service.UnitMeasurement = input.UnitMeasurement;
            @service.UnitMeasurementOthers = input.UnitMeasurementOthers;
            @service.CronExpression = input.CronExpression;
            service.IsRecurrent = input.IsRecurrent;
            @service.Quantity = input.Quantity;
            @service.DiscountPercentage = input.DiscountPercentage;

            if (input.TaxId != null)
            {
                Tax @tax = _taxRepository.Get(input.TaxId.Value);
                _serviceManager.AssignTaxToService(@tax, @service);
            }
            _serviceRepository.Update(@service);
        }

        public void Update(ServiceDto input)
        {
            var @service = _serviceRepository.Get(input.Id);
            if (@service == null)
            {
                throw new UserFriendlyException("Could not found the service, may be it's deleted.");
            }

            if (!input.IsRecurrent)
                input.CronExpression = null;

            @service.Name = input.Name;
            decimal temp = Convert.ToDecimal(input.Price);
            @service.Price = temp;
            @service.UnitMeasurement = input.UnitMeasurement;
            @service.UnitMeasurementOthers = input.UnitMeasurementOthers;
            @service.CronExpression = input.CronExpression;
            @service.IsRecurrent = input.IsRecurrent;
            @service.Quantity = input.Quantity;
            @service.DiscountPercentage = input.DiscountPercentage;

            if (input.TaxId != null)
            {
                Tax @tax = _taxRepository.Get(input.TaxId.Value);
                _serviceManager.AssignTaxToService(@tax, @service);
            }
            _serviceRepository.Update(@service);
        }

        public void Delete(Guid input)
        {
            var @service = _serviceRepository.Get(input);
            if (@service == null)
            {
                throw new UserFriendlyException("Could not found the service, maybe it's deleted.");
            }

            @service.IsDeleted = true;
            _serviceRepository.Update(@service);
        }

        public bool isAllowedDelete(Guid Id)
        {          

            var ClientServiceList = _clientServiceRepository.GetAll().Where(a=>a.Id== Id);
            if (ClientServiceList.Count() <= 0)
                return true;
            else
                return false;


        }

        public IList<Tax> GetAllTaxes()
        {
            return _serviceManager.GetAllListTaxes();
        }

        public IList<User> GetAllUser()
        {
            var users = _userManager.Users.ToList();
            if (users == null)
            {
                throw new UserFriendlyException("Could not found the users, maybe it's deleted.");
            }
            return users;
        }

        public IList<ClientService> GetAllClientServices()
        {
            var clientServices = _clientServiceRepository.GetAllList();
            if (clientServices == null)
            {
                throw new UserFriendlyException("Could not found the Client Services, maybe it's deleted.");
            }
            return clientServices;
        }

        [UnitOfWork]
        public Service CreateServiceInvoice(CreateServiceInput input)
        {
            decimal temp = Convert.ToDecimal(input.Price);
            var currentTenant = _tenantManager.Get(AbpSession.GetTenantId());

            Service @service = Service.Create(AbpSession.GetTenantId(), input.Name, temp, input.CronExpression, input.UnitMeasurement.Value, input.UnitMeasurementOthers, input.IsRecurrent, input.Quantity,input.DiscountPercentage);
            if (input.TaxId != null)
            {
                Tax @tax = _taxRepository.Get(input.TaxId.Value);
                _serviceManager.AssignTaxToService(@tax, @service);
            }
            _serviceRepository.Insert(@service);

            if (!currentTenant.IsTutotialServices)
            {
                currentTenant.IsTutotialServices = true;
                _tenantRepository.Update(currentTenant);
            }

            return @service;
        }

        public Service GetBy(Expression<Func<Service, bool>> predicate)
        {
            return _serviceRepository.GetAll().Where(predicate).FirstOrDefault();
        }

        public IList<InvoiceLine> invoiceLineRepository()
        {
            var tenantId = _tenantManager.Get(AbpSession.GetTenantId());
            var invoicesLines =
                _invoiceLineRepository.GetAll().Include(a => a.Service);
            return invoicesLines.ToList();
        }
    }
}
