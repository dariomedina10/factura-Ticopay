using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AutoMapper;
using TicoPay.Authorization;
using TicoPay.Users.Dto;
using Microsoft.AspNet.Identity;
using PagedList;
using Abp.UI;
using System.Net.Mail;
using System.Net.Mime;
using System;
using System.Web.Mvc;
using TicoPay.Authorization.Roles;
using Abp.Domain.Uow;
using Abp.Authorization.Users;
using System.Data.Entity;
using LinqKit;
using TicoPay.Drawers;
using Abp.Runtime.Session;

namespace TicoPay.Users
{
    /* THIS IS JUST A SAMPLE. */
    // [AbpAuthorize(PermissionNames.Pages_Users)]
    public class UserAppService : TicoPayAppServiceBase, IUserAppService
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly UserStore _userStoreRepository;
        private readonly IPermissionManager _permissionManager;
        private readonly RoleManager _roleManager;
        public readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserManager _userManager;
        private readonly IRepository<Drawer, Guid> _drawerRepository;
        private readonly IDrawersAppService  _drawersAppService;
        public readonly IRepository<Role> _roleRepository;
        //private readonly IRepository<DrawerUser, Guid> _drawerUserRepository;

        public UserAppService(IRepository<User, long> userRepository, IPermissionManager permissionManager, RoleManager roleManager,
                              IRepository<UserRole, long> userRoleRepository, UserStore userStoreRepository,
                              IUnitOfWorkManager unitOfWorkManager, UserManager userManager, IRepository<Drawer, Guid> drawerRepository, IDrawersAppService drawersAppService,
                              IRepository<Role> roleRepository)
        {
            _userRepository = userRepository;
            _permissionManager = permissionManager;
            _roleManager = roleManager;
            _userRoleRepository = userRoleRepository;
            _userStoreRepository = userStoreRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _userManager = userManager;
            _drawerRepository = drawerRepository;
            _drawersAppService = drawersAppService;
            _roleRepository = roleRepository;
        }

        public async Task ProhibitPermission(ProhibitPermissionInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            var permission = _permissionManager.GetPermission(input.PermissionName);

            await UserManager.ProhibitPermissionAsync(user, permission);
        }

        public IList<User> GetAllListUsersSoftDelete()
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var predicate = PredicateBuilder.New<User>(true);

                var superAdminRol = _roleRepository.GetAllList(r => r.Name == StaticRoleNames.Host.SuperAdmin).FirstOrDefault();
                if (AbpSession != null && AbpSession.UserId != null && superAdminRol != null)
                {
                    var canAddSuperAdminRol = _userRoleRepository.Count(u => u.UserId == AbpSession.UserId && u.RoleId == superAdminRol.Id) > 0;
                    if (!canAddSuperAdminRol)
                    {
                        predicate = predicate.And(c => c.Roles.Any(d => d.RoleId != superAdminRol.Id));
                    }
                }

