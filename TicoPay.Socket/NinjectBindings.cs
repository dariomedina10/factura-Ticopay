using Abp.Domain.Repositories;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.EntityFramework;
using TicoPay.EntityFramework.Repositories;
using TicoPay.Services;
using TicoPay.Taxes;
using TicoPay.Users;
using Abp.Zero.EntityFramework;
using TicoPay.Authorization.Roles;
using TicoPay.MultiTenancy;
using Abp.EntityFramework.Repositories;
using Ninject.Extensions.Factory;
using Abp.Authorization.Users;
using Abp.Domain.Uow;
using Abp.Authorization.Roles;
using Abp.Authorization;
using Abp.Zero.Configuration;
using Abp.Runtime.Caching;
using Abp.Configuration.Startup;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Organizations;

namespace TicoPay
{
    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            //Bind<IRepository<Service, Guid>>().To<IRepository<Service, Guid>>().InSingletonScope();
            //Bind<IRepository<Tax, Guid>>().To<AbpRepositoryBase<Tax, Guid>>().InSingletonScope();
            //Bind<IRepository<ClientService, Guid>>().To<AbpRepositoryBase<ClientService, Guid>>().InSingletonScope();
            //Bind<IServiceManager>().To<ServiceManager>().InSingletonScope();
            //Bind<UserManager>().To<UserManager>().InSingletonScope();
            //Bind<TicoPayDbContext>().ToSelf().InTransientScope();
            //Bind<ContentService>().ToSelf().InRequestScope();
            //kernel.Bind<IContentRepository>().To<ContentRepository>().InRequestScope();
            //Bind<TicoPayDbContext>().ToConstructor(_ => new TicoPayDbContext());
            //Bind<AbpZeroDbContext<Tenant, Role, User>>().To<TicoPayDbContext>().InSingletonScope();
            //Bind(typeof(IRepository<,>)).To(typeof(Repository<,>)).InSingletonScope();

            //Bind(typeof(IRepository<,>)).To(typeof(AbpRepositoryBase<,>)).InSingletonScope();

            //Bind(typeof(IRepository<Service,Guid>)).ToFactory();
            //Bind(typeof(IRepository<Tax, Guid>)).ToFactory();

            Bind<IRepository<Service, Guid>>().ToFactory(() => new TypeMatchingArgumentInheritanceInstanceProvider());

           // Bind<IRepository<Service, Guid>>().ToFactory().InSingletonScope();
            Bind<IRepository<Tax, Guid>>().ToFactory();
            Bind<IRepository<ClientService, Guid>>().ToFactory();
            Bind<IRepository<User, long>>().ToFactory();
            Bind<IRepository<UserLogin, long>>().ToFactory();
            Bind<IRepository<UserRole, long>>().ToFactory();
            Bind<IRepository<Role, int>>().ToFactory();
            Bind<IRepository<Tenant>>().ToFactory();
            Bind<IRepository<Role>>().ToFactory();
            Bind<IRepository<UserPermissionSetting, long>>().ToFactory();
            Bind<IRepository<RolePermissionSetting, long>>().ToFactory();
            Bind<IRepository<OrganizationUnit, long>>().ToFactory();
            Bind<IRepository<UserOrganizationUnit, long>>().ToFactory();
            Bind<IRepository<UserLoginAttempt, long>>().ToFactory();


            Bind<IUnitOfWorkManager>().ToFactory();
            Bind<IPermissionManager>().ToFactory();
            Bind<IRoleManagementConfig>().ToFactory();
            Bind<ICacheManager>().ToFactory();
            Bind<IMultiTenancyConfig>().ToFactory();
            Bind<ISettingManager>().ToFactory();
            Bind<IUserManagementConfig>().ToFactory();
            Bind<IIocResolver>().ToFactory();
            Bind<IOrganizationUnitSettings>().ToFactory();
            //Bind<I>().ToFactory();



            Bind<RoleStore>().To<RoleStore>().InSingletonScope();
            Bind<UserStore>().To<UserStore>().InSingletonScope();

            Bind<IServiceManager>().To<ServiceManager>().InSingletonScope();
            
            Bind<UserManager>().To<UserManager>().InSingletonScope();
           
            //Bind(typeof(IRepository<,>)).To(typeof(IRepository<,>)).InSingletonScope();
            //Bind<IRepository<Service, Guid>>().ToSelf().InTransientScope();
            Bind<IServiceAppService>().To<ServiceAppService>();
           
         
        }
    }
}
