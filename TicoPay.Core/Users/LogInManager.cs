using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Authorization.Roles;
using TicoPay.MultiTenancy;
using Microsoft.AspNet.Identity;
using Abp.Extensions;
using Abp.Timing;
using System.Security.Claims;

namespace TicoPay.Users
{
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {

        public LogInManager(
            UserManager userManager,
            RoleManager roleManager,
            IMultiTenancyConfig multiTenancyConfig,
            IRepository<Tenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver) : base(
                userManager,
                multiTenancyConfig,
                tenantRepository,
                unitOfWorkManager,
                settingManager,
                userLoginAttemptRepository,
                userManagementConfig,
                iocResolver,
                roleManager)
        {
        }

        protected async override Task<AbpLoginResult<Tenant, User>> CreateLoginResultAsync(User user, Tenant tenant = null)
        {
            if (!user.IsActive)
            {
                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.UserIsNotActive);
            }

            //if (!user.IsEmailConfirmed)
            //{
            //    return new AbpLoginResult<Tenant, User>(AbpLoginResultType.UserEmailIsNotConfirmed);
            //}

            user.SecurityStamp = Guid.NewGuid().ToString();
            return await base.CreateLoginResultAsync(user, tenant);
        }
    }
}
