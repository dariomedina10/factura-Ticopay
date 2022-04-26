using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.AutoMapper;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;
using TicoPay.MultiTenancy.Dto;
using TicoPay.Common;

namespace TicoPay.BackgroundJobs
{
    public class TicoPayInvoicesBackgroundJobManager : ITransientDependency
    {
        private readonly TenantManager _tenantManager;
        private readonly ITenantAppService _tenantAppService;
        private readonly IClientAppService _clientAppService;
        private readonly IInvoiceAppService _invoiceAppService;

        public TicoPayInvoicesBackgroundJobManager(TenantManager tenantManager, ITenantAppService tenantAppService, IClientAppService clientAppService, IInvoiceAppService invoiceAppService)
        {
            _tenantManager = tenantManager;
            _tenantAppService = tenantAppService;
            _clientAppService = clientAppService;
            _invoiceAppService = invoiceAppService;
        }

        [UnitOfWork]
        [DisableConcurrentExecution(1000 * 60 * 10)]//timeout 5min
        [AutomaticRetry(Attempts = 0)]
        public virtual void TriggerPendingInvoicesAlerts()
        {
            var tenants = (_tenantManager.GetActiveTenants().Where(d => d.TenancyName != "ticopay" && d.TenancyName != "Default").ToList()).Select(d => new TenantDto { Id = d.Id, LastPayNotificationSendedAt = d.LastPayNotificationSendedAt, IdentificationNumber = d.IdentificationNumber });
            if (tenants != null)
            {
                foreach (var tenant in tenants)
                {
                    int totalDaysWithDebt = -1;
                    if ((tenant.LastPayNotificationSendedAt == null && !_tenantAppService.HasPayFirstInvoice(tenant.IdentificationNumber)) || (HasPendinInvoices(tenant.IdentificationNumber, out totalDaysWithDebt) && ((DateTime.UtcNow - tenant.LastPayNotificationSendedAt)?.Days >= 3) && totalDaysWithDebt > 0))
                    {
                        if (totalDaysWithDebt == -1)
                        {
                            HasPendinInvoices(tenant.IdentificationNumber, out totalDaysWithDebt);
                        }
                        int daysOfGrace = (!_tenantAppService.HasPayFirstInvoice(tenant.IdentificationNumber)) ? (totalDaysWithDebt < 4 ? 4 - totalDaysWithDebt : 0) : (totalDaysWithDebt < 7 ? 7 - totalDaysWithDebt : 0);
                        var currentTenant = _tenantManager.Get(tenant.Id);
                        _tenantAppService.SendPayNotificationIfNeeded(currentTenant, daysOfGrace);
                    }
                }
            }
        }

        private bool HasPendinInvoices(string IdentificationNumber, out int totalDaysWithDebt)
        {
            totalDaysWithDebt = 0;
            bool result = false;
            if (IdentificationNumber != null)
            {
                Client client = _clientAppService.GetTicoPayClientByIdentification(IdentificationNumber);
                if (client != null)
                {
                    ClientBN clientBN = new ClientBN { Id = client.Id };
                    var oldPendinInvoice = _invoiceAppService.GetInvoicesPendingPay(clientBN)
                        .OrderBy(i => i.DueDate).FirstOrDefault();
                    if (oldPendinInvoice != null)
                    {
                        result = true;
                        totalDaysWithDebt = (DateTimeZone.Now() - oldPendinInvoice.DueDate).Days;
                        if (_tenantAppService.IsClientExonerate(IdentificationNumber))
                        {
                            int CreditDays = 15;

                            if (client.CreditDays != null && client.CreditDays > 0)
                            {
                                CreditDays = (int)client.CreditDays;
                            }

                            if (oldPendinInvoice.CreditTerm > 0 && oldPendinInvoice.CreditTerm > CreditDays)
                            {
                                CreditDays = oldPendinInvoice.CreditTerm;
                            }

                            if (totalDaysWithDebt < CreditDays)
                            {
                                totalDaysWithDebt = 0;
                            }
                            else
                            {
                                totalDaysWithDebt = (DateTimeZone.Now().AddDays(-CreditDays) - (oldPendinInvoice.DueDate)).Days;
                            }
                        }
                    }
                }
            }
            return result;
        }

    }
}
