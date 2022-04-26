using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using PagedList;
using System;
using System.Linq;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;
using TicoPay.ReportInvoicesSentToTribunet.Dto;

namespace TicoPay.ReportInvoicesSentToTribunet
{
    public class ReportInvoicesSentToTribunetAppService : IReportInvoicesSentToTribunetAppService
    {
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<Client, Guid> _clientRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ReportInvoicesSentToTribunetAppService(IRepository<Invoice, Guid> invoiceRepository, IRepository<Client, Guid> clientRepository, IRepository<Tenant, int> tenantRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _tenantRepository = tenantRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public ReportInvoicesSentToTribunetOutput Search(ReportInvoicesSentToTribunetSearchInput input)
        {
            var query = _invoiceRepository.GetAll();

            if (input != null)
            {
                if (input.ClienteId != null)
                    query = query.Where(i => i.Client.Id == input.ClienteId);

                if (input.CedulaCliente != null)
                    query = query.Where(i => i.Client.Identification == input.CedulaCliente);

                if (input.NombreCliente != null)
                    query = query.Where(i => i.Client.Name.Contains(input.NombreCliente));

                if (input.ClienteId != null)
                    query = query.Where(i => i.Client.Id == input.ClienteId);

                if (input.NumeroFactura != null)
                    query = query.Where(i => i.ConsecutiveNumber == input.NumeroFactura);

                if (input.MedioPago != null)
                    query = query.Where(i => i.InvoicePaymentTypes.Any(t => t.Payment.PaymetnMethodType == input.MedioPago.Value));

                if (input.CondicionVenta != null)
                    query = query.Where(i => i.ConditionSaleType == input.CondicionVenta.Value);

                if (input.FechaEmisionDesde != null)
                    query = query.Where(a => a.DueDate >= input.FechaEmisionDesde);

                if (input.FechaEmisionHasta != null)
                {
                    DateTime endDueDate = input.FechaEmisionHasta.Value.AddDays(1);
                    query = query.Where(a => a.DueDate < endDueDate);
                }

                if (input.StatusTribunet != null)
                    query = query.Where(a => a.StatusTribunet == input.StatusTribunet.Value);

                if (input.RecepcionConfirmada != null)
                    query = query.Where(a => a.IsInvoiceReceptionConfirmed == input.RecepcionConfirmada);

                if (input.Status != null)
                    query = query.Where(a => a.Status == input.Status.Value);
            }

            ReportInvoicesSentToTribunetOutput output = new ReportInvoicesSentToTribunetOutput();
            output.Invoices = query.ToList().ConvertAll(i => new ReportInvoicesSentToTribunetDto(i));
            return output;

        }

        public IPagedList<ReportInvoicesSentToTribunetClienteDto> GetAllInvoiceClients(int? pageIndex, int? pageSize, string q)
        {
            var query = _clientRepository.GetAll();
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(t => (t.Code.ToString().Contains(q) || t.Identification.Contains(q) || t.Name.Contains(q) || t.LastName.Contains(q) || t.Email.Contains(q) || t.PhoneNumber.Contains(q)));
            }
            int pageIndexOrDefault = pageIndex.GetValueOrDefault();
            if (pageIndexOrDefault > 1)
            {
                pageIndexOrDefault = pageIndexOrDefault - 1;
            }
            var clients = query.ToList()
                .ConvertAll(c => new ReportInvoicesSentToTribunetClienteDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Identification = c.Identification,
                    Name = c.Name,
                    LastName = c.LastName,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email
                }).ToPagedList(pageIndexOrDefault, pageSize.GetValueOrDefault(10));
            return clients;
        }
    }
}
