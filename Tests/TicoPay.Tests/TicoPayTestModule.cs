using Abp.Modules;
using Abp.MultiTenancy;
using Abp.Zero.Configuration;
using Castle.MicroKernel.Registration;
using NSubstitute;

namespace TicoPay.Tests
{
    [DependsOn(
        typeof(TicoPayApplicationModule),
        typeof(TicoPayDataModule))]
    public class TicoPayTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            //Registering fake services

            IocManager.IocContainer.Register(
                Component.For<IAbpZeroDbMigrator>()
                    .UsingFactoryMethod(() => Substitute.For<IAbpZeroDbMigrator>())
                    .LifestyleSingleton()
                );
        }
    }
}
