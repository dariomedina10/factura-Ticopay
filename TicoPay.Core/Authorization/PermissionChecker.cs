using Abp.Authorization;
using TicoPay.Authorization.Roles;
using TicoPay.MultiTenancy;
using TicoPay.Users;

namespace TicoPay.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
