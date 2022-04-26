using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using TicoPay.EntityFramework;

namespace TicoPay.Migrator
{
    [DependsOn(typeof(TicoPayDataModule))]
    public class TicoPayMigratorModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer<TicoPayDbContext>(null);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}