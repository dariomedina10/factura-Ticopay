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
using System.Data.Entity;

namespace TicoPay.Migrations.SeedData
{
    public class UpdatePermissionAccess
    {
        private readonly TicoPayDbContext _context;

        public UpdatePermissionAccess(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create(string permission)
        {
            //if (permission == PermissionNames.Reports_InvoicesSentToTribunet)
            //{
            //    if (_context.Roles.Count(r => r.Permissions.Any(d => d.Name == PermissionNames.Reports_InvoicesSentToTribunet)) > 0)
            //        DeletePermission(PermissionNames.Reports_InvoicesSentToTribunet); 
            //}
            if (permission == PermissionNames.Reports_InvoicesNotes)
            {
                if (_context.Roles.Count(r => r.Permissions.Any(d => d.Name != PermissionNames.Reports_InvoicesNotes) && r.Name == StaticRoleNames.Host.Admin) > 0)
                    AddPermission(PermissionNames.Reports_InvoicesNotes); 
            }
            //if (permission == PermissionNames.Billing_ConfirmXML)
            //{
            //    if (_context.Roles.Count(r => r.Permissions.Any(d => d.Name != PermissionNames.Billing_ConfirmXML) && r.Name == StaticRoleNames.Host.Admin) > 0)
            //        AddPermission(PermissionNames.Billing_ConfirmXML);
            //}
            if (permission == PermissionNames.Integration)
            {
                if (_context.Roles.Count(r => r.Permissions.Any(d => d.Name != PermissionNames.Integration) && r.Name == StaticRoleNames.Host.Admin) > 0)
                    AddPermission(PermissionNames.Integration);
                if (_context.Roles.Count(r => r.Permissions.Any(d => d.Name != PermissionNames.Integration_SVConta) && r.Name == StaticRoleNames.Host.Admin) > 0)
                    AddPermission(PermissionNames.Integration_SVConta);
                if (_context.Roles.Count(r => r.Permissions.Any(d => d.Name != PermissionNames.Integration_Zoho) && r.Name == StaticRoleNames.Host.Admin) > 0)
                    AddPermission(PermissionNames.Integration_Zoho);
            }
            if(permission == PermissionNames.Maintenance_Products)
            {
                if (_context.Roles.Count(r => r.Permissions.Any(d => d.Name != PermissionNames.Maintenance_Products) && r.Name == StaticRoleNames.Host.Admin) > 0)
                    AddPermission(PermissionNames.Maintenance_Products);
            }
            if (permission == PermissionNames.Dashboard)
            {
                if (_context.Roles.Count(r => r.Permissions.Any(d => d.Name != permission) && r.Name == StaticRoleNames.Host.Admin) > 0)
                    AddPermission(permission);
            }

        }

        //private void DeletePermission(string permissionName)
        //{
        //    var userRols = _context.Roles.Where(r => r.Permissions.Any(d => d.Name == PermissionNames.Reports_InvoicesSentToTribunet));
        //    foreach (var rol in userRols)
        //    {
        //        var permission = rol.Permissions.FirstOrDefault(d => d.Name == permissionName);
        //        _context.Entry(permission).State = EntityState.Deleted;
        //    }
        //    _context.SaveChanges();
        //}

        private void AddPermission(string permissionName)
        {
            var userRols = _context.Roles.Where(r => !r.Permissions.Any(d => d.Name == permissionName) && r.Name == StaticRoleNames.Host.Admin);
            foreach (var rol in userRols)
            {
                rol.Permissions.Add(new RolePermissionSetting
                {
                    Name = permissionName,
                    IsGranted = true,
                    RoleId = rol.Id,
                    TenantId = rol.TenantId
                });
            }
            _context.SaveChanges();
            
        }
    }
}
