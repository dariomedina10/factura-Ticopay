using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TicoPay.Common;
using TicoPay.Editions;
using TicoPay.General;
using TicoPay.MultiTenancy;
using TicoPay.Services;


namespace TicoPay.Clients
{
    public class ClientManager : DomainService, IClientManager
    {

        public IEventBus EventBus { get; set; }
        private readonly IRepository<Client, Guid> _clientRepository;
        private readonly IRepository<Group, Guid> _groupRepository;
        private readonly IRepository<ClientService, Guid> _clientServiceRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<ClientGroup, Guid> _clientGroupRepository;
        private readonly IRepository<ClientGroupConcept, Guid> _clientGroupConceptRepository;
        private readonly IRepository<GroupConcepts, Guid> _groupConceptsRepository;
        // private readonly ICodeGenerator _codeGenerator;
        private readonly IRepository<Provincia, int> _provinceRepository;
        private readonly IRepository<Canton, int> _cantonRepository;
        private readonly IRepository<Distrito, int> _distritoRepository;
        private readonly IRepository<Barrio, int> _barrioRepository;
        private readonly IRepository<Country, int> _countryRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ClientManager(IRepository<Client, Guid> clientRepository, IRepository<Group, Guid> groupRepository,
            IRepository<ClientService, Guid> clientServiceRepository,
            IRepository<ClientGroup, Guid> clientGroupRepository, IRepository<Service, Guid> serviceRepository,
            IRepository<GroupConcepts, Guid> groupConceptsRepository, IRepository<Provincia, int> provinceRepository,
            IRepository<Canton, int> cantonRepository, IRepository<Distrito, int> distritoRepository, IRepository<Barrio, int> barrioRepository,
            IRepository<Country, int> countryRepository, IUnitOfWorkManager unitOfWorkManager, IRepository<ClientGroupConcept, Guid> clientGroupConceptRepository, IRepository<Tenant, int> tenantRepository)
        {
            _clientRepository = clientRepository;
            _countryRepository = countryRepository;
            _groupRepository = groupRepository;
            _clientServiceRepository = clientServiceRepository;
            EventBus = NullEventBus.Instance;
            _clientGroupRepository = clientGroupRepository;
            _serviceRepository = serviceRepository;
            //_codeGenerator = codeGenerator;
            _clientGroupConceptRepository = clientGroupConceptRepository;
            _groupConceptsRepository = groupConceptsRepository;
            _provinceRepository = provinceRepository;
            _cantonRepository = cantonRepository;
            _distritoRepository = distritoRepository;
            _barrioRepository = barrioRepository;
            _tenantRepository = tenantRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void Create(Client @client)
        {
            var insertclients = _clientRepository.Insert(@client);

            if (insertclients == null)
                throw new UserFriendlyException("Hubo un error creando el cliente");
        }

        public Client Get(Guid id)
        {
            var @client = _clientRepository.FirstOrDefault(id);
            if (@client == null)
            {
                throw new UserFriendlyException("Could not found the client, maybe it's deleted!");
            }
            return @client;
        }

        public Service GetService(Guid id)
        {
            var @client = _serviceRepository.FirstOrDefault(id);
            if (@client == null)
            {
                throw new UserFriendlyException("Could not found the client, maybe it's deleted!");
            }
            return @client;
        }


        public void AddServiceToClient(Service service, Client client, string cronExpresion,
            DateTime initDateTime, bool generateInvoice, bool allowLatePayment, decimal quantity, decimal discountPercentage)
        {
            if (service == null || client == null)
                throw new UserFriendlyException("Entity is null!");
           
            client.AssignService(service, cronExpresion, initDateTime, generateInvoice, allowLatePayment, quantity, discountPercentage);
            //foreach (var item in service)
            //{
            //    var serv = GetService(item.Id);
            //    client.AssignService(serv, cronExpresion, initDateTime, generateInvoice, allowLatePayment,item.Quantity,item.DiscountPercentage);
            //}
        }

        public void AddServiceToClient(Service service, Client client, string cronExpresion,
    DateTime initDateTime, bool generateInvoice, bool allowLatePayment, decimal quantity, decimal discountPercentage, ClientServiceState clientServiceState)
        {
            if (service == null || client == null)
                throw new UserFriendlyException("Entity is null!");

            client.AssignService(service, cronExpresion, initDateTime, generateInvoice, allowLatePayment, quantity, discountPercentage, clientServiceState);
            //foreach (var item in service)
            //{
            //    var serv = GetService(item.Id);
            //    client.AssignService(serv, cronExpresion, initDateTime, generateInvoice, allowLatePayment,item.Quantity,item.DiscountPercentage);
            //}
        }



        //public void ChangeCode(Client @client, Tenant tenant, string code = null)
        //{
        //    @client.ChangeCode(tenant, _codeGenerator, code);
        //}


        public void AssignGroupToClient(IList<Group> groups, Client client)
        {
            foreach (var item in groups)
            {
                ClientGroup clientGroup = ClientGroup.Create(item, client);
                _clientGroupRepository.Insert(clientGroup);
            }
        }

        public void AssignGroupConceptsToClient(GroupConcepts groups, Client client, decimal quantity, decimal discountPercentage)
        {
            //foreach (var item in groups)
            //{
            //    ClientGroupConcept clientGroup = ClientGroupConcept.Create(item, client);
            //    _clientGroupConceptRepository.Insert(clientGroup);
            //}
            if (groups == null || client == null)
                throw new UserFriendlyException("Entity is null!");

            client.AssignGroupConcept(groups, quantity,  discountPercentage);
           
        }

        public void UpdateGroupToClient(IList<Group> groups, Client client)
        {
            foreach (var item in groups)
            {
                ClientGroup clientGroup = ClientGroup.Create(item, client);
                _clientGroupRepository.Insert(clientGroup);
            }
        }


        public IList<Group> GetAllListGroups()
        {
            return _groupRepository.GetAllList();
        }
        public IList<Country> GetAllListCountries()
        {
            return _countryRepository.GetAll().OrderBy(d => d.CountryName).ToList();
        }

        /// <summary>
        /// Obtiene todos los Grupo de Servicios creados
        /// </summary>
        /// <returns></returns>
        public IList<GroupConcepts> GetAllListGroupsConcepts()
        {
            return _groupConceptsRepository.GetAllList();
        }

        public IList<ClientGroupConcept> GetAllListGroupsConceptsClient(Guid id)
        {
            IList<ClientGroupConcept> temp = new List<ClientGroupConcept>();
            temp = _clientGroupConceptRepository.GetAll().Where(a => a.ClientId == id).ToList();
            return temp;
        }
        public IList<Service> GetAllListService()
        {
            return _serviceRepository.GetAllList();
        }
        /// <summary>
        /// Obtiene el listado de todas las provincias
        /// </summary>
        /// <returns></returns>
        public IList<Provincia> GetAllListProvince()
        {
            IList<Provincia> temp = new List<Provincia>();
            temp = _provinceRepository.GetAll().OrderBy(d => d.NombreProvincia).ToList();
            temp.Insert(0, new Provincia() { NombreProvincia = "Seleccione una Provincia ", Id = 0 });

            return temp;
        }
        /// <summary>
        /// Obtiene el listado de los cantones segun el id de provincia
        /// </summary>
        /// <returns></returns>
        public IList<Canton> GetAllListCantonByProvince(int? id)
        {
            return _cantonRepository.GetAll().Where(a => a.ProvinciaID == id).OrderBy(d => d.NombreCanton).ToList();
        }
        /// <summary>
        /// Obtiene el Id de Distrito segun el id del barrio
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GeDistritoByBarrio(int id)
        {
            // return 
            var barrio = _barrioRepository.FirstOrDefault(a => a.Id == id);

            return barrio.DistritoID;
        }
        /// <summary>
        /// Obtiene el Id de canton segun el Id de Distrito
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetCantonByDistrito(int id)
        {

            var canton = _distritoRepository.FirstOrDefault(a => a.Id == id);

            return canton.CantonID;
        }
        /// <summary>
        /// Obtiene el id de la provincia segun el id del canton
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetIdProvinceByIdCanton(int id)
        {

            var province = _cantonRepository.FirstOrDefault(a => a.Id == id);

            return province.ProvinciaID;
        }
        /// <summary>
        /// Obtiene el listado de distritos segun el id del canton
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IList<Distrito> GetAllListDistritosByCanton(int? id)
        {
            return _distritoRepository.GetAll().Where(a => a.CantonID == id).OrderBy(d => d.NombreDistrito).ToList();
        }

        public IList<Barrio> GetAllListBarriosByDistrito(int? id)
        {
            return _barrioRepository.GetAll().Where(a => a.DistritoID == id).OrderBy(d => d.NombreBarrio).ToList();
        }

        /// <summary>
        /// Obtiene todos los Grupo de Servicios asociados a un cliente
        /// </summary>
        /// <param name="id">el id del cliente</param>
        /// <returns></returns>
        public IList<ClientGroup> GetAllListGroupsClient(Guid id)
        {
            IList<ClientGroup> temp = new List<ClientGroup>();
            temp = _clientGroupRepository.GetAll().Where(a => a.ClientId == id).ToList();
            return temp;
        }

        //public IList<ClientGroupConcept> GetAllListGroupsConceptsClient(Guid id)
        //{
        //    IList<ClientGroupConcept> temp = new List<ClientGroupConcept>();
        //    temp = _clientGroupConceptRepository.GetAll().Where(a => a.ClientId == id).ToList();
        //    return temp;
        //}

        public IList<ClientService> GetAllListClientService(Guid id)
        {
            IList<ClientService> temp = new List<ClientService>();
            temp = _clientServiceRepository.GetAll().Where(a => a.ClientId == id).ToList();
            return temp;
        }

        public IList<ClientService> GetAllListClientServiceAdjustment(Guid id)
        {
            var AdjustmentRangeLimit = DateTimeZone.Now().AddMonths(-1);
            var now = DateTimeZone.Now();
            IList<ClientService> temp = new List<ClientService>();
            temp = _clientServiceRepository.GetAll().Where(a => a.ClientId == id && (a.CreationTime >= AdjustmentRangeLimit && a.CreationTime <= now && a.State == ClientServiceState.Adjustment)).OrderByDescending(d=>d.CreationTime).ToList();
            return temp;
        }



        public bool ExistIdentification(int tenantId, string identification)
        {
            var @entity = _clientRepository.FirstOrDefault(e => e.TenantId == tenantId && e.Identification == identification);
            return @entity != null;
        }

        //public bool ExistCode(long code)
        //{
        //    var @entity = _clientRepository.FirstOrDefault(e => e.Code == code );
        //    return @entity != null;
        //}

        public bool ExistIdentificationEdit(string identification, Guid id)
        {
            var @entity = _clientRepository.FirstOrDefault(e => e.Identification == identification && e.Id != id);
            return @entity != null;
        }

        public bool ExistIdentificationExt(int tenantId, string identification, Guid? id)
        {
            var @entity = _clientRepository.FirstOrDefault(e => e.IdentificacionExtranjero == identification && e.TenantId == tenantId && e.Id != id);
            return @entity != null;
        }
        /// <summary>
        /// Verifica si el Cliente Existe y devuelve el id - banco
        /// </summary>
        /// <param name="identification"></param>
        /// <param name="listTenant"></param>
        /// <returns></returns>
        [UnitOfWork]
        public Client ExistCodeClient(long code)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant); // deshabilitar el filtro por tenant
            return _clientRepository.FirstOrDefault(e => e.Code == code);
        }

