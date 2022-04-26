using System;
using Abp.Dependency;
using Abp.Domain.Uow;
using Hangfire;
using TicoPay.MultiTenancy;

namespace TicoPay.BackgroundJobs
{
    public class BackgroundJobManager :  ITransientDependency
    {
        private readonly TenantManager _tenantManager;
        
        public BackgroundJobManager(TenantManager tenantManager)
        {
            _tenantManager = tenantManager;
        }

        public virtual void TriggerRecurrentInvoicesJob()
        {
           string id= BackgroundJob.Enqueue<TenantBackgroundJobManager>(manager => manager.TriggerServiceCheckerInvoiceJob());
           BackgroundJob.ContinueWith<TenantBackgroundJobManager>(id, manager => manager.TriggerGroupConceptsCheckerInvoiceJob(),JobContinuationOptions.OnAnyFinishedState);
             //BackgroundJob.Enqueue<TenantBackgroundJobManager>(manager => manager.TriggerGroupConceptsCheckerInvoiceJob());
        }

        //[UnitOfWork]
        //public virtual void TriggerGroupTenantJob()
        //{
        //    BackgroundJob.Enqueue<TenantBackgroundJobManager>(manager => manager.TriggerGroupConceptsCheckerInvoiceJob());
        //}

        public virtual void TriggerResendInvoicesJob()
        {

            BackgroundJob.Enqueue<TenantBackgroundJobManager>(manager => manager.TriggerResendFailedInvoices());
        }

        public virtual void TriggerResendAsadaCloudTicopay()
        {

            BackgroundJob.Enqueue<TenantBackgroundJobManager>(manager => manager.TriggerResendAsadaCloudTicopay());
        }

        public virtual void TriggerResendVoucherJob()
        {
            BackgroundJob.Enqueue<TenantBackgroundJobManager>(manager => manager.TriggerResendFailedVouchers());
        }

        public virtual void TriggerSyncsInvoicesJob()
        {

            BackgroundJob.Enqueue<TenantBackgroundJobManager>(manager => manager.TriggerSyncsInvoicesWithTaxAdministration());
        }

        public virtual void TriggerSyncsVouchersJob()
        {

            BackgroundJob.Enqueue<TenantBackgroundJobManager>(manager => manager.TriggerSyncsVoucherithTaxAdministration());
        }

        public virtual void TriggerSyncsInvoicesUnattenedJob()
        {

            BackgroundJob.Enqueue<TenantBackgroundJobManager>(manager => manager.TriggerSyncsInvoicesUnattendedWithTaxAdministration());
        }

        [UnitOfWork]
        public virtual void TrigerTicoPayInvoiceAlerts()
        {
            BackgroundJob.Enqueue<TicoPayInvoicesBackgroundJobManager>(manager => manager.TriggerPendingInvoicesAlerts());
        }

        [UnitOfWork]
        public virtual void TriggerCheckSaveIssuesInAzure()
        {
            BackgroundJob.Enqueue<TenantBackgroundJobManager>(manager => manager.TriggerCheckSaveIssuesInAzure());
        }
    }
}
