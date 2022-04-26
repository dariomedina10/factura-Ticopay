using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using TicoPay.Authorization.Roles;
using TicoPay.Roles.Dto;
using Abp.Authorization.Users;
using PagedList;
using System.Collections.Generic;
using Abp.Domain.Repositories;
using Abp.AutoMapper;
using Abp.UI;
using TicoPay.Users;
using Abp.Authorization.Roles;
using Abp.Application.Services.Dto;

namespace TicoPay.Roles
{
    /* THIS IS JUST A SAMPLE. */
    public class RoleAppService : TicoPayAppServiceBase,IRoleAppService
    {
        private readonly RoleManager _roleManager;
        private readonly IPermissionManager _permissionManager;
        public readonly IRepository<Role, int> _roleRepository;
        public readonly IRepository<UserRole, long> _userRoleRepository;
        public readonly UserManager _userManager;

        public RoleAppService(RoleManager roleManager, IPermissionManager permissionManager, IRepository<Role, int> roleRepository,
            IRepository<UserRole, long> userRoleRepository, UserManager userManager)
        {
            _roleManager = roleManager;
            _permissionManager = permissionManager;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userManager = userManager;
        }

        public async Task UpdateRolePermissions(UpdateRolePermissionsInput input)
        {
            var role = await _roleManager.GetRoleByIdAsync(input.RoleId);
            var grantedPermissions = _permissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissionNames.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
        }

        public async Task UpdateRolePermissions(User user,string name)
        {
            var grantedPermissions = _permissionManager
                .GetAllPermissions()
                .Where(p => p.Name.Contains(name))
                .ToList();

            foreach (var role in user.Roles)
            {
                await _roleManager.SetGrantedPermissionsAsync(role.RoleId, grantedPermissions); 
            }
        }

        public async Task Update(UpdateRoleInput input)
        {
            var role = await _roleManager.GetRoleByIdAsync(input.Id);
            List<string> grantedPermisos = new List<string>();

            foreach (var data in input.permisos)
            {
                var node = data.ToString();

                var nodeList = node.Split(',');

                Permission permission = null;

                foreach (var per in nodeList)
                    {
                        permission = PermissionManager.GetPermission(per.Trim());

                        if (permission.Parent != null)
                            grantedPermisos.Add(per.Trim());

                        if ((permission.Parent == null) && (!grantedPermisos.Exists(x => x.ToString() == permission.Name.Trim())))
                            grantedPermisos.Add(permission.Name.Trim());
                    }
                
            }
            var grantedPermissions = _permissionManager
                .GetAllPermissions()
                .Where(p => grantedPermisos.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
        }

        public int GetRoleByUser(long IdUser)
        {
            var role = _roleManager.GetRoleByUser(IdUser);

            //if (role == null)
            //{
            //    throw new UserFriendlyException("Could not found the role, maybe it's deleted.");
            //}
            return role;
        }

        public IList<UserRole> GetAllListUserRoles()
        {
            var userroles = _userRoleRepository.GetAllList();

            if (userroles == null)
            {
                throw new UserFriendlyException("Could not found the role, maybe it's deleted.");
            }
            return userroles;
        }

        public ListResultDto<RolesDTO> GetAllRoles()
        {
            var roles = _roleRepository.GetAllList();
            return new ListResultDto<RolesDTO>(roles.MapTo<List<RolesDTO>>());
        }

        public ListResultDto<UserRoleDto> GetUserRoles(long UserId)
        {
            var roles = (from u in _userRoleRepository.GetAll() join r in _roleRepository.GetAll() on u.RoleId equals r.Id
                         where u.UserId== UserId
                         select new UserRoleDto {
                             UserId = u.UserId,
                             RoleId = u.RoleId,                          

                             Role = new RolesDTO
                             {
                                 Id = r.Id,
                                 Name = r.Name,
                                 CreationTime = r.CreationTime,
                                 LastModificationTime = r.LastModificationTime
                             }
                         }) ;

            

            return new ListResultDto<UserRoleDto>(roles.MapTo<List<UserRoleDto>>());
        }

        public async Task<ListResultDto<PermissionListDto>> getPermissions(long UserId)
        {
            var user = await _userManager.GetUserByIdAsync(UserId);           
            var permisos = await _userManager.GetGrantedPermissionsAsync(user);
            return new ListResultDto<PermissionListDto>(permisos.MapTo<List<PermissionListDto>>());
        }

        public RolDetailOutput GetDetail(int input)
        {
            var role = _roleRepository.FirstOrDefault(input);

            if (role == null)
            {
                throw new UserFriendlyException("Could not found the role, maybe it's deleted.");
            }

            return role.MapTo<RolDetailOutput>();
        }

        public IList<User> GetAllUser()
        {
            var users = _userManager.Users.ToList();

            if (users == null)
            {
                throw new UserFriendlyException("Could not found the users, maybe it's deleted.");
            }
            return users;
        }

        public List<PermissionListDto> GetAllPermission()
        {
            var permissions = PermissionManager.GetAllPermissions();
            var rootPermissions = permissions.Where(p => p.Parent == null);

            List<PermissionListDto> retValue = rootPermissions.MapTo<List<PermissionListDto>>();

            if (retValue == null)
            {
                throw new UserFriendlyException("Could not found the prmissions, maybe it's deleted.");
            }

            return retValue;
        }

        public async Task<bool> Create(CreateRoleInput input)
        {
            try
            {
                Role role = new Role();
                var permissionList = new List<Permission>();

                role.DisplayName = input.Name; // asigna los datos del rol
                role.Name = input.Name;
                role.IsDefault = false;
                role.IsStatic = false;
                role.TenantId = AbpSession.TenantId.Value;
                role.CreatorUserId = AbpSession.UserId.Value;

                //arma la lista de permisos

                foreach (var data in input.permisos)
                {
                    var node = data.ToString();

                    var nodeList = node.Split(',');

                    Permission permission = null;


                    foreach (var per in nodeList)
                    {
                        permission = PermissionManager.GetPermission(per.Trim());
                        
                        permissionList.Add(permission);
                    }
                }

                CheckErrors(await _roleManager.CreateAsync(role));
                await CurrentUnitOfWork.SaveChangesAsync(); //To get new role's id.

                await _roleManager.SetGrantedPermissionsAsync(role.Id, permissionList); // asigna los permisos 
            }
            catch (Exception)
            {

                return false;

            }

            return true;
        }

        public IPagedList<RolesDTO> SearchRoles(SearchRolesInput searchInput)
        {
            if (searchInput.Query == null)
                searchInput.Query = "";
            else
                searchInput.Query = searchInput.Query.ToLower();

            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value : 1;

            
            var roles = _roleRepository.GetAll().Where(c => c.Name.Contains(searchInput.Query)
                                                                || c.Name.Contains(searchInput.Query)).OrderByDescending(p => p.Name);
            return roles.MapTo<List<RolesDTO>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount);

        }
    }
}