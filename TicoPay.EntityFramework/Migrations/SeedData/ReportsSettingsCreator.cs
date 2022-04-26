using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.MultiTenancy;
using EntityFramework.DynamicFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Authorization;
using TicoPay.Authorization.Roles;
using TicoPay.Core;
using TicoPay.EntityFramework;
using TicoPay.ReportsSettings;

namespace TicoPay.Migrations.SeedData
{
    public class ReportsSettingsCreator
    {
        private readonly TicoPayDbContext _context;

        public ReportsSettingsCreator(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            //TODO: se eliminan de la bdd las configuraciones de notas ya que se debe primero ajustar el pdf de las notas al formato de factura
            var notaSettings = _context.ReportSettings.Where(r => r.ReportName == "Nota").ToList();
            notaSettings.ForEach(n => _context.ReportSettings.Remove(n));
            //End TODO

            var tenants = _context.Tenants.ToList();
            var reports = Enum.GetValues(typeof(TicoPayReports));
            foreach (var report in reports)
            {
                //TODO: Se debe quitar esta condicion cuando se agrege las notas a la lista
                if(report.ToString() == "Nota")
                {
                    continue;
                }

                foreach (var tenant in tenants)
                {
                    var exist = _context.ReportSettings.Any(r => r.TenantId == tenant.Id && r.ReportName == report.ToString());
                    if (!exist)
                    {
                        var reportSettings = new ReportsSettings.ReportSettings
                        {
                            TenantId = tenant.Id,
                            ReportName = report.ToString(),
                            ReportTitle = ((TicoPayReports)report).GetDescription()
                        };
                        _context.ReportSettings.Add(reportSettings);
                    }
                    GrandPermissionToAdminRole(tenant.Id);
                }
            }

        }

        private void GrandPermissionToAdminRole(int tenantId)
        {
            var adminRole = _context.Roles.FirstOrDefault(r => r.TenantId == tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                return;
            }

            var permissions = PermissionFinder
                .GetAllPermissions(new TicoPayAuthorizationProvider())
                .Where(p => p.Name == PermissionNames.Pages_ReportSettings)
                .ToList();

            foreach (var permission in permissions)
            {
                var exist = _context.Permissions.Any(p => p.TenantId == tenantId && p.Name == PermissionNames.Pages_ReportSettings);
                if (exist)
                {
                    continue;
                }

                _context.Permissions.Add(
                    new RolePermissionSetting
                    {
                        TenantId = tenantId,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = adminRole.Id
                    });
            }

            _context.SaveChanges();
        }
    }
}