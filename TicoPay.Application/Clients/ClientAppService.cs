using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using AutoMapper;
using PagedList;
using TicoPay.Clients.Dto;
using TicoPay.Invoices;
using TicoPay.Services;
using TicoPay.Users;
using TicoPay.General;
using TicoPay.MultiTenancy;
using Abp.Domain.Uow;
using Abp.Dependency;
using System.Collections.ObjectModel;
using LinqKit;
using TicoPay.Services.Dto;
using TicoPay.Taxes.Dto;
using TicoPay.Groups.Dto;
using TicoPay.GroupConcept.Dto;
using TicoPay.Common;
using TicoPay.Invoices.XSD;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Globalization;


namespace TicoPay.Clients
{
    public class ClientAppService : ApplicationService, IClientAppService, ITransientDependency
    {
        //These members set in constructor using constructor injection.
        private readonly IRepository<Client, Guid> _clientRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<Group, Guid> _groupRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IRepository<GroupConcepts, Guid> _groupConceptRepository;
        private readonly IRepository<ClientService, Guid> _clientServiceRepository;
        private readonly IClientManager _clientManager;
        public readonly UserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly TenantManager _tenantManager;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public ClientAppService(IRepository<Client, Guid> clientRepository, IRepository<Service, Guid> serviceRepository, IRepository<Group, Guid> groupRepository, IRepository<ClientService, Guid> clientServiceRepository,
            IClientManager clientManager, UserManager userManager, IRepository<Invoice, Guid> invoiceRepository, TenantManager tenantManager, IRepository<Tenant, int> tenantRepository,
            IRepository<GroupConcepts, Guid> groupConceptRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _clientRepository = clientRepository;
            _serviceRepository = serviceRepository;
            _clientServiceRepository = clientServiceRepository;
            _groupRepository = groupRepository;
            _invoiceRepository = invoiceRepository;
            _clientManager = clientManager;
            _userManager = userManager;
            _groupConceptRepository = groupConceptRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _tenantManager = tenantManager;
            _tenantRepository = tenantRepository;
        }

        [UnitOfWork]
        public void Create(CreateClientInput input)
        {
            Create(AbpSession.GetTenantId(), input);
        }

        [UnitOfWork]
        public void Create(int tenantId, CreateClientInput input)
        {
            if ((!String.IsNullOrEmpty(input.Identification) && _clientManager.ExistIdentification(tenantId, input.Identification)) ||
               (!String.IsNullOrEmpty(input.IdentificacionExtranjero) && _clientManager.ExistIdentificationExt(tenantId, input.IdentificacionExtranjero, null)))
                throw new UserFriendlyException("Existe un cliente con el mismo número identificación.");

            var currentTenant =  _tenantManager.Get(tenantId);



                var @client = Client.Create(tenantId, input.Name, input.LastName, input.Gender, input.Birthday, input.Identification, input.IdentificationType,
                                        input.PhoneNumber, input.MobilNumber, input.Fax, input.Email, input.WebSite, input.BarrioId,
                                        input.PostalCode, input.IsoCountry, input.Address, input.Latitud, input.Longitud, input.ContactName, input.ContactMobilNumber,
                                        input.ContactPhoneNumber, input.ContactEmail, input.Note, input.NameComercial, input.IdentificacionExtranjero, input.CreditDays, input.CountryID);
            IList<Group> temp = new List<Group>();
            if (input.Test != null && input.Test.Count > 0)
            {
                foreach (var item in input.Test)
                {
                    var group = _groupRepository.Get(Guid.Parse(item));
                    temp.Add(group);
                }
                _clientManager.AssignGroupToClient(temp, @client);
            }

            //IList<Service> @service = new List<Service>();

            if (input.ClientServiceList != null && input.ClientServiceList.Count > 0)
            {

                foreach (var item in input.ClientServiceList)
                {
                    var serv = _clientManager.GetService(item.Id);
                    //serv.Quantity = item.Quantity;
                    //serv.DiscountPercentage = item.DiscountPercentage;
                    _clientManager.AddServiceToClient(serv, @client, "", DateTimeZone.Now(), false, false, item.Quantity, item.DiscountPercentage);
                }


            }

            // Asigna el grupo de servicios al cliente
            //client.GroupConcepts = new Collection<GroupConcepts>();
            //IList<GroupConcepts> groupConcepts = new List<GroupConcepts>();
            if (input.GroupsConceptsList != null && input.GroupsConceptsList.Count > 0)
            {
                foreach (var item in input.GroupsConceptsList)
                {
                    var group = _groupConceptRepository.Get(item.Id);
                    _clientManager.AssignGroupConceptsToClient(group, client, item.Quantity, item.DiscountPercentage);
                    //client.GroupConcepts.Add(group);
                }

            }
            _clientManager.Create(@client);

            if (!currentTenant.IsTutorialClients)
            {

                currentTenant.IsTutorialClients = true;
                _tenantRepository.Update(currentTenant);
            }


            // actualizo el tenant
        }

