using System;
using Abp.Dependency;
using Abp.Domain.Uow;
using Hangfire;
using TicoPay.Invoices;
using TicoPay.Services;
using TicoPay.GroupConcept;
using TicoPay.MultiTenancy;
using TicoPay.Users;
using System.Linq;
using static TicoPay.MultiTenancy.Tenant;
using TicoPay.Vouchers;
using TicoPay.Unattended;

namespace TicoPay.BackgroundJobs
{
    public class TenantBackgroundJobManager : ITransientDependency
    {
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly IServiceAppService _serviceAppService;
        private readonly IGroupConceptsAppService _groupConcetsAppService;
        private readonly TenantManager _tenantManager;
        private readonly IUserAppService _userManager;
        private readonly IVoucherAppService _voucherAppService;
        private readonly IUnattendedAppService _unattendedAppService;

        public TenantBackgroundJobManager(IInvoiceAppService invoiceAppService, IServiceAppService serviceAppService, IGroupConceptsAppService groupConcetsAppService, TenantManager tenantManager, IUserAppService userManager, IVoucherAppService voucherAppService, IUnattendedAppService unattendedAppService)
        {
            _tenantManager = tenantManager;
            _invoiceAppService = invoiceAppService;
            _serviceAppService = serviceAppService;
            _groupConcetsAppService = groupConcetsAppService;
            _userManager = userManager;
            _voucherAppService = voucherAppService;
            _unattendedAppService = unattendedAppService;
        }

        [UnitOfWork(isTransactional:false)]
        [DisableConcurrentExecution(1000 * 60 * 10)]//timeout 5min
        [AutomaticRetry(Attempts = 0)]
        public virtual void TriggerResendFailedVouchers()
        {
            try
            {
                var tenants = _tenantManager.GetActiveTenants().Where(d => d.ValidateHacienda);
                if (tenants != null)
                {
                    foreach (var tenant in tenants)
                    {
                        _voucherAppService.ResendFailedVouchers(tenant);
                    }
                }
            }
            catch (Exception ex)
            {
                BackgroundJob.Enqueue(() => Console.WriteLine("Error al Procesar el proceso de Creación de facturas de Consumo \n Error Message: {0} \n Trace: {1}", ex.Message, ex.StackTrace));
            }
        }

        [UnitOfWork(isTransactional: false)]
        [DisableConcurrentExecution(1000 * 60 * 5)]//timeout 5min
        [AutomaticRetry(Attempts = 0)]
        public virtual void TriggerServiceCheckerInvoiceJob()
        {
            string tenancyName = null;
            try
            {
                var tenants = _tenantManager.GetActiveTenantsWithRecurringInvoice();
                foreach (var tenant in tenants)
                {
                    tenancyName = tenant.Name;
                    //BackgroundJob.Enqueue(() => Console.WriteLine("Procesando Recibos de Servicios Tenant: " + tenancyName));

                    User user = null;
                    try
                    {
                        user = _userManager.GetUserByRole("Admin", tenant.Id);
                    }
                    catch (Exception ex)
                    {
                        BackgroundJob.Enqueue(() => Console.WriteLine("Error al Procesar el proceso de Creación de facturas de Consumo Tenant: {0} \n Error Message: {1} \n Trace: {2}", tenancyName, ex.Message, ex.StackTrace));
                        continue;
                    }

                    if (!user.IsEmailConfirmed || !tenant.IsAddressCompleted())
                    {
                        //BackgroundJob.Enqueue(() => Console.WriteLine("Tenant: " + tenancyName + " usuario no confirmado o dirección incompleta"));
                        continue;
                    }

                    var services = _serviceAppService.GetServicesEntities(tenant.Id).Where(x => x.IsRecurrent == true).ToList();

                    _invoiceAppService.CreateServiceInvoices(services, tenant.Email, (tenant.AlternativeEmail == null) ? "" : tenant.AlternativeEmail, tenant.ComercialName, (FirmType)tenant.FirmaRecurrente);

                }
            }
            catch (Exception ex)
            {
                BackgroundJob.Enqueue(() => Console.WriteLine("Error al Procesar el proceso de Creación de facturas de Consumo Tenant: {0} \n Error Message: {1} \n Trace: {2}", tenancyName, ex.Message, ex.StackTrace));
            }
        }

