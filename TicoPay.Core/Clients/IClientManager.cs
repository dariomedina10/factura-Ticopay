using System;
using System.Collections.Generic;
using Abp.Domain.Services;
using TicoPay.MultiTenancy;
using TicoPay.Services;
using TicoPay.General;



namespace TicoPay.Clients
{
    public interface IClientManager : IDomainService
    {
        Client Get(Guid id);
        void Create(Client @client);
        void AssignGroupToClient(IList<Group> groups, Client client);
        void AssignGroupConceptsToClient(GroupConcepts groups, Client client, decimal quantity, decimal discountPercentage);
        bool ExistIdentification(int tenantId, string identification);
        IList<Country> GetAllListCountries();
        bool ExistIdentificationEdit(string identification, Guid clientId);
        int GeDistritoByBarrio(int id);
        int GetCantonByDistrito(int id);
        int GetIdProvinceByIdCanton(int id);
        IList<Group> GetAllListGroups();
        IList<GroupConcepts> GetAllListGroupsConcepts();
        IList<Service> GetAllListService();
        IList<ClientGroup> GetAllListGroupsClient(Guid id);
        //IList<ClientGroupConcept> GetAllListGroupsConceptsClient(Guid id);
        IList<ClientService> GetAllListClientService(Guid id);
        void DeleteClientGroups(IList<ClientGroup> clientsGroups);
        //void DeleteClientGroupsConcepts(IList<ClientGroupConcept> clientsGroupsConcepts);
        void DeleteClientServices(IList<ClientService> clientServices);
        Service GetService(Guid id);
        IList<Canton> GetAllListCantonByProvince(int? id);
        IList<Distrito> GetAllListDistritosByCanton(int? id);
        IList<Barrio> GetAllListBarriosByDistrito(int? id);
        IList<Provincia> GetAllListProvince();
        Client ExistCodeClient(long code);
        void AddServiceToClient(Service service, Client client, string cronExpresion, DateTime initDateTime,bool generateInvoice, bool allowLatePayment, decimal quantity, decimal discountPercentage);
        IList<ClientGroupConcept> GetAllListGroupsConceptsClient(Guid id);
        void DeleteGroupsConcepts(IList<ClientGroupConcept> clientGroupConcepts);



        bool UpdateGroupsConcepts(ClientGroupConcept clientGroupConcept);
        bool UpdateClientService(ClientService clientService);
        ClientService GetClientService(String displayName, String identification);
        bool CanTenantCreateClient(int tenantId);
        bool ExistIdentificationExt(int tenantId, string identification, Guid? id);

        IList<ClientService> GetAllListClientServiceAdjustment(Guid id);

        void AddServiceToClient(Service service, Client client, string cronExpresion,
    DateTime initDateTime, bool generateInvoice, bool allowLatePayment, decimal quantity, decimal discountPercentage, ClientServiceState clientServiceState);
    }
}
