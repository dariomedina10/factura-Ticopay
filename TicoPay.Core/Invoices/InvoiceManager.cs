using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.UI;
using TicoPay.Clients;
using TicoPay.MultiTenancy;
using TicoPay.Services;
using TicoPay.Users;
using TicoPay.General;
using TicoPay.Invoices.NCR;
using System.Threading.Tasks;
//using SendMail;

namespace TicoPay.Invoices
{
    public class InvoiceManager : DomainService, IInvoiceManager
    {

        public IEventBus EventBus { get; set; }
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<Note, Guid> _noteRepository;
        private readonly IRepository<InvoiceLine, Guid> _invoiceLineRepository;
        private readonly IRepository<Client, Guid> _clientRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<Register, Guid> _registerRepository;
        private readonly IRepository<Moneda, int> _moneyRepository;
        private readonly TenantManager _tenantManager;
        private readonly UserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IClientManager _clientManager;

        public InvoiceManager(IRepository<Invoice, Guid> invoiceRepository, IRepository<Client, Guid> clientRepository,
            IRepository<Service, Guid> serviceRepository, IRepository<InvoiceLine, Guid> invoiceLinesRepository, IRepository<Note, Guid> noteRepository
            , UserManager userManager, TenantManager tenantManager, IUnitOfWorkManager unitOfWorkManager, IRepository<Register, Guid> registerRepository,
            IRepository<Moneda, int> moneyRepository, IClientManager clientManager)
        {
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _serviceRepository = serviceRepository;
            _invoiceLineRepository = invoiceLinesRepository;
            _noteRepository = noteRepository;
            _userManager = userManager;
            _tenantManager = tenantManager;
            _unitOfWorkManager = unitOfWorkManager;
            EventBus = NullEventBus.Instance;
            _registerRepository = registerRepository;
            _moneyRepository = moneyRepository;
            _clientManager = clientManager;
        }
       
        public void Create(Invoice @invoice)
        {
           
               var insertinvoice = _invoiceRepository.Insert(@invoice);
               if (insertinvoice == null)
                   throw new UserFriendlyException("Hubo un error creando la factura");
        }

        public async Task CreateAsync(Invoice @invoice)
        {

            var insertinvoice = await _invoiceRepository.InsertAsync(@invoice);
            if (insertinvoice == null)
                throw new UserFriendlyException("Hubo un error creando la factura");
        }

        //public void CreateNote(int tenantId, Guid invoiceId, decimal amount, string description, NoteType type)
        //{
        //    SendMailTP mail = new SendMailTP();
        //    var allRegisters = GetallRegisters(tenantId);
        //    var tenant = _tenantManager.Get(tenantId);
        //    var invoice = Get(invoiceId);
        //    var client = _clientManager.Get(invoice.ClientId.Value);


        //    using (var unitOfWork = _unitOfWorkManager.Begin())
        //    {
        //        //**obtener el codigo moneda de Tenant
        //        var note = Note.Create(tenantId, amount, invoiceId, "", NoteCodigoMoneda.CRC, description, type);

        //        note.SetInvoiceConsecutivo(allRegisters, tenant, note.NoteType);
        //        note.SetInvoiceNumberKey(note.CreationTime, note.ConsecutiveNumber, tenant, VoucherSituation.Normal.ToString()); 

        //        // crear XML  y **actualizar register

        //        note.CreateXMLNote(invoice, client, tenant, note);

        //        note.CreatePDFNote(invoice, client, tenant, note);

        //        note.SetInvoiceXMLPDF(@invoice.VoucherKey);

        //        //// Enviar Correo Electronico 
        //        mail.SendMailTicoPay(client.Email, Note.subject, Note.emailbody, Note.emailfooter, "", @"C:\XML\note_" + @invoice.VoucherKey + ".xml", @"C:\PDF\note_" + @invoice.VoucherKey + ".pdf");

        //        _noteRepository.Insert(@note);

        //        unitOfWork.Complete();
        //    }
        //}

        public int CheckInvoiceNumber(TypeDocumentInvoice type)
        {
            //todo: dviquez revisar si es mejor que se genere el número de factura cuando se pague el recibo
            var @invoice = _invoiceRepository.GetAll().Where(X=>X.TypeDocument== type).OrderByDescending(i => i.Number).FirstOrDefault();
            if (@invoice == null)
            {
                return 1;
            }
            return @invoice.Number + 1;
        }
        
