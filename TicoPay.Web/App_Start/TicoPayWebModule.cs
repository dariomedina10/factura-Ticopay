using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
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
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp;
using TicoPay.BackgroundJobs;
using Abp.Web.Mvc.Configuration;
using System.Configuration;

namespace TicoPay.Web
{
    [DependsOn(
        typeof(TicoPayDataModule),
        typeof(TicoPayApplicationModule),
        typeof(TicoPayWebApiModule),
        typeof(AbpWebSignalRModule),
        typeof(AbpWebMvcModule),
        typeof(AbpHangfireModule),              //Comentar para hacer debug en produccion sin hangfire
        typeof(TicoPayBackgroundJobsModule)     //Comentar para hacer debug en produccion sin hangfire
        )]  
    public class TicoPayWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.Languages.Add(new LanguageInfo("es", "Español", "famfamfam-flag-es"));

            //Enable database based localization
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            //Configure navigation/menu
            Configuration.Navigation.Providers.Add<TicoPayNavigationProvider>();

            //Configure Hangfire
            #region Comentar para hacer debug en produccion sin hangfire
            Configuration.BackgroundJobs.UseHangfire(configuration =>
            {
                configuration.GlobalConfiguration.UseSqlServerStorage("Default");
            }); 
            #endregion

            Configuration.Modules.AbpWeb().AntiForgery.IsEnabled = false;

            Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = ConfigurationManager.AppSettings["DomainFormat"];

            Configuration.Modules.AbpMvc().IsValidationEnabledForControllers = false;

            JobActivator.Current = new WindsorJobActivator(IocManager.IocContainer.Kernel);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);            
        }
    }
}