        [Abp.Runtime.Validation.DisableValidation]
        public ClientDto Create(ClientDto input)
        {
            var tenantId = AbpSession.GetTenantId();
            if(input.Identification != null)
            {
                input.Identification = Regex.Replace(input.Identification, "[^0-9]", "", RegexOptions.None);
            }
            if (!String.IsNullOrEmpty(input.Identification) && _clientManager.ExistIdentification(tenantId, input.Identification))
                throw new UserFriendlyException("Existe un cliente con el mismo número de cédula. \n");

            if (input.IdentificationType == IdentificacionTypeTipo.Cedula_Fisica && (input.Identification.Length < 9 || input.Identification.Length > 9))
                throw new UserFriendlyException("El formato de Cédula Física es incorrecto. El número debe contener 9 dígitos. \n");

            if (input.IdentificationType == IdentificacionTypeTipo.Cedula_Juridica && (input.Identification.Length < 10 || input.Identification.Length > 10))
                throw new UserFriendlyException("El formato de Cédula Jurídica es incorrecto. El número debe contener 10 dígitos. \n");

            if (input.IdentificationType == IdentificacionTypeTipo.DIMEX && (input.Identification.Length < 11 || input.Identification.Length > 12))
                throw new UserFriendlyException("El formato de DIMEX es incorrecto. El número debe contener 11 0 12 dígitos. \n");

            if (input.IdentificationType == IdentificacionTypeTipo.NITE && (input.Identification.Length < 10 || input.Identification.Length > 10))
                throw new UserFriendlyException("El formato de NITE es incorrecto. El número debe contener 10 dígitos. \n");


            input.PhoneNumber = (input.PhoneNumber == null || input.PhoneNumber == "") ? null : DataTelefono(input.PhoneNumber);
            input.MobilNumber = (input.MobilNumber == null || input.MobilNumber == "") ? null : DataTelefono(input.MobilNumber);
            input.Fax = (input.Fax == null || input.Fax == "") ? null : DataTelefono(input.Fax);
            input.ContactMobilNumber = (input.ContactMobilNumber == null || input.ContactMobilNumber == "") ? null : DataTelefono(input.ContactMobilNumber);
            input.ContactPhoneNumber = (input.ContactPhoneNumber == null || input.ContactPhoneNumber == "") ? null : DataTelefono(input.ContactPhoneNumber);

            input.Name = (input.Name == null || input.Name == "") ? null : DataName(input.Name);
            input.LastName = (input.LastName == null || input.LastName == "") ? null : DataName(input.LastName);
            input.CreditDays = (input.CreditDays == null || input.CreditDays == 0) ? 1 : input.CreditDays;

            var @client = Client.Create(tenantId, input.Name, input.LastName, input.Gender, input.Birthday, input.Identification, input.IdentificationType,
                                        input.PhoneNumber, input.MobilNumber, input.Fax, input.Email, input.WebSite, input.BarrioId,
                                        input.PostalCode, input.IsoCountry, input.Address, input.Latitud, input.Longitud, input.ContactName, input.ContactMobilNumber,
                                        input.ContactPhoneNumber, input.ContactEmail, input.Note, input.NameComercial, input.IdentificacionExtranjero, input.CreditDays, input.CountryId);

            _clientManager.Create(@client);
            /*
            IList<Group> temp = new List<Group>();
            if (input.Categories != null && input.Categories.Count > 0)
            {
                foreach (var item in input.Categories)
                {
                    var group = _groupRepository.Get(item.Id);
                    temp.Add(group);
                }
                _clientManager.AssignGroupToClient(temp, @client);
            }
            */
            return Mapper.Map<ClientDto>(@client);
            // actualizo el tutorial tenant?
        }

        //Definicion Formato numero telefonico
        public string DataTelefono(string NumeroTelefono)
        {

            NumeroTelefono = Regex.Replace(NumeroTelefono, "[^0-9]", "", RegexOptions.None);
            string doble = Convert.ToChar(34).ToString(CultureInfo.InvariantCulture);
            string line = Convert.ToChar(45).ToString(CultureInfo.InvariantCulture);
            
            string nuevotelf = "";

            var ArrayNumeroTelefono = NumeroTelefono.ToCharArray();

            var prefijo = ArrayNumeroTelefono[0] + ArrayNumeroTelefono[1] + ArrayNumeroTelefono[2];

            if(prefijo == 506)
            {
            nuevotelf = ArrayNumeroTelefono[0] + ArrayNumeroTelefono[1] + ArrayNumeroTelefono[2] + line +
                         ArrayNumeroTelefono[3] + ArrayNumeroTelefono[4] + ArrayNumeroTelefono[5] + ArrayNumeroTelefono[6] +
                          ArrayNumeroTelefono[7] + ArrayNumeroTelefono[8] + ArrayNumeroTelefono[9] + ArrayNumeroTelefono[10];
            }
            else
            {
             nuevotelf = "506" + line +
                         ArrayNumeroTelefono[0] + ArrayNumeroTelefono[1] + ArrayNumeroTelefono[2] + ArrayNumeroTelefono[3] +
                         ArrayNumeroTelefono[4] + ArrayNumeroTelefono[5] + ArrayNumeroTelefono[6] + ArrayNumeroTelefono[7];

            }
            return nuevotelf;

        }

