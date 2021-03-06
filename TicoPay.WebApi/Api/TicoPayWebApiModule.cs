using System.Reflection;
using System.Web.Http;
using Abp.Application.Services;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.WebApi;
using Abp.WebApi.Controllers.Dynamic.Builders;

namespace TicoPay.Api
{
    [DependsOn(typeof(AbpWebApiModule),
        typeof(TicoPayCoreModule),
        typeof(TicoPayApplicationModule))]
    public class TicoPayWebApiModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.AbpWebApi()
                .DynamicApiControllerBuilder
                .ForAll<IApplicationService>(typeof(TicoPayApplicationModule).Assembly, "app")
                .Build();

            Configuration.Modules.AbpWebApi().HttpConfiguration.Filters.Add(new HostAuthenticationFilter("Bearer"));
        }
    }
}
