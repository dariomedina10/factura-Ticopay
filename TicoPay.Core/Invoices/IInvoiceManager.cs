using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.General;
using TicoPay.MultiTenancy;
using TicoPay.Services;
using TicoPay.Users;

namespace TicoPay.Invoices
{
    public interface IInvoiceManager : IDomainService
    {
        Invoice Get(Guid id);

        void Create(Invoice @invoice);
        Task CreateAsync(Invoice @invoice);

        //void CreateNote(int tenantId, Guid invoiceId, decimal amount, string description, NoteType type);

        int CheckInvoiceNumber( TypeDocumentInvoice type);
        IList<Moneda> GetAllMoney();

        IQueryable<Client> SearchClients(string query);

        Client GetClientSoftDelete(Guid id);

        IList<Client> GetAllListClientsSoftDelete();
        Register GetallRegisters(int tenantId);

        Register GetRegistersbyDrawer(Guid drawerId);

        IList<Service> GetAllServices();

        IList<InvoiceLine> GetInvoiceLines(Guid invoiceId);

        void DeleteNotes(Guid invoiceId, Guid noteId);

        User GetUser(long userId);

        Tenant GetTenant(long tenantId);

        Client GetClientToIdentification(string q, long tenantId);

        Service GetServiceSoftDelete(Guid serviceId);
        IQueryable<Register> SearchRegisters(string query);
    }
}