        public string DataName(string Name)
        {

            string newName = "";

            if (Name.Length > 79) { newName = Name.Substring(0, 79); } else { newName = Name; }
            return newName;

        }

        public ListResultDto<ClientDto> GetClients()
        {
            var clients = _clientRepository.GetAllList();
            return new ListResultDto<ClientDto>(clients.MapTo<List<ClientDto>>());
        }

        public ListResultDto<ClientDto> GetClients(bool detallado)
        {
            if (detallado)
            {
                var clients = _clientRepository.GetAllList();
                return new ListResultDto<ClientDto>(clients.MapTo<List<ClientDto>>());
            }
            else
            {
                var result = new ListResultDto<ClientDto>();
                result.Items = _clientRepository.GetAll().Select(d => new ClientDto { Id = d.Id, Name = d.Name, LastName = d.LastName, Identification = d.Identification, IdentificationType = d.IdentificationType, IdentificacionExtranjero = d.IdentificacionExtranjero, Email = d.Email, ContactEmail = d.ContactEmail, CreditDays = (int)d.CreditDays }).ToList();
                return result;
            }
        }

        public ClientDetailOutput GetDetail(Guid input)
        {
            var @client = _clientRepository.Get(input);

            if (@client == null)
            {
                throw new UserFriendlyException("Could not found the client, maybe it's deleted.");
            }

            return @client.MapTo<ClientDetailOutput>();
        }

        public ClientDto Get(Guid input)
        {
            try
            {
                var @client = _clientRepository.Get(input);               
                return Mapper.Map<ClientDto>(client);
            }
            catch
            {
                return null;
            }
            
        }

        public ClientDto Get(Expression<Func<Client,bool>> predicate)
        {
            try
            {
                var @client = _clientRepository.FirstOrDefault(predicate);
                return Mapper.Map<ClientDto>(client);
            }
            catch
            {
                return null;
            }

        }

        public Client GetClient(Guid input)
        {
            var @client = _clientRepository.Get(input);
            if (@client == null)
            {
                throw new UserFriendlyException("Could not found the client, maybe it's deleted.");
            }
            return Mapper.Map<Client>(client);
        }

        public UpdateClientInput GetEdit(Guid input)
        {
            var @client = _clientRepository.Get(input);
            if (@client == null)
            {
                throw new UserFriendlyException("Could not found the client, maybe it's deleted.");
            }
            return Mapper.Map<UpdateClientInput>(client);
        }

      
        public IList<Group> ListGroups(Guid input)
        {
            var @clientGroups = _clientManager.GetAllListGroupsClient(input);
            IList<Group> temp = new List<Group>();
            foreach (var item in @clientGroups)
            {
                temp.Add(item.Group);
            }
            return temp;
        }

        public IList<ClientService> ListClientServices()
        {
            var services =
                _clientServiceRepository.GetAll()
                    .Where(s => s.State == ClientServiceState.Active && !s.GeneratingInvoice)
                    .ToList();
            ////&& s.DateEvent >= _dateTime.Now
            return services;
        }

        public string[] ListGroupsTest(Guid input)
        {
            var @clientGroups = _clientManager.GetAllListGroupsClient(input);
            string[] temp = new string[@clientGroups.Count];
            int count = 0;
            foreach (var item in @clientGroups)
            {
                temp[count] = item.GroupId.ToString();
                count++;
            }
            return temp;
        }

        public string[] ListServicesStrings(Guid input)
        {
            var @clientService = _clientManager.GetAllListClientService(input);
            string[] temp = new string[@clientService.Count];
            int count = 0;
            foreach (var item in @clientService)
            {
                temp[count] = item.ServiceId.ToString();
                count++;
            }
            return temp;
        }

        public string[] ListGroupServicesStrings(Guid input)
        {
            Client client = _clientRepository.Get(input);
            if (client != null && client.GroupConcepts != null)
            {
                return client.GroupConcepts.Select(g => g.Id.ToString()).ToArray();
            }
            return new string[] { };
        }

