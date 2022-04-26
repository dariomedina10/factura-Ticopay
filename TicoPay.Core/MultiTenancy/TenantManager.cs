using System.Collections.Generic;
using System.Linq;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using TicoPay.Authorization.Roles;
using TicoPay.Editions;
using TicoPay.Users;
using System;
using Abp.UI;
using Abp.Domain.Uow;
using Abp.Dependency;
using TicoPay.Services;

namespace TicoPay.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, User>, ITransientDependency
    {
        private readonly IRepository<AgreementConectivity, int> _agreementRepository;
        private readonly IRepository<ClientService, Guid> _clienteServiceRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public TenantManager(
            IRepository<Tenant> tenantRepository,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            EditionManager editionManager,
            IAbpZeroFeatureValueStore featureValueStore, IRepository<AgreementConectivity, int> agreementRepository, IRepository<ClientService, Guid> clienteServiceRepository,
            IUnitOfWorkManager unitOfWorkManager
            )
            : base(
                tenantRepository,
                tenantFeatureRepository,
                editionManager,
                featureValueStore
            )
        {
            _agreementRepository = agreementRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _clienteServiceRepository = clienteServiceRepository;
        }

        public List<Tenant> GetActiveTenants(bool? generatesInvoiceSetting = null)
        {
            var query = TenantRepository.GetAll().Where(t => t.IsActive);

            if (generatesInvoiceSetting.HasValue)
            {
                query.Where(t => t.IsActive == generatesInvoiceSetting);
            }
            return query.ToList();
        }

        public List<Tenant> GetTenantsActiveByClient(string consecutiveNumber)
        {
            var query = TenantRepository.GetAll().Where(t => t.IdentificationNumber == consecutiveNumber && t.IsActive && (t.MotiveSuspension != Tenant.InactiveReason.inactive || t.MotiveSuspension != Tenant.InactiveReason.InvoicePending));
            return query.ToList();
        }

        public List<Tenant> GetActiveTenantsWithRecurringInvoice(bool? generatesInvoiceSetting = null)
        {
            var query = TenantRepository.GetAll().Where(t => t.IsActive && t.Id != 1 && t.FirmaRecurrente!=null && ((t.ValidateHacienda && t.UserTribunet!=null && t.PasswordTribunet != null) || (!t.ValidateHacienda)));
            List<int> tenantsId = (List<int>)query.Select(d => d.Id ).ToList();
            var clientServices = _clienteServiceRepository.GetAllList(d => tenantsId.Contains(d.TenantId)).GroupBy(d => d.TenantId, (key, group) => new { key }).ToList();
            List<int> tenantsdIdClientServices = new List<int>();
            foreach (var item in clientServices)
            {
                tenantsdIdClientServices.Add(item.key);
            }
            return query.Where(d=> tenantsdIdClientServices.Contains(d.Id)).ToList();
        }

        public Tenant Get(int id)
        {
            var @tenant = TenantRepository.FirstOrDefault(id);

            //var @tenant = TenantRepository.GetAll().Where(a => a.Id == id).i.FirstOrDefault();


            if (@tenant == null)
            {
                throw new UserFriendlyException("Could not found the Tenant, may be it's deleted!");
            }
            return @tenant;
        }

        public Tenant GetByName(string tenancyName)
        {
            var tenant = TenantRepository.GetAll().Where(t => t.TenancyName == tenancyName).FirstOrDefault();
            return tenant;
        }
        /// <summary>
        /// Obatiene los tenant configurados en ese puerto -- banco
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        [UnitOfWork]
        public List<AgreementConectivity> GetTenantsAgreementByPort(int port)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant); // deshabilitar el filtro por tenant

            return _agreementRepository.GetAllList(a => a.Port == port).ToList();
        }

        [UnitOfWork]
        public List<AgreementConectivity> GetTenantsAgreement()
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant); // deshabilitar el filtro por tenant

            return _agreementRepository.GetAll().ToList();
        }
        /// <summary>
        /// Obtiene los tipos de acceso - banco
        /// </summary>
        /// <param name="listTenant"></param>
        /// <returns></returns>
        [UnitOfWork]
        public TipoLLaveAcceso GetTenantsKeyType(List<int> listTenant)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant); // deshabilitar el filtro por tenant

            var AgreementConId = listTenant[0];
            var agreementCon = _agreementRepository.FirstOrDefault(a => a.Id == AgreementConId);
            if (agreementCon!=null)
            {
                return agreementCon.KeyType;
            }
            throw new UserFriendlyException("No posee llave configurada.");
        }

    }
}