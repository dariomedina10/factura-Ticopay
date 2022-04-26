using System.Reflection;
using System.Web.Routing;
using Abp.Hangfire;
using Abp.Hangfire.Configuration;
using Abp.Zero.Configuration;
using Abp.Modules;
using Abp.Web.Mvc;
using Abp.Web.SignalR;
using TicoPay.Api;
using Hangfire;
using Hangfire.Windsor;
using Abp.EntityFramework;
using Abp.Threading.BackgroundWorkers;

namespace TicoPay
{
    //[DependsOn(typeof(TicoPayCoreModule),
    //    typeof(TicoPayApplicationModule),
    //    typeof(TicoPayDataModule))]
    //[DependsOn(typeof(AbpEntityFrameworkModule))]
    [DependsOn(
         typeof(TicoPayDataModule),
         typeof(TicoPayApplicationModule),
         typeof(TicoPayWebApiModule),
         typeof(AbpWebSignalRModule),
         typeof(AbpHangfireModule),
         typeof(AbpWebMvcModule))]  
    public class SocketBNModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PreInitialize()
        {
            
            //Configure Hangfire
            Configuration.BackgroundJobs.UseHangfire(configuration =>
            {
                configuration.GlobalConfiguration.UseSqlServerStorage("Default");
            });
            JobActivator.Current = new WindsorJobActivator(IocManager.IocContainer.Kernel);
        }

        //public override void PostInitialize()
        //{
        //    var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
        //   // workManager.Add(IocManager.Resolve<MakeInactiveUsersPassiveWorker>());
        //}

    }
}