        [UnitOfWork]
        [DisableConcurrentExecution(1000 * 60 * 5)]//timeout 5min
        [AutomaticRetry(Attempts = 0)]
        public virtual void TriggerGroupConceptsCheckerInvoiceJob()
        {
            try
            {
                var tenants = _tenantManager.GetActiveTenants();
                foreach (var tenant in tenants)
                {
                    var user = _userManager.GetUserByRole("Admin", tenant.Id);

                    if (!user.IsEmailConfirmed || !tenant.IsAddressCompleted())
                    {
                        //BackgroundJob.Enqueue(() => Console.WriteLine("Tenant: " + tenant.Name + " usuario no confirmado o dirección iincompleta"));
                        continue;
                    }
                    if (String.IsNullOrEmpty(tenant.UserTribunet) || String.IsNullOrEmpty(tenant.PasswordTribunet))
                    {
                        //BackgroundJob.Enqueue(() => Console.WriteLine("Tenant: " + tenant.Name + " credenciales de hacienda incompletas"));
                        continue;
                    }
                    if (tenant.FirmaRecurrente == Tenant.FirmType.Llave && _invoiceAppService.isdigitalPendingInvoice(tenant.Id))
                    {
                        //BackgroundJob.Enqueue(() => Console.WriteLine("Tenant: " + tenant.Name + " Facturas con firma digital pendientes"));
                        continue;
                    }

                    var groupConcetps = _groupConcetsAppService.GetGroupConceptsEntities(tenant.Id);
                    foreach (var group in groupConcetps.Where(d => d.Services.Any(c => c.IsRecurrent)))
                    {
                        _invoiceAppService.CreateGroupConceptsInvoices(group);
                    }
                }
            }
            catch (Exception ex)
            {
                BackgroundJob.Enqueue(() => Console.WriteLine("Error al Procesar el proceso de Creación de facturas de Consumo \n Error Message: {0} \n Trace: {1}", ex.Message, ex.StackTrace));
            }
        }

        [UnitOfWork(isTransactional: false)]
        [DisableConcurrentExecution(1000 * 60 * 10)]//timeout 5min
        [AutomaticRetry(Attempts = 0)]
        public virtual void TriggerResendFailedInvoices()
        {
            try
            {
                var tenantIdWithNotSendInvoices = _invoiceAppService.GetTenantsIdWithInvoicesNotSended();
                var tenants = _tenantManager.GetActiveTenants().Where(d => d.ValidateHacienda && d.IsActive && tenantIdWithNotSendInvoices.Any(i => i == d.Id)).Select(d => new Tenant { Id = d.Id, PasswordTribunet = d.PasswordTribunet, UserTribunet = d.UserTribunet });
                if (tenants != null)
                {
                    foreach (var tenant in tenants)
                    {
                        _invoiceAppService.ResendFailedInvoices(tenant);
                    }
                }
            }
            catch (Exception ex)
            {
                BackgroundJob.Enqueue(() => Console.WriteLine("Error al Procesar el proceso de Creación de facturas de Consumo \n Error Message: {0} \n Trace: {1}", ex.Message, ex.StackTrace));
            }
        }

        [UnitOfWork(isTransactional:false)]
        [DisableConcurrentExecution(1000 * 60 * 10)]//timeout 5min
        [AutomaticRetry(Attempts = 0)]
        public virtual void TriggerSyncsInvoicesWithTaxAdministration()
        {
            try
            {
                var tenants = _tenantManager.GetActiveTenants().Where(d => d.ValidateHacienda && d.IsActive).Select(d => new Tenant { Id = d.Id, PasswordTribunet = d.PasswordTribunet, UserTribunet = d.UserTribunet });
                if (tenants != null)
                {
                    foreach (var tenant in tenants)
                    {
                        _invoiceAppService.SyncsInvoicesWithTaxAdministration(tenant);
                    }
                }
            }
            catch (Exception ex)
            {
                BackgroundJob.Enqueue(() => Console.WriteLine("Error al Procesar el proceso de Creación de facturas de Consumo \n Error Message: {0} \n Trace: {1}", ex.Message, ex.StackTrace));
            }
        }

