using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;

namespace TicoPay
{
    [DependsOn(typeof(TicoPayCoreModule), typeof(AbpAutoMapperModule))]
    public class TicoPayApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