        public IList<GroupConceptsDto> GetListGroupsConcepts(Guid input)
        {
            try
            {
                var groups = _clientManager.GetAllListGroupsConceptsClient(input);
                if (groups == null)
                {
                    return null;
                }
                var lgroups = (from c in groups
                               select new GroupConceptsDto
                               {
                                   Id = c.Group.Id,
                                   IdDetails=c.Id,
                                   Name = c.Group.Name,
                                   Description = c.Group.Description,
                                   Quantity = c.Quantity,
                                   DiscountPercentage = c.DiscountPercentage

                               }).ToList();
                return lgroups;
            }
            catch
            {

                return null;
            }

        }
        [UnitOfWork]
        public void Delete(Guid input)
        {
            var @client = _clientRepository.Get(input);
            if (@client == null)
            {
                throw new UserFriendlyException("Could not found the client, maybe it's deleted.");
            }
            if (_invoiceRepository.GetAll().Any(a => a.ClientId == input))
            {
                throw new UserFriendlyException("El cliente posee facturas asociadas.");
            }

            @client.IsDeleted = true;

            var @clientGroups = _clientManager.GetAllListGroupsClient(input);
            if (@clientGroups.Count > 0)
                _clientManager.DeleteClientGroups(@clientGroups);

            var @clientServices = _clientManager.GetAllListClientService(input);
            if (@clientServices.Count > 0)
                _clientManager.DeleteClientServices(@clientServices);

            _clientRepository.Update(@client);
        }

        /// <summary>
        /// Searches for clients and returns page result
        /// </summary>
        /// <param name="searchInput"></param>
        /// <returns></returns>
        public IPagedList<ClientDto> SearchClients(SearchClientsInput searchInput)
        {
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value  : 1;

            var clients = _clientRepository.GetAll();
            if (searchInput.NameFilter == null)
                searchInput.NameFilter = "";

            //if (searchInput.IdentificationFilter == null)
            //    searchInput.IdentificationFilter = "";

            if (searchInput.EmailFilter == null)
                searchInput.EmailFilter = "";

            //lets search by Identification or Name or Email or Code      
            if (!String.IsNullOrWhiteSpace(searchInput.EmailFilter))
            {
                //lets search by Identification or Name or Email or Code      
                clients = clients.Where(c => c.Email.Contains(searchInput.EmailFilter));
            }

            if (searchInput.IdentificationFilter != null && searchInput.IdentificationFilter != "")
                clients = clients.Where(c => c.Identification==searchInput.IdentificationFilter || c.IdentificacionExtranjero==searchInput.IdentificationFilter);

            if (!String.IsNullOrWhiteSpace(searchInput.NameFilter))
                clients = clients.Where(c => c.Name.Contains(searchInput.NameFilter)
                                        || c.LastName.Contains(searchInput.NameFilter));

            if (searchInput.GroupId != null && searchInput.GroupId != "")
            {
                Guid group = Guid.Parse(searchInput.GroupId);
                clients = clients.Where(c => c.Groups.Any(d => d.GroupId == group));
            }

            if (searchInput.CodeFilter != null && searchInput.CodeFilter != "")
            {
                int code = int.Parse(searchInput.CodeFilter);
                clients = clients.Where(c => c.Code == code);
            }
            var list = clients.Select(d => new ClientDto { Id = d.Id, Code = d.Code, Name = d.Name, LastName = d.LastName, IdentificationType = (IdentificacionTypeTipo)d.IdentificationType, Identification = d.Identification, IdentificacionExtranjero = d.IdentificacionExtranjero, Email = d.Email, MobilNumber = d.MobilNumber, CanBeDelete = d.Invoices.Count() == 0 });
            var result = list.OrderByDescending(p => p.Name).ToPagedList(currentPageIndex, searchInput.MaxResultCount);
            
            return result;
        }
        /// <summary>
        /// Afiliar/desafiliar cliente - banco
        /// </summary>
        /// <param name="client"></param>
        public void UpdateClientBn(ClientBN clientBN)
        {
            var client = _clientRepository.Get(clientBN.Id);

            client.FormaPagoBn = clientBN.FormaPagoBn;
            client.DiaPagoBn = clientBN.DiaPagoBn;
            client.MontoMaximoBn = clientBN.MontoMaximoBn;
            client.PagoAutomaticoBn = clientBN.PagoAutomaticoBn;

            _clientRepository.Update(client);
        }

