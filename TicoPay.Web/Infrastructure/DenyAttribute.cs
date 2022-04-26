using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace TicoPay.Web.Infrastructure
{
    public class DenyAttribute : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            IPrincipal user = httpContext.User;

            if (Users.Length > 0 && Users.Split(',').Any(u => string.Compare(u.Trim(), user.Identity.Name, true) == 0))
            {
                return false;
            }

            if (Roles.Length > 0 && Roles.Split(',').Any(u => user.IsInRole(u.Trim())))
            {
                return false;
            }

            return true;
        }

    }
}