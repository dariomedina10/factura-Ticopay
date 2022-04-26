using System.Linq;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using TicoPay.Authorization;
using TicoPay.Authorization.Roles;
using TicoPay.EntityFramework;
using TicoPay.Users;
using Microsoft.AspNet.Identity;
using TicoPay.MultiTenancy;
using Abp.Configuration;
using System.Collections.Generic;
using System;

namespace TicoPay.Migrations.SeedData
{
    public class SupportRoleAndUserCreator
    {
        private readonly TicoPayDbContext _context;
        public const string DefaultPassword = "s0portE2018";
        public SupportRoleAndUserCreator(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {

            var tenants = _context.Tenants.Where(r => r.IsActive && !r.IsDeleted).Select(h => h.Id).ToList();

            foreach (var Id in tenants)
            {

                var supportTeam = new User[] {
                new User { UserName = "crojas", Name = "Carlos", Surname = "Rojas", EmailAddress = "soporte@ticopays.com", IsEmailConfirmed = true, Password = new PasswordHasher().HashPassword(DefaultPassword) },
                //new User { UserName = "vfiguera", Name = "Veronica", Surname = "Figuera", EmailAddress = "soporte@ticopays.com", IsEmailConfirmed = true, Password = new PasswordHasher().HashPassword(DefaultPassword) },
                //new User { UserName = "jmarin", Name = "Jorge", Surname = "Marin", EmailAddress = "soporte@ticopays.com", IsEmailConfirmed = true, Password = new PasswordHasher().HashPassword(DefaultPassword) },
                //new User { UserName = "pmacchiarulo", Name = "Pablo", Surname = "Macchiarulo", EmailAddress = "soporte@ticopays.com", IsEmailConfirmed = true, Password = new PasswordHasher().HashPassword(DefaultPassword) },
                };

                foreach (var member in supportTeam)
                {
                    try
                    {
                        CreateHostRoleAndUsers(member, Id);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine("Verificar usuario se soporte " + member.UserName + " en tenant " + Id);
                    }
                }

            }
        }

        private void CreateSuperAdminRol(int Id)
        {
            var superAdmin = _context.Roles.FirstOrDefault(r => r.TenantId == Id && r.Name == StaticRoleNames.Host.SuperAdmin);
            if (superAdmin == null)
            {
                superAdmin = _context.Roles.Add(new Role { Name = StaticRoleNames.Host.SuperAdmin, DisplayName = "Super Administrador", IsStatic = true, TenantId = Id });
                _context.SaveChanges();

                //Grant all tenant permissions
                var permissions = PermissionFinder
                    .GetAllPermissions(new TicoPayAuthorizationProvider())
                    .ToList();

                foreach (var permission in permissions)
                {
                    _context.Permissions.Add(
                        new RolePermissionSetting
                        {
                            Name = permission.Name,
                            IsGranted = true,
                            RoleId = superAdmin.Id,
                            TenantId = Id
                        });
                }

                _context.SaveChanges();
            }
        }

        private void CreateHostRoleAndUsers(User member, int Id, Boolean repair = false)
        {

            CreateSuperAdminRol(Id);

            var supportUser = _context.Users.FirstOrDefault(u => u.TenantId == Id && u.UserName == member.UserName);
            var superAdmin = _context.Roles.FirstOrDefault(r => r.TenantId == Id && r.Name == StaticRoleNames.Host.SuperAdmin);

            if (supportUser == null)
            {
                member.TenantId = Id;
                supportUser = _context.Users.Add(member);
                _context.SaveChanges();
                member.Settings = new List<Setting>();
                member.Settings.Add(new Setting { UserId = supportUser.Id, Name = Abp.Localization.LocalizationSettingNames.DefaultLanguage, Value = "en", TenantId = Id });
                _context.SaveChanges();
                _context.UserRoles.Add(new UserRole(Id, supportUser.Id, superAdmin.Id));
                _context.SaveChanges();
            }
            else
            {
                if (repair)
                {
                    var setting = supportUser.Settings.Where(s => s.Name == Abp.Localization.LocalizationSettingNames.DefaultLanguage).FirstOrDefault();
                    if (setting == null)
                    {
                        supportUser.Settings.Add(new Setting { UserId = supportUser.Id, Name = Abp.Localization.LocalizationSettingNames.DefaultLanguage, Value = "en", TenantId = Id });
                        _context.SaveChanges();
                    }
                    var userRol = supportUser.Roles.Any(r => r.RoleId == superAdmin.Id);
                    if (!userRol)
                    {
                        _context.UserRoles.Add(new UserRole(Id, supportUser.Id, superAdmin.Id));
                        _context.SaveChanges();
                    }
                }
            }
        }

    }
}