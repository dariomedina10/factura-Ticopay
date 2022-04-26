using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;
using TicoPay.ReportTaxAdministration.Dto;

namespace TicoPay.ReportTaxAdministration
{
    public class ReportsTaxAdministrationService : IReportsTaxAdministrationService
    {
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<Client, Guid> _clientRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ReportsTaxAdministrationService(IRepository<Invoice, Guid> invoiceRepository, IRepository<Client, Guid> clientRepository, IRepository<Tenant, int> tenantRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _tenantRepository = tenantRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public ReportTaxAdministrationSearchInvoicesOutput Search(ReportTaxAdministrationSearchInvoicesInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.SoftDelete))
            {
                var query = _invoiceRepository.GetAll();
                if (input != null)
                {
                    if (input.EmisorId != null)
                        query = query.Where(i => i.Tenant.Id == input.EmisorId.Value);

                    if (input.ReceptorId != null)
                        query = query.Where(i => i.Client.Id == input.ReceptorId);

                    if (input.NumeroFactura != null)
                        query = query.Where(i => i.Number == input.NumeroFactura.Value);

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

                    if (input.MontoDesde != null)
                        query = query.Where(a => a.SaleTotal >= input.MontoDesde.Value);

                    if (input.MontoHasta != null)
                        query = query.Where(a => a.SaleTotal < input.MontoHasta.Value);

                    if (input.TotalImpuestosDesde != null)
                        query = query.Where(a => a.TotalTax >= input.TotalImpuestosDesde.Value);

                    if (input.TotalImpuestosHasta != null)
                        query = query.Where(a => a.TotalTax < input.TotalImpuestosHasta.Value);

                    if (input.StatusTribunet != null)
                        query = query.Where(a => a.StatusTribunet == input.StatusTribunet.Value);

                    if (input.RecepcionConfirmada != null)
                        query = query.Where(a => a.IsInvoiceReceptionConfirmed == input.RecepcionConfirmada);

                    if (input.Status != null)
                        query = query.Where(a => a.Status == input.Status.Value);
                }

                ReportTaxAdministrationSearchInvoicesOutput output = new ReportTaxAdministrationSearchInvoicesOutput();
                output.Invoices = query.ToList().ConvertAll(i => new ViewInvoiceDto(i));
                return output;
            }
        }

        public IPagedList<EmisorDto> GetAllInvoicesSenders(int? pageIndex, int? pageSize, string q)
        {
            var query = _tenantRepository.GetAll();
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(t => (t.IdentificationNumber.Contains(q) || t.Name.Contains(q) || t.Email.Contains(q) || t.PhoneNumber.Contains(q)));
            }
            int pageIndexOrDefault = pageIndex.GetValueOrDefault();
            if (pageIndexOrDefault > 1)
            {
                pageIndexOrDefault = pageIndexOrDefault - 1;
            }
            var tenants = query.ToList()
                .ConvertAll(i => new EmisorDto
                {
                    Id = i.Id,
                    IdentificationNumber = i.IdentificationNumber,
                    Name = i.Name,
                    PhoneNumber = i.PhoneNumber,
                    Email = i.Email
                }).ToPagedList(pageIndexOrDefault, pageSize.GetValueOrDefault(10));
            return tenants;
        }

        public IPagedList<ReceptorDto> GetAllInvoiceReceivers(int? pageIndex, int? pageSize, string q)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant))
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
                    .ConvertAll(c => new ReceptorDto
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
}
