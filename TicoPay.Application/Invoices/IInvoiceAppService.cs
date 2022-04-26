using System;
using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PagedList;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.Invoices.Dto;
using TicoPay.Services;
using TicoPay.General;
using System.Threading.Tasks;
using TicoPay.Invoices.XSD;
using static TicoPay.MultiTenancy.Tenant;
using TicoPay.MultiTenancy;

namespace TicoPay.Invoices
{
    public interface IInvoiceAppService : IApplicationService
    {
        ListResultDto<InvoiceDto> GetInvoices();
        InvoiceDto Get(Guid input);
        Invoice GetInvoice(Guid input);
        InvoiceDetailOutput GetDetail(EntityDto<Guid> input);
        InvoiceApiDto GetPaidInvoice(Guid input);

        //void Create(CreateInvoiceInput input);

        InvoiceApiDto Create(CreateInvoiceDTO input);
        Invoice CreateList(CreateInvoiceInput input, List<PaymentInvoceDto> listPaymetnInvoce);
        IList<Moneda> GetAllListMoney();
        NoteDto CreateNote(NoteDto input, bool noAfectarBalance = false);
        NoteDto CreateReverseNoteFromInvoice(InvoiceDto invoiceOrTicket, string externalReferenceNumber);
        void ApplyReverse(NoteDto input);
        //void CreateNote(Guid invoiceId, double amount, int reason, int type, decimal montotax, decimal totalNota);
        //void ApplyReverse(Guid invoiceId, double amount, int reason, int type, decimal montotax, decimal totalNota);
        Task<bool> CreateAllServiceInvoices(Service service);
        //void CreateAllServiceInvoices(Service service);
        void DeleteNotes(Guid invoiceId, Guid noteId);
        void UpdateRegister(Register register);
        IPagedList<Client> SearchClients(string query, int? page);
        IPagedList<InvoiceDto> SearchInvoices(SearchInvoicesInput searchInput);
        ListResultDto<InvoiceApiDto> SearchInvoicesApi(SearchInvoicesApi searchInput);
        ListResultDto<NoteApiDto> SearchNotesApi(SearchNotesApi searchInput);
        IList<InvoiceDto> SearchInvoicesPending(string identification, long tenantId);
        IList<Service> GetAllServices();
        IList<Bank> GetAllBanks();
        bool HasPayFirstInvoice(Tenant tenant);
        IList<InvoiceLineDto> GetAllListInvoiceLines(Guid invoiceId);
        void PayInvoice(int typePayment, decimal balance, string trans,  Invoice invoice, decimal balanceDebit, Guid? bankId, string userCard);
        void PayInvoiceList(List<PaymentInvoceDto> listPaymetnInvoce, Guid invoiceId);
        void VoidInvoice(Guid invoiceId);
        string GetTenant();
        Service GetService(Guid serviceId);
        ClientDto GetClient(Guid clientId);
        ClientDto GetClient(string identification, long tenantId);
        Client GetClientPdf(Guid ClienteId);
        List<InvoicePendingPayBN> GetInvoicesPendingPay(ClientBN client);
        List<InvoicePendingPayBN> GetInvoicesPendingOld(ClientBN client, DateTime invoicedate);
        InvoicePendingPayBN GetInvoicesByNumber(ClientBN client, string invoicenumber);
        int PayInvoiceBn(InvoicePendingPayBN factura, int codigoAgencia, string trama);
        void ReverseInvoice(InvoicePendingPayBN factura, string reference);
        IList<InvoicePendingPayBN> GetInvoicesPendingPayPAR(int index, List<int> listTenant, int PageSize, out bool indicador);
        IPagedList<Register> SearchRegisters(string q, int? page);
        void CreateGroupConceptsInvoices(GroupConcepts groupConcepts);

        Task<int> GetTotalInvoicesInMonthAsync(int tenantId);
        Task<int> GetInvoicesMonthlyLimitAsync(int tenantId);
        void ResendFailedInvoices(Tenant tenant);
        void SyncsInvoicesWithTaxAdministration(Tenant tenantId);
        bool IsInvoiceReceptionConfirmed(Guid clientId, Guid invoiceId);
        Task ConfirmInvoiceReceptionAsync(Guid clientId, Guid invoiceId);
        Task<bool> InvoicesMonthlyLimitReachedAsync(int tenantId);

        List<Invoice> ObtenerFacturaNCR(Guid ClienteId);
        void CreateNoteCorreccion(Guid invoiceId, double amount, int reason, int type, decimal montotax, decimal totalNota, int tenantid);
        void ResendInvoice(Guid id);
        void ResendNote(Guid noteId);
        List<Payment> GetPaymentReverse(Guid clientId, FacturaElectronicaResumenFacturaCodigoMoneda codigoModeda);
        Task CreateInvoices(List<CreateInvoiceDTO> Invoices);
        Note ReverseNote(NoteDto model);

        bool IsNoteReceptionConfirmed(Guid invoiceId, Guid noteId);
        Task ConfirmNoteReceptionAsync(Guid invoiceId, Guid noteId);
        decimal GetNoteAmountAlreadyReversed(string ConsecutiveNumber);
        Task SaveXMLFirmaDigital(Invoice invoice, string xmlContent);

        Task SaveXMLFirmaDigital(Note note, string xmlContent);

        void CreateServiceInvoices(IList<Service> services, string Email, string AlternativeEmail,string remitente, FirmType firmType);
        void sendemailbodyRecurren(string email, string AlternativeEmail, string type, string remitente);
        bool isdigitalPendingInvoice(int tenatId);
        bool isdigitalPendingNote(int tenatId);

        Task<bool> TenantCantDoInvoices(string username, FirmType? firmType);

        Note GetNote(Guid id);

        void ResendFailedInvoicesRepair(Tenant tenant);

        void CheckSaveIssuesInAzure(Tenant tenant);

        List<int> GetTenantsIdWithInvoicesNotSended();

        void CreateUploadSendElectronicIncoice(Guid idinvoice, int TenantId);

    }
}