        [UnitOfWork]
        public void Update(UpdateClientInput input)
        {
            var @client = _clientRepository.Get(input.Id);

            if ((!String.IsNullOrEmpty(input.Identification) && _clientManager.ExistIdentificationEdit(input.Identification, input.Id)) ||
               (!String.IsNullOrEmpty(input.IdentificacionExtranjero) && _clientManager.ExistIdentificationExt(AbpSession.GetTenantId(), input.IdentificacionExtranjero, input.Id)))
                throw new UserFriendlyException("Existe un cliente con el mismo número de identificación.");


            if (@client == null)
            {
                throw new UserFriendlyException("Could not found the client, maybe it's deleted.");
            }

            @client.Name = input.Name;
            @client.LastName = input.LastName;
            @client.NameComercial = input.NameComercial;
            @client.IdentificationType = input.IdentificationType;
            @client.Identification = input.Identification;
            @client.Birthday = input.Birthday;
            @client.Gender = input.Gender;
            @client.PhoneNumber = input.PhoneNumber;
            @client.MobilNumber = input.MobilNumber;
            @client.Fax = input.Fax;
            @client.Email = input.Email;
            @client.WebSite = input.WebSite;
            if (input.BarrioId.HasValue)
                @client.BarrioId = input.BarrioId.Value;
            if (input.CountryID.HasValue)
                client.CountryId = input.CountryID.Value;
            client.CountryId = input.CountryID;
            @client.PostalCode = input.PostalCode;
            @client.IsoCountry = input.IsoCountry;
            @client.Address = input.Address;
            @client.ContactName = input.ContactName;
            @client.ContactMobilNumber = input.ContactMobilNumber;
            @client.ContactPhoneNumber = input.ContactPhoneNumber;
            @client.ContactEmail = input.ContactEmail;
            @client.Note = input.Note;
            @client.Latitud = input.Latitud;
            @client.Longitud = input.Longitud;
            @client.IdentificacionExtranjero = input.IdentificacionExtranjero;
            if (input.CreditDays.HasValue)
                @client.CreditDays = input.CreditDays.Value;

            IList<Group> newGroups = new List<Group>();
            var @clientGroups = _clientManager.GetAllListGroupsClient(input.Id);
            if (@clientGroups.Count > 0)
                _clientManager.DeleteClientGroups(@clientGroups);
            if (input.GroupsSelected != null)
            {
                foreach (var item in input.GroupsSelected)
                {
                    var group = _groupRepository.Get(Guid.Parse(item));
                    newGroups.Add(group);
                }
                _clientManager.AssignGroupToClient(newGroups, @client);
            }
            var @clientServices = _clientManager.GetAllListClientService(input.Id);
            if (input.ClientServiceList != null)
            {
                // IList<Service> newServices = new List<Service>();
               // eliminar los servicios eliminados por el usuario
                if (@clientServices.Count > 0)
                {
                    var deleteservice = (from sc in @clientServices
                                         where !input.ClientServiceList.Any(x=>(x.Id==sc.ServiceId && x.IdDetails==sc.Id))
                                         select sc
                                         ).ToList();
                    _clientManager.DeleteClientServices(deleteservice);
                }

           
            
                foreach (var item in input.ClientServiceList)
                {
                    var clientservice = @clientServices.Where(x => (x.ServiceId == item.Id && item.IdDetails == x.Id)).FirstOrDefault();
                    if (clientservice != null)
                    {
                        clientservice.Quantity = item.Quantity;
                        clientservice.DiscountPercentage = item.DiscountPercentage;
                        _clientManager.UpdateClientService(clientservice);
                    }
                    else
                    {
                        var service = _clientManager.GetService(item.Id);
                        _clientManager.AddServiceToClient(service, @client, "", DateTimeZone.Now(), false, false, item.Quantity, item.DiscountPercentage);
                    }
                    
                }
                //newServices = input.ClientServiceList.MapTo<List<Service>>();


            }
            else
            {
                _clientManager.DeleteClientServices(@clientServices);
            }
            var @GroupsConcepts = _clientManager.GetAllListGroupsConceptsClient(input.Id);
            if (input.GroupsConceptsList != null && input.GroupsConceptsList.Count > 0)
            {                      

                //IList<GroupConcepts> groupConcepts = new List<GroupConcepts>();
               

                if (@GroupsConcepts.Count > 0)
                {
                    var deleteservice = (from sc in @GroupsConcepts
                                         where !input.GroupsConceptsList.Any(x => (x.Id == sc.GroupId && x.IdDetails == sc.Id))
                                         select sc
                                        ).ToList();
                    _clientManager.DeleteGroupsConcepts(deleteservice);
                }                    

            
                foreach (var item in input.GroupsConceptsList)
                {
                    var clientGservice = @GroupsConcepts.Where(x => (x.GroupId == item.Id && item.IdDetails == x.Id)).FirstOrDefault();
                    if (clientGservice != null)
                    {
                        clientGservice.Quantity = item.Quantity;
                        clientGservice.DiscountPercentage = item.DiscountPercentage;
                        _clientManager.UpdateGroupsConcepts(clientGservice);
                    }
                    else
                    {
                        var group = _groupConceptRepository.Get(item.Id);
                        _clientManager.AssignGroupConceptsToClient(group, client, item.Quantity, item.DiscountPercentage);
                    }
                }

            }
            else
            {
                _clientManager.DeleteGroupsConcepts(@GroupsConcepts);
            }

            _clientRepository.Update(@client);

            _unitOfWorkManager.Current.SaveChanges();
        }

