using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using Abp.Threading.BackgroundWorkers;
using TicoPay;
using TicoPay.EntityFramework;

namespace WorkerInvoiceCreator
{
    [DependsOn(typeof(TicoPayCoreModule), typeof(TicoPayApplicationModule), typeof(TicoPayDataModule))]
    public class WorkerRole : AbpModule
    {
        //public override void PreInitialize()
        //{
        //    Configuration.BackgroundJobs.IsJobExecutionEnabled = true;
        //    Database.SetInitializer<TicoPayDbContext>(null);
            
        //}

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());


            //        IocManager.IocContainer.Register(Component.For(
            //typeof(IRepository<>))
            //    .ImplementedBy(typeof(TicoPayRepositoryBase<>)).LifestyleTransient().Named("IRepositoryImplementation"));
            //        IocManager.IocContainer.Register(Component.For(
            //            typeof(IRepository<,>))
            //                .ImplementedBy(typeof(TicoPayRepositoryBase<,>)).LifestyleTransient().Named("IRepositoryOfPrimaryKeyImplementation"));
        }

        public override void PostInitialize()
        {
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<WorkerProcess>());
        }
    }
}
