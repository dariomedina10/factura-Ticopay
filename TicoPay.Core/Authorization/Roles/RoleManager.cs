using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicoPay.Users;
using System.Linq;
using LinqKit;
using System.Linq.Expressions;

namespace TicoPay.Authorization.Roles
{
    public class RoleManager : AbpRoleManager<Role, User>
    {
        public readonly IRepository<Role> _roleRepository;
        public readonly IRepository<UserRole, long> _userRoleRepository;
        public readonly IUnitOfWorkManager _unitOfWorkManager;

        public RoleManager(
            RoleStore store,
            IPermissionManager permissionManager,
            IRoleManagementConfig roleManagementConfig,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager, IRepository<Role> roleRepository, IRepository<UserRole, long> userRoleRepository)
            : base(
                store,
                permissionManager,
                roleManagementConfig,
                cacheManager,
                unitOfWorkManager)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public List< Role> Get()
        {
            var superAdminRol = _roleRepository.GetAllList(r => r.Name == StaticRoleNames.Host.SuperAdmin).FirstOrDefault();
            var predicate = PredicateBuilder.New<Role>(true);
            if (AbpSession != null && AbpSession.UserId != null && superAdminRol != null)
            {
                var canAddSuperAdminRol = _userRoleRepository.Count(u => u.UserId == AbpSession.UserId && u.RoleId == superAdminRol.Id) > 0;
                if (!canAddSuperAdminRol)
                {
                    predicate = predicate.And(d => d.Id != superAdminRol.Id);
                }
            }

            var @roles = _roleRepository.GetAllList(predicate);


            if (@roles == null)
            {
                throw new UserFriendlyException("Could not found the Roles, may be it's deleted!");
            }
            return @roles;
        }

        public int GetRoleByUser(long IdUser)
        {
            var @userrol = _userRoleRepository.FirstOrDefault(a => a.UserId == IdUser);


            if (@userrol == null)
            {
                throw new UserFriendlyException("Could not found the Roles, may be it's deleted!");
            }
            return @userrol.RoleId;
        }

        public void UpdateUserRole(long IdUserRole, int Idrole)
        {
            var userRole = _userRoleRepository.Get(IdUserRole);
            userRole.RoleId = Idrole;
            _userRoleRepository.Update(userRole);
        }
        
        public override async Task<IdentityResult> CheckDuplicateRoleNameAsync(int? expectedRoleId, string name, string displayName)
        {
            var role = await FindByNameAsync(name);
            if (role != null && role.Id != expectedRoleId)
            {
                throw new UserFriendlyException("Ya existe un rol con ese nombre");
            }

            role = await FindByDisplayNameAsync(displayName);
            if (role != null && role.Id != expectedRoleId)
            {
                throw new UserFriendlyException("Ya existe un rol con ese nombre");
            }

            return IdentityResult.Success;
        }

        private Task<Role> FindByDisplayNameAsync(string displayName)
        {
            return AbpStore.FindByDisplayNameAsync(displayName);
        }

        public Role FindAdminRole(int TenantId)
        {
            _unitOfWorkManager.Current.SetFilterParameter(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, TenantId);

            return _roleRepository.GetAll().Where(d => d.TenantId == TenantId && d.Name == "Admin").FirstOrDefault();
        }

        public Role FindSuperAdminRole(int TenantId)
        {
            _unitOfWorkManager.Current.SetFilterParameter(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, TenantId);

            return _roleRepository.GetAll().Where(d => d.TenantId == TenantId && d.Name == "SuperAdmin").FirstOrDefault();
        }

        public Role FindNameRol(int id)
        {
            return _roleRepository.GetAll().Where(d => d.Id == id).FirstOrDefault();
        }
    }
}