        public Invoice Get(Guid id)
        {
            var @invoice =  _invoiceRepository.FirstOrDefault(id);

            //var @invoice = _invoiceRepository.GetAll().Include(a => a.InvoiceLines.First()).Include(a => a.InvoiceLines.First().Service).Include(a => a.InvoiceLines.First().Service.Tax);
            //invoice = invoice.Where(a => a.Id == id).FirstOrDefault();
            if (@invoice == null)
            {
                throw new UserFriendlyException("Could not found the invoice, maybe it's deleted!");
            }
            return @invoice;
        }

        public IQueryable<Client> SearchClients(string q)
        {
            string query = "";
            if (q != null)
                query = q.ToLower();

            var clients = _clientRepository.GetAll();
            clients = clients.Where(a => a.IsDeleted == false && a.Name.ToLower().Contains(query) || a.LastName.ToLower().Equals(query) 
                || a.Identification.ToLower().Contains(query) || a.Identification.ToLower().Equals(query));

            //||a.Code.Equals(query)
            return clients.OrderBy(a => a.Name);
        }

        public Client GetClientToIdentification(string q, long tenantId)
        {
            string query = "";
            if (q != null)
                query = q.ToLower();

            var clients = _clientRepository.GetAll();
            var client = clients.Where(a => a.Identification.ToLower().Equals(query) && a.TenantId == tenantId).FirstOrDefault();
            if (client==null)
                throw new UserFriendlyException("No se ha encontrado ningún cliente con este numero de identificación");
            return client;
        }

        public Register GetallRegisters (int tenantId)
        {
           var registers = _registerRepository.GetAll();
           var register = registers.Where(a => a.TenantId == tenantId).FirstOrDefault();

            if (register == null)
                throw new UserFriendlyException("No se ha encontrado ninguna caja configurada para esta compañia");
            return register;
        }

        public Register GetRegistersbyDrawer( Guid drawerId)
        {
            var registers = _registerRepository.GetAll();
            var register = registers.Where(a => a.DrawerId == drawerId).FirstOrDefault();

            if (register == null)
                throw new UserFriendlyException("No se ha encontrado ninguna caja configurada para esta compañia");
            return register;
        }

        public IList<Service> GetAllServices()
        {
            return _serviceRepository.GetAllList();
        }

        public IList<Moneda> GetAllMoney()
        {
            return _moneyRepository.GetAllList();
        }

        public Client GetClientSoftDelete(Guid id)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var client = _clientRepository.Get(id);
                return client;
            }
        }

        public IList<Client> GetAllListClientsSoftDelete()
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var clients = _clientRepository.GetAllList();
                return clients;
            }
        }


        public IList<InvoiceLine> GetInvoiceLines(Guid invoiceId)
        {
            var invoicesLines =
                _invoiceLineRepository.GetAll().Where(a => a.InvoiceId == invoiceId).Include(a => a.Service);
            return invoicesLines.ToList();
        }

        public void DeleteNotes(Guid invoiceId, Guid noteId)
        {
            var note = _noteRepository.Get(noteId);
            _noteRepository.Delete(note);
        }

        public User GetUser(long userId)
        {
            var user = _userManager.Users.FirstOrDefault(f => f.Id == userId);
            return user;
        }

        public Tenant GetTenant(long tenantId)
        {
            var tenant = _tenantManager.Tenants.FirstOrDefault(f => f.Id == tenantId);
            return tenant;
        }

        public Service GetServiceSoftDelete(Guid serviceId)
        {
           using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
           {
               var service = _serviceRepository.GetAll().Where(a => a.Id == serviceId).Include(a => a.Tax);
              // var service = _serviceRepository.Get(serviceId);
               return service.FirstOrDefault();
           }
        }

        public IQueryable<Register> SearchRegisters(string q)
        {
            string query = "";
            if (q != null)
                query = q.ToLower();

            var registers = _registerRepository.GetAll();
            registers = registers.Where(a =>
                a.IsDeleted == false &&
                a.Name.ToLower().Contains(query) || a.Name.ToLower().Equals(query) ||
                a.RegisterCode.ToLower().Contains(query) || a.RegisterCode.ToLower().Equals(query));

            return registers.OrderBy(a => a.Name);
        }

    }
}
