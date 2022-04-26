using Abp.Hangfire;
using Abp.Hangfire.Configuration;
using Abp.Modules;
using Hangfire;
using System;
using System.Reflection;

namespace TicoPay.BackgroundJobs
{
    [DependsOn(typeof(TicoPayCoreModule), typeof(AbpHangfireModule))]
    public class TicoPayBackgroundJobsModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            RecurringJob.AddOrUpdate<BackgroundJobManager>(job => job.TriggerRecurrentInvoicesJob(), Cron.MinuteInterval(30)); 
            RecurringJob.AddOrUpdate<BackgroundJobManager>(job => job.TriggerResendInvoicesJob(), Cron.HourInterval(6));
            RecurringJob.AddOrUpdate<BackgroundJobManager>(job => job.TriggerResendVoucherJob(), Cron.HourInterval(12));
            RecurringJob.AddOrUpdate<BackgroundJobManager>(job => job.TriggerSyncsInvoicesJob(), Cron.HourInterval(1));
            RecurringJob.AddOrUpdate<BackgroundJobManager>(job => job.TriggerSyncsVouchersJob(), Cron.HourInterval(1));
            RecurringJob.AddOrUpdate<BackgroundJobManager>(job => job.TriggerSyncsInvoicesUnattenedJob(), Cron.HourInterval(1));
            RecurringJob.AddOrUpdate<BackgroundJobManager>(job => job.TrigerTicoPayInvoiceAlerts(), Cron.Daily());
            //RecurringJob.AddOrUpdate<BackgroundJobManager>(job => job.TriggerResendAsadaCloudTicopay(), Cron.Yearly());
            RecurringJob.AddOrUpdate<BackgroundJobManager>(job => job.TriggerCheckSaveIssuesInAzure(), Cron.HourInterval(1));
        }
    }
}