        public void Update(ClientDto input)
        {
            var @client = _clientRepository.Get(input.Id);

            if ((!String.IsNullOrEmpty(input.Identification) && _clientManager.ExistIdentificationEdit(input.Identification, input.Id)) ||
                (!String.IsNullOrEmpty(input.IdentificacionExtranjero) && _clientManager.ExistIdentificationExt(AbpSession.GetTenantId(), input.IdentificacionExtranjero, input.Id)))
                throw new UserFriendlyException("Existe un cliente con el mismo número de identificación.");


            if (@client == null)
            {
                throw new UserFriendlyException("Could not found the client, maybe it's deleted.");
            }

            @client.Name = input.Name;
            @client.LastName = input.LastName;
            @client.NameComercial = input.NameComercial;
            @client.IdentificationType = input.IdentificationType;
            @client.Identification = input.Identification;
            @client.Birthday = input.Birthday;
            @client.Gender = input.Gender;
            @client.PhoneNumber = input.PhoneNumber;
            @client.MobilNumber = input.MobilNumber;
            @client.Fax = input.Fax;
            @client.Email = input.Email;
            @client.WebSite = input.WebSite;
            if (input.BarrioId!=0)
                @client.BarrioId = input.BarrioId;
            @client.PostalCode = input.PostalCode;
            @client.IsoCountry = input.IsoCountry;
            @client.Address = input.Address;
            @client.ContactName = input.ContactName;
            @client.ContactMobilNumber = input.ContactMobilNumber;
            @client.ContactPhoneNumber = input.ContactPhoneNumber;
            @client.ContactEmail = input.ContactEmail;
            @client.Note = input.Note;
            @client.Latitud = input.Latitud;
            @client.Longitud = input.Longitud;
            @client.IdentificacionExtranjero = input.IdentificacionExtranjero;
            
            _clientRepository.Update(@client);
        }

        public IList<Group> GetAllGroups()
        {
            return _clientManager.GetAllListGroups();
        }

        public IList<Country> GetAllCountries()
        {
            return _clientManager.GetAllListCountries();
        }

        public IList<GroupConcepts> GetAllGroupsConcepts()
        {
            return _clientManager.GetAllListGroupsConcepts();
        }

        public IList<Service> GetAllServices()
        {
            return _clientManager.GetAllListService();
        }

        public IList<Canton> GetCantonByProvince(int? id)
        {
            return _clientManager.GetAllListCantonByProvince(id);
        }

        public IList<Distrito> GetDistritosByCanton(int? id)
        {
            return _clientManager.GetAllListDistritosByCanton(id);
        }

        public IList<Barrio> GetBarriosByDistrito(int? id)
        {
            return _clientManager.GetAllListBarriosByDistrito(id);
        }

        public int GetDistritoByBarrios(int id)
        {
            return _clientManager.GeDistritoByBarrio(id);
        }

        public int GetCantonByDistrito(int id)
        {
            return _clientManager.GetCantonByDistrito(id);
        }

        public int GetIdProvinceByCanton(int id)
        {
            return _clientManager.GetIdProvinceByIdCanton(id);
        }
        public IList<Provincia> GetAllProvince()
        {
            return _clientManager.GetAllListProvince();
        }
        public IList<User> GetAllUser()
        {
            var users = _userManager.Users.ToList();
            return users;
        }
        /// <summary>
        /// Verifica si el Cliente existe en una lista de tenant - banco
        /// </summary>
        /// <param name="identification"></param>
        /// <param name="listTenant"></param>
        /// <returns></returns>
        public ClientBN GetExistClientByCode(TipoLLaveAcceso tipo, long code)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant); // deshabilitar el filtro por tenant

            var query = from client in _clientRepository.GetAll()
                        where client.Code == code
                        select new ClientBN
                        {
                            Id = client.Id,
                            Code = client.Code,
                            Name = client.Name,
                            LastName = client.LastName,
                            TenantId = client.TenantId,
                            PagoAutomaticoBn = client.PagoAutomaticoBn,
                            DiaPagoBn = client.DiaPagoBn,
                            MontoMaximoBn = client.MontoMaximoBn,
                            FormaPagoBn = client.FormaPagoBn
                        };
            var entities = query.FirstOrDefault();