        [UnitOfWork(isTransactional: false)]
        [DisableConcurrentExecution(1000 * 60 * 10)]//timeout 5min
        [AutomaticRetry(Attempts = 0)]
        public virtual void TriggerSyncsVoucherithTaxAdministration()
        {
            try
            {
                var tenants = _tenantManager.GetActiveTenants().Where(d => d.ValidateHacienda && d.IsActive).Select(d => new Tenant { Id = d.Id, PasswordTribunet = d.PasswordTribunet, UserTribunet = d.UserTribunet });
                if (tenants != null)
                {
                    foreach (var tenant in tenants)
                    {
                        _voucherAppService.SyncsInvoicesWithTaxAdministration(tenant);
                    }
                }
            }
            catch (Exception ex)
            {
                BackgroundJob.Enqueue(() => Console.WriteLine("Error al Procesar el proceso de Creación de facturas de Consumo \n Error Message: {0} \n Trace: {1}", ex.Message, ex.StackTrace));
            }
        }

        [UnitOfWork(isTransactional: false)]
        [DisableConcurrentExecution(1000 * 60 * 10)]//timeout 5min
        [AutomaticRetry(Attempts = 0)]
        public virtual void TriggerSyncsInvoicesUnattendedWithTaxAdministration()
        {
            try
            {
                var tenants = _tenantManager.GetActiveTenants().Where(d => d.ValidateHacienda && d.IsActive).Select(d => new Tenant { Id = d.Id, PasswordTribunet = d.PasswordTribunet, UserTribunet = d.UserTribunet });
                if (tenants != null)
                {
                    foreach (var tenant in tenants)
                    {
                        _unattendedAppService.SyncsUnattendedWithTaxAdministration(tenant);
                    }
                }
            }
            catch (Exception ex)
            {
                BackgroundJob.Enqueue(() => Console.WriteLine("Error al Procesar el proceso de Creación de facturas de Consumo \n Error Message: {0} \n Trace: {1}", ex.Message, ex.StackTrace));
            }
        }

        [UnitOfWork(isTransactional: false)]
        [DisableConcurrentExecution(1000 * 60 * 10)]//timeout 5min
        [AutomaticRetry(Attempts = 0)]
        public virtual void TriggerResendAsadaCloudTicopay()
        {
            try
            {
                var tenants = _tenantManager.GetActiveTenants().Where(d => d.ValidateHacienda && d.Id == 2 && d.TenancyName == "ticopay");
                if (tenants != null)
                {
                    foreach (var tenant in tenants)
                    {
                        _invoiceAppService.ResendFailedInvoicesRepair(tenant);
                    }
                }
            }
            catch (Exception ex)
            {
                BackgroundJob.Enqueue(() => Console.WriteLine("Error al Procesar el proceso de Creación de facturas de Consumo \n Error Message: {0} \n Trace: {1}", ex.Message, ex.StackTrace));
            }
        }

        [UnitOfWork(isTransactional: false)]
        [DisableConcurrentExecution(1000 * 60 * 10)]//timeout 5min
        [AutomaticRetry(Attempts = 0)]
        public virtual void TriggerCheckSaveIssuesInAzure()
        {
            try
            {
                var tenants = _tenantManager.GetActiveTenants().Where(d => d.ValidateHacienda && d.IsActive).Select(d => new Tenant { Id = d.Id, PasswordTribunet = d.PasswordTribunet, UserTribunet = d.UserTribunet });
                if (tenants != null)
                {
                    foreach (var tenant in tenants)
                    {
                        _invoiceAppService.CheckSaveIssuesInAzure(tenant);
                    }
                }
            }
            catch (Exception ex)
            {
                BackgroundJob.Enqueue(() => Console.WriteLine("Error al Procesar el proceso de Creación de facturas de Consumo \n Error Message: {0} \n Trace: {1}", ex.Message, ex.StackTrace));
            }
        }
    }
}
