using System;
using System.Collections.Generic;
using Abp.Authorization.Users;
using Abp.Extensions;
using Microsoft.AspNet.Identity;
using TicoPay.Drawers;

namespace TicoPay.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress, string password)
        {
            return new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Password = new PasswordHasher().HashPassword(password)
            };
        }

        public static User CreateTenantUser(int tenantId, string userName, string name, string surname, string emailAddress, string password)
        {
            return new User
            {
                TenantId = tenantId,
                UserName = userName,
                Name = name,
                Surname = surname,
                EmailAddress = emailAddress,
                Password = new PasswordHasher().HashPassword(password),
                DrawerUsers = new List<DrawerUser>()
            };
        }

        public virtual ICollection<DrawerUser> DrawerUsers { get; set; }

        internal void AssignDrawers(DrawerUser drawer)
        {
            DrawerUsers.Add(drawer);

        }
    }
}