                var users = _userRepository.GetAllList(predicate);
                return users;
            }
        }

        //Example for primitive method parameters.
        public async Task RemoveFromRole(long userId, string roleName)
        {
            CheckErrors(await UserManager.RemoveFromRoleAsync(userId, roleName));
        }

        public async Task<ListResultDto<UserListDto>> GetUsers()
        {

            var users = await _userRepository.GetAllListAsync();
            if (users == null)
            {
                throw new UserFriendlyException("Could not found the users, maybe it's deleted.");
            }

            return new ListResultDto<UserListDto>(
                users.MapTo<List<UserListDto>>()
                );
        }
        public async Task CreateUser(CreateUserInput input)
        {
            var user = input.MapTo<User>();

            user.TenantId = AbpSession.TenantId;
            user.Password = new PasswordHasher().HashPassword(input.Password);
            user.IsEmailConfirmed = true;
            user.Name = input.Name;
            user.Surname = input.Surname;
            user.IsActive = true;
            CheckErrors(await UserManager.CreateAsync(user));
        }

        public IPagedList<UserListDto> SearchUsers(SearchUsersInput searchInput)
        {
            int currentPageIndex = searchInput.Page.HasValue ? searchInput.Page.Value : 1;

            var predicate = PredicateBuilder.New<User>(true);

            if (searchInput.NameFilter != null)
                predicate = predicate.And(c => c.Name.Contains(searchInput.NameFilter));

            if (searchInput.SurnameFilter != null)
                predicate = predicate.And(c => c.Surname.Contains(searchInput.SurnameFilter));

            if (searchInput.EmailAddressFilter != null)
                predicate = predicate.And(c => c.EmailAddress.Contains(searchInput.EmailAddressFilter));

            var superAdminRol = _roleRepository.GetAllList(r => r.Name == StaticRoleNames.Host.SuperAdmin).FirstOrDefault();
            if (AbpSession != null && AbpSession.UserId != null && superAdminRol != null)
            {
                var canAddSuperAdminRol = _userRoleRepository.Count(u => u.UserId == AbpSession.UserId && u.RoleId == superAdminRol.Id) > 0;
                if (!canAddSuperAdminRol)
                {
                    predicate = predicate.And(c => c.Roles.Any(d => d.RoleId != superAdminRol.Id)); 
                }
            }

            if (searchInput.IdRolFilter != null && searchInput.IdRolFilter != 0)
                predicate = predicate.And(c => c.Roles.Any(d => d.RoleId == searchInput.IdRolFilter));

            var users = _userRepository.GetAll().Where(predicate);

            if (users == null)
                throw new UserFriendlyException("Could not found the users, maybe it's deleted.");
            return users.OrderByDescending(p => p.Name).MapTo<List<UserListDto>>().ToPagedList(currentPageIndex, searchInput.MaxResultCount);
        }

        public User GetUserByRole(string name)
        {
            Role role = _roleManager.FindByName(name);
            if (role == null)
            {
                throw new UserFriendlyException("Could not found the role, maybe it's deleted.");
            }
            var user = _userRepository.GetAll().Include(u => u.Roles).Where(u => u.Roles.Any(r => r.RoleId == role.Id)).FirstOrDefault();
            if (user == null)
            {
                throw new UserFriendlyException("Could not found the user, maybe it's deleted.");
            }

            return user;
        }

        public User GetUserByRole(string name, int tenantId)
        {
            Role role = _roleManager.FindAdminRole(tenantId);
            if (role == null)
            {
                throw new UserFriendlyException("Could not found the role, maybe it's deleted.");
            }
            var user = _userRepository.GetAll().Include(u => u.Roles).Where(u => u.Roles.Any(r => r.RoleId == role.Id) && u.TenantId == tenantId).FirstOrDefault();
            if (user == null)
            {
                throw new UserFriendlyException("Could not found the user, maybe it's deleted.");
            }

            return user;
        }

        [UnitOfWork]
        public async Task Update(UpdateUserInput input)
        {
            try
            {
                var @User = _userRepository.Get(input.Id);

                if (EmailExist(input.EmailAddress, input.Id))
                    throw new UserFriendlyException("Existe una Usuario con el mismo Correo.");

                @User.Name = input.Name;
                @User.Surname = input.Surname;

                @User.EmailAddress = input.EmailAddress;
                @User.UserName = input.UserName;
                @User.Password = new PasswordHasher().HashPassword(input.Password);


                var user = await UserManager.GetRolesAsync(input.Id);
                foreach (var p in user)
                {
                    var result = await UserManager.RemoveFromRoleAsync(input.Id, p);
                }


                @User.Roles.Add(new UserRole { RoleId = input.IdRol, TenantId = AbpSession.TenantId.Value, UserId = input.Id });

                _userRepository.Update(@User);


            }
            catch (Exception)
            {

            }
        }


        public bool EmailExistCreate(string email)
        {
            var @entity = _userRepository.FirstOrDefault(e => e.EmailAddress.Equals(email));
            if (@entity == null)
            {
                //throw new UserFriendlyException("Could not found the user, maybe it's deleted.");
            }
            return @entity != null;
        }

        public bool EmailExist(string email, long id)
        {
            var @entity = _userRepository.FirstOrDefault(e => e.EmailAddress.Equals(email) && e.Id != id);
            if (@entity == null)
            {
                //throw new UserFriendlyException("Could not found the user, maybe it's deleted.");
            }
            return @entity != null;
        }

        public bool UserNameExist(string userName)
        {
            userName = userName.ToLower();
            var @entity = _userRepository.FirstOrDefault(e => e.UserName.ToLower().Equals(userName));
            if (@entity == null)
            {
                //throw new UserFriendlyException("Could not found the user, maybe it's deleted.");
            }
            return @entity != null;
        }

        public void ChangeStatus(long id)
        {
            var @entity = _userRepository.Get(id);
            if (@entity == null)
            {
                throw new UserFriendlyException("No se pudo encontrar el usuario, probablemente fue eliminado.");
            }
            if (@entity.IsActive)
                @entity.IsActive = false;
            else
                @entity.IsActive = true;
            _userRepository.Update(@entity);
        }

        public async Task UpdateDrawesUser(UpdateUserDrawers input)
        {

            var @User = _userRepository.Get(input.Id);

            var drawerOpen = _drawerRepository.GetAll().Where(x => x.IsOpen == true && x.UserIdOpen == input.Id).FirstOrDefault();

            if (drawerOpen != null)
            {                
                if ((input.ListDrawers == null)||(!input.ListDrawers.Any(x => x.Id == drawerOpen.Id)))
                {
                    throw new UserFriendlyException("El usuario no ha cerrado la caja: " + drawerOpen.Code + ", se debe cerrar la caja antes de quitar el acceso.");
                };
            }
            if (input.ListDrawers != null)
            {
                
                if (@User.DrawerUsers.Count > 0)
                {                 
                    var deletedrawers = (from sc in @User.DrawerUsers
                                         where !input.ListDrawers.Any(x => (x.Id == sc.DrawerId && x.IdDrawerUser == sc.Id))
                                         select sc
                                         ).ToList();

                    DeleteDrawesUser(@User, deletedrawers);
                }

                var addDrawers = (from sc in input.ListDrawers
                                  where !@User.DrawerUsers .Any(x => (x.DrawerId == sc.Id && x.Id == sc.IdDrawerUser))
                                     select new DrawerUser { DrawerId= sc.Id } 
                                        ).ToList();

                foreach (var item in addDrawers)
                {
                    var drawer = _drawerRepository.Get(item.DrawerId.Value);
                    @User.DrawerUsers.Add(new DrawerUser { DrawerId= drawer.Id, Drawer= drawer, IsActive=true });
                }
                //newServices = input.ClientServiceList.MapTo<List<Service>>();


            }
            else
            {
                DeleteDrawesUser(@User,@User.DrawerUsers.ToList());
            }
        }

        public IList<DrawerUser> getUserDrawers(Guid? IdBranch, Guid? IdDrawer=null)
        {
            var IdUser = AbpSession.GetUserId();
            if (IdUser == 0)
            {
                throw new UserFriendlyException("No posse uan sesion activa en la compañía");
            }
            var user = _userRepository.Get(IdUser);

            var lista = user.DrawerUsers.AsEnumerable();

            if (IdBranch != null)
                lista = lista.Where(x => x.Drawer != null && x.Drawer.BranchOfficeId == (Guid)IdBranch);

            if (IdDrawer != null)
                lista = lista.Where(x => x.DrawerId == (Guid)IdDrawer);

            return lista.ToList();
        }

        private void DeleteDrawesUser(User user, IList<DrawerUser> drawerUsers)
        {
            foreach (var item in drawerUsers)
            {
                user.DrawerUsers.Remove(item);
            }
        }
    }
}