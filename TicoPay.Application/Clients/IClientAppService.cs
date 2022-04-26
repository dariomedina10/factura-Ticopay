using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PagedList;
using TicoPay.Clients.Dto;
using TicoPay.Invoices;
using TicoPay.Services;
using TicoPay.Users;
using TicoPay.General;
using TicoPay.MultiTenancy;
using TicoPay.Services.Dto;
using TicoPay.Groups.Dto;
using TicoPay.GroupConcept.Dto;
using System.Linq.Expressions;

namespace TicoPay.Clients
{
    public interface IClientAppService : IApplicationService
    {
        ListResultDto<ClientDto> GetClients();
        ListResultDto<ClientDto> GetClients(bool detallado);
        ClientDto Get(Guid input);
        ClientDto Get(Expression<Func<Client, bool>> predicate);
        Client GetClient(Guid input);
        ClientDetailOutput GetDetail(Guid input);
        IList<Country> GetAllCountries();
        void Update(UpdateClientInput input);
        void Update(ClientDto input);
        void Create(int tenantId, CreateClientInput input);
        void Create(CreateClientInput input);
        ClientDto Create(ClientDto input);
        void Delete(Guid input);
        IPagedList<ClientDto> SearchClients(SearchClientsInput searchInput);
        IList<Group> GetAllGroups();
        IList<GroupConcepts> GetAllGroupsConcepts();
        string[] ListGroupServicesStrings(Guid input);
        IList<Service> GetAllServices();
        int GetDistritoByBarrios(int id);
        int GetCantonByDistrito(int id);
        int GetIdProvinceByCanton(int id);
        IList<Canton> GetCantonByProvince(int? id);
        IList<Distrito> GetDistritosByCanton(int? id);
        IList<Barrio> GetBarriosByDistrito(int? id);
        IList<Provincia> GetAllProvince();
        UpdateClientInput GetEdit(Guid input);
        IList<Group> ListGroups(Guid input);
        IList<ClientService> ListClientServices();
        string[] ListGroupsTest(Guid input);
        string[] ListServicesStrings(Guid input);
        IList<User> GetAllUser();
        ClientBN GetExistClientByCode(TipoLLaveAcceso tipo, long code);
        IList<Invoice> GetAllInvoicesWithStatusIsParked();
        void UpdateClientBn(ClientBN client);
        Client GetClintByName(string name);
        bool isAllowedDelete(Guid clientId);
        ListResultDto<ServiceDto> GetServices(Guid input);
        void AddServices(Guid clientId, IList<ServiceDto> servicesDto);
        Client GetTicoPayClientByIdentification(string identificationNumber);
        Client GetClintByIdentification(string identificationNumber, int v);
        void UpdateServices(Guid clientId, IList<ServiceDto> servicesDto);        
        void AddGroups(Guid clientId, IList<GroupDto> groupsDto, bool isUpdate);
        ListResultDto<GroupDto> GetGroups(Guid input);
        void AddGroupsConcepts(Guid clientId, IList<GroupConceptsDto> groupsDto, bool isUpdate);
        ListResultDto<GroupConceptsDto> GetGroupsConcepts(Guid input);
        List<ClientDto> GetClientsByTenantId(int tenantId);

        Client GetClintByIdentification(string identification);
        Client CreateManual(int tenantId, CreateClientInput input);
        Client CreateManual(CreateClientInput input);

        IList<ServiceDto> GetListServices(Guid input);
        IList<GroupConceptsDto> GetListGroupsConcepts(Guid input);
    }
}