        //public bool ExistCodeEdit(long code, Guid id)
        //{
        //    var @entity = _clientRepository.FirstOrDefault(e => e.Code == code && e.Id != id);
        //    return @entity != null;
        //}

        public void DeleteClientGroups(IList<ClientGroup> clientsGroups)
        {
            foreach (var item in clientsGroups)
            {
                _clientGroupRepository.Delete(item.Id);
            }
        }

        //public void DeleteClientGroupsConcepts(IList<ClientGroupConcept> clientsGroupsConcepts)
        //{
        //    foreach (var item in clientsGroupsConcepts)
        //    {
        //        _clientGroupConceptRepository.Delete(item.Id);
        //    }
        //}

        public void DeleteClientServices(IList<ClientService> clientServices)
        {
            foreach (var item in clientServices)
            {
                _clientServiceRepository.Delete(item.Id);
            }
        }

        public bool UpdateClientService(ClientService clientService)
        {
            try
            {
                _clientServiceRepository.Update(clientService);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ClientService GetClientService(String displayName, String identification)
        {
            ClientService clientService = null;
            try
            {
                clientService = _clientServiceRepository.GetAll().Where(d => d.Service.Name == displayName && d.Client.Identification == identification).FirstOrDefault();
                return clientService;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void DeleteGroupsConcepts(IList<ClientGroupConcept> clientGroupConcepts)
        {
            foreach (var item in clientGroupConcepts)
            {
                _clientGroupConceptRepository.Delete(item.Id);
            }
        }

        public bool UpdateGroupsConcepts(ClientGroupConcept clientGroupConcept)
        {
            try
            {
                _clientGroupConceptRepository.Update(clientGroupConcept);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool CanTenantCreateClient(int tenantId)
        {
            var DayOfRestriction = new DateTime(2018, 4, 10);
            var tenant = _tenantRepository.GetAll().Where(d => d.Id == tenantId && d.CreationTime > DayOfRestriction).FirstOrDefault();
            if (tenant != null)
            {
                var edition = tenant.Edition;
                return (edition.Name == EditionManager.PymeJrEditionName || 
                    edition.Name == EditionManager.Pyme1EditionName || 
                    edition.Name == EditionManager.Pyme1EditionName || 
                    edition.Name == EditionManager.BusinessEditionDisplayName || 
                    edition.Name == EditionManager.PymeJrAnnualEditionName || 
                    edition.Name == EditionManager.Pyme1AnnualEditionName || 
                    edition.Name == EditionManager.Pyme1AnnualEditionName || 
                    edition.Name == EditionManager.BusinessAnnualEditionDisplayName) ? true : false;
            }
            return true;
        }

        //public void updateclientService(Guid id, decimal quantity, decimal discountPercentage)
        //{
        //    var service = _clientServiceRepository.Get(id);
        //    service.Quantity = quantity;
        //    service.DiscountPercentage = discountPercentage;
        //    _clientServiceRepository.Update(service);


        //}

        //public void updateGroupsConcepts(Guid id, decimal quantity, decimal discountPercentage)
        //{
        //    var group =_clientGroupConceptRepository.Get(id);
        //    group.Quantity = quantity;
        //    group.DiscountPercentage = discountPercentage;
        //    _clientGroupConceptRepository.Update(group);


        //}
    }
}