            return entities;
           // return _clientManager.ExistCodeClient(code);
        }

        public IList<Invoice> GetAllInvoicesWithStatusIsParked()
        {
            var incoiceList = _invoiceRepository.GetAll();
            incoiceList = incoiceList.Where(a => a.Status == Status.Parked);
            return incoiceList.Include(a => a.Client).Include(a => a.InvoiceLines).Include(a => a.Notes).OrderBy(a => a.DueDate).ToList();
        }

        public bool isAllowedDelete(Guid clientId)
        {
            var predicate = PredicateBuilder.New<Invoice>(true);
            predicate = predicate.And(a => a.Status == Status.Parked);
            predicate = predicate.And(a => a.ClientId == clientId);

            var incoiceList = _invoiceRepository.GetAll().Where(predicate);
            if (incoiceList.Count() <= 0)
                return true;
            else
                return false;
                
            
        }

        public ListResultDto<ServiceDto> GetServices(Guid input)
        {

            var @services = _clientManager.GetAllListClientService(input);
            if (@services == null)
            {
                throw new UserFriendlyException("Could not found the client, maybe it's deleted.");
            }
            var lservices = (from c in @services
                             select new ServiceDto
                             {
                                 Id=c.Service.Id,
                                 Name = c.Service.Name,
                                 Price = c.Service.Price,
                                 TaxId = c.Service.TaxId,
                                 Tax = c.Service.Tax.MapTo<TaxDto>(),
                                 UnitMeasurement = c.Service.UnitMeasurement,
                                 UnitMeasurementOthers = c.Service.UnitMeasurementOthers,
                                 CronExpression = c.Service.CronExpression,
                                 IsRecurrent = c.Service.IsRecurrent

                             }).ToList();
            return new ListResultDto<ServiceDto>(lservices);
        }

        public IList<ServiceDto> GetListServices(Guid input)
        {

            var @services = _clientManager.GetAllListClientService(input);
            if (@services == null)
            {
                return null;
            }
            var lservices = (from c in @services
                             select new ServiceDto
                             {
                                 IdDetails=c.Id,
                                 Id = c.Service.Id,
                                 Name = c.Service.Name,
                                 Price = c.Service.Price,
                                 TaxId = c.Service.TaxId,
                                 Tax = c.Service.Tax.MapTo<TaxDto>(),
                                 UnitMeasurement = c.Service.UnitMeasurement,
                                 UnitMeasurementOthers = c.Service.UnitMeasurementOthers,
                                 CronExpression = c.Service.CronExpression,
                                 Quantity=c.Quantity,
                                 DiscountPercentage=c.DiscountPercentage,
                                 IsRecurrent = c.Service.IsRecurrent

                             }).ToList();
            return lservices;
        }
        [UnitOfWork]
        public void  UpdateServices(Guid clientId, IList<ServiceDto> servicesDto)
        {           
           var @client = _clientRepository.Get(clientId);

            var @clientServices = _clientManager.GetAllListClientService(clientId);

            if (@clientServices.Count > 0)
                _clientManager.DeleteClientServices(@clientServices);

            addServiceDTo(@client, servicesDto);
        }

        public void AddServices(Guid clientId, IList<ServiceDto> servicesDto)
        {
            var @client = _clientRepository.Get(clientId);

            addServiceDTo(@client, servicesDto);

        }

        private void  addServiceDTo (Client client, IList<ServiceDto> servicesDto)
        {
            foreach (var item in servicesDto)
            {
                var serv = _clientManager.GetService(item.Id);
                //serv.Quantity = item.Quantity;
                //serv.DiscountPercentage = item.DiscountPercentage;
                _clientManager.AddServiceToClient(serv, @client, "", DateTimeZone.Now(), false, false, item.Quantity, item.DiscountPercentage);
            }
            //var @service = (from d in servicesDto
            //                join s in _serviceRepository.GetAll() on d.Id equals s.Id
            //                select s).ToList();
            //_clientManager.AddServiceToClient(@service, client, "", DateTime.Now, false, false);
        }

        public ListResultDto<GroupConceptsDto> GetGroupsConcepts(Guid input)
        {
            try
            {
                var @client = _clientRepository.Get(input);
                var groups = client.GroupConcepts;               
                var lgroups = (from c in groups
                               select new GroupConceptsDto
                               {
                                   Id = c.Group.Id,
                                   Name = c.Group.Name,
                                   Description = c.Group.Description

                               }).ToList();
                return new ListResultDto<GroupConceptsDto>(lgroups);
            }
            catch 
            {

                return null;
            }
           
        }            

        [UnitOfWork]
        public void AddGroupsConcepts(Guid clientId, IList<GroupConceptsDto> groupsDto, bool isUpdate)
        {
            var @client = _clientRepository.Get(clientId);
            if (isUpdate)
            {
                client.GroupConcepts.Clear();
            }

            //var Groups = (from d in groupsDto
            //              join s in _groupConceptRepository.GetAll() on d.Id equals s.Id
            //              select s).ToList();
            //IList<GroupConcepts> groups = new List<GroupConcepts>();
            foreach (var item in groupsDto)
            {
                var group = _groupConceptRepository.Get(item.Id);
                _clientManager.AssignGroupConceptsToClient(group, @client, item.Quantity, item.DiscountPercentage);
            }
           

            _clientRepository.Update(@client);
            //_clientManager.AssignGroupToClient(Groups, client);
        }     

        public ListResultDto<GroupDto> GetGroups(Guid input)
        {
            try
            {
                var groups = _clientManager.GetAllListGroupsClient(input);
               
                var lgroups = (from c in groups
                               select new GroupDto
                               {
                                   Id = c.Group.Id,
                                   Name = c.Group.Name,
                                   Description = c.Group.Description

                               }).ToList();
                return new ListResultDto<GroupDto>(lgroups);
            }
            catch 
            {
                return null;
            }
           
        }       
        
        [UnitOfWork]
        public void AddGroups(Guid clientId, IList<GroupDto> groupsDto, bool isUpdate)
        {
            var @client = _clientRepository.Get(clientId);
            if (isUpdate)
            {
                var @clientGroups = _clientManager.GetAllListGroupsClient(clientId);

                if (@clientGroups.Count > 0)
                    _clientManager.DeleteClientGroups(@clientGroups);
            }

            var Groups = (from d in groupsDto
                          join s in _groupRepository.GetAll() on d.Id equals s.Id
                          select s).ToList();
            _clientManager.AssignGroupToClient(Groups, client);
        }     

        public Client GetClintByName(string name)
        {
            Client client = _clientRepository.GetAll().Where(c => c.Name == name).FirstOrDefault();
            return client;
        }

        public List<ClientDto> GetClientsByTenantId(int tenantId)
        {
            return _clientRepository
                .GetAll()
                .Where(i => i.TenantId == tenantId)
                .ToList()
                .MapTo<List<ClientDto>>();
        }

        public Client GetClintByIdentification(string identification)
        {
            Client client = _clientRepository.GetAll().Where(c => c.Identification == identification).FirstOrDefault();
            return client;
        }

        public Client GetClintByIdentification(string identification, int tenantId)
        {
            Client client = _clientRepository.GetAll().Where(c => c.Identification == identification && c.TenantId == tenantId).FirstOrDefault();
            return client;
        }

        public Client GetTicoPayClientByIdentification(string identification)
        {
            Client client = GetClintByIdentification(identification, 2);
            return client;
        }

        [UnitOfWork]
        public Client CreateManual(int tenantId, CreateClientInput input)
        {
            if ((!String.IsNullOrEmpty(input.Identification) && _clientManager.ExistIdentification(tenantId, input.Identification)) ||
                (!String.IsNullOrEmpty(input.IdentificacionExtranjero) && _clientManager.ExistIdentificationExt(tenantId, input.IdentificacionExtranjero, null)))
                throw new UserFriendlyException("Existe un cliente con el mismo número de identificación.");

            var currentTenant = _tenantManager.Get(tenantId);

            var @client = Client.Create(tenantId, input.Name, input.LastName, input.Gender, input.Birthday, input.Identification, input.IdentificationType,
                                        input.PhoneNumber, input.MobilNumber, input.Fax, input.Email, input.WebSite, input.BarrioId,
                                        input.PostalCode, input.IsoCountry, input.Address, input.Latitud, input.Longitud, input.ContactName, input.ContactMobilNumber,
                                        input.ContactPhoneNumber, input.ContactEmail, input.Note, input.NameComercial, input.IdentificacionExtranjero, null,input.CountryID);
            IList<Group> temp = new List<Group>();
            if (input.Test != null && input.Test.Count > 0)
            {
                foreach (var item in input.Test)
                {
                    var group = _groupRepository.Get(Guid.Parse(item));
                    temp.Add(group);
                }
                _clientManager.AssignGroupToClient(temp, @client);
            }

            //IList<Service> @service = new List<Service>();
            if (input.ClientServiceList != null && input.ClientServiceList.Count > 0)
            {
                foreach (var item in input.ClientServiceList)
                {
                    var serv = _clientManager.GetService(item.Id);
                    _clientManager.AddServiceToClient(serv, @client, "", DateTimeZone.Now(), false, false, item.Quantity, item.DiscountPercentage);
                }
                
                //_clientManager.AddServiceToClient(@service, @client, "", DateTime.Now, false, false);
            }

            // Asigna el grupo de servicios al cliente
            //client.GroupConcepts = new Collection<GroupConcepts>();
           // IList<GroupConcepts> groupConcepts = new List<GroupConcepts>();
            if (input.GroupsConceptsList != null && input.GroupsConceptsList.Count > 0)
            {
                foreach (var item in input.GroupsConceptsList)
                {
                    var group = _groupConceptRepository.Get(item.Id);
                    _clientManager.AssignGroupConceptsToClient(group, client, item.Quantity, item.DiscountPercentage);
                }
                
            }
            _clientManager.Create(@client);

            if (!currentTenant.IsTutorialClients)
            {

                currentTenant.IsTutorialClients = true;
                _tenantRepository.Update(currentTenant);
            }
            _clientManager.Create(@client);

            if (!currentTenant.IsTutorialClients)
            {

                currentTenant.IsTutorialClients = true;
                _tenantRepository.Update(currentTenant);
            }

            return @client;

            // actualizo el tenant
        }

        [UnitOfWork]
        public Client CreateManual(CreateClientInput input)
        {
            return CreateManual(AbpSession.GetTenantId(), input);
        }
    }
}
