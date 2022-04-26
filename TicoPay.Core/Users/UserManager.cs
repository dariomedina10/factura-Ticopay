using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.Localization;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Zero.Configuration;
using TicoPay.Authorization.Roles;
using TicoPay.MultiTenancy;
using System.Collections.Generic;

namespace TicoPay.Users
{
    public class UserManager : AbpUserManager<Role, User>
    {
        public UserManager(
            UserStore store,
            RoleManager roleManager,
            IRepository<Tenant> tenantRepository,
            IMultiTenancyConfig multiTenancyConfig,
            IPermissionManager permissionManager,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver,
            ICacheManager cacheManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IOrganizationUnitSettings organizationUnitSettings,
            ILocalizationManager localizationManager,
            IdentityEmailMessageService emailService,
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            IUserTokenProviderAccessor userTokenProviderAccessor)
            : base(
                store,
                roleManager,
                permissionManager,
                unitOfWorkManager,
                cacheManager,
                organizationUnitRepository,
                userOrganizationUnitRepository,
                organizationUnitSettings,
                localizationManager,
                emailService,
                settingManager,
                userTokenProviderAccessor)
        {
        }

        public override Task<bool> IsEmailConfirmedAsync(long userId)
        {
            return base.IsEmailConfirmedAsync(userId);
        }
    }
}