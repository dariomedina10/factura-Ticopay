using System.Linq;
using TicoPay.EntityFramework;

namespace TicoPay.Migrations.SeedData
{
    public class updateNavigationSeed
    {
        private readonly TicoPayDbContext _context;

        public updateNavigationSeed(TicoPayDbContext context)
        {
            _context = context;
        }

        public void create()
        {
            var isPermissions = _context.Permissions.Any(x => x.Name == "Billing.OpenDrawer");
            if (!isPermissions)
            {
              
                string sqlQuery = @"insert into TicoPay.AbpPermissions (TenantId, [Name], IsGranted, CreationTime, CreatorUserId, UserId, RoleId, Discriminator)
                    SELECT         TenantId, 'Billing.Billing', IsGranted, CreationTime, CreatorUserId, UserId, RoleId, Discriminator
                    FROM            TicoPay.AbpPermissions
                    WHERE([Name] = 'Billing') 
                    
                    INSERT INTO [TicoPay].[AbpPermissions]([TenantId],[RoleId],[Name],[IsGranted],[CreationTime],[CreatorUserId],[UserId],[Discriminator]) 
                    VALUES(null, 1, 'Billing.OpenDrawer', 1, GETDATE(), null, null, 'RolePermissionSetting')

                    INSERT INTO [TicoPay].[AbpPermissions]([TenantId],[RoleId],[Name],[IsGranted],[CreationTime],[CreatorUserId],[UserId],[Discriminator]) 
                    (select t.Id as TenantId, r.Id as RolId, 'Billing.OpenDrawer', 1, GETDATE(), null, null, 'RolePermissionSetting'  
                    from [TicoPay].AbpTenants t inner join[TicoPay].[AbpRoles] r on t.Id = r.TenantId where t.IsActive = 1 and r.Name = 'Admin')

                    INSERT INTO [TicoPay].[AbpPermissions]([TenantId],[RoleId],[Name],[IsGranted],[CreationTime],[CreatorUserId],[UserId],[Discriminator]) 
                    (select t.Id as TenantId, r.Id as RolId, 'Billing.OpenDrawer', 1, GETDATE(), null, null, 'RolePermissionSetting'  
                    from [TicoPay].AbpTenants t inner join[TicoPay].[AbpRoles] r on t.Id = r.TenantId where t.IsActive = 1 and r.Name = 'SuperAdmin')
                    
                    update TicoPay.AbpPermissions set [Name]='Billing.ConfirmXML' where [Name]='ConfirmXML'";

                var result= _context.Database.ExecuteSqlCommand(sqlQuery);
                
            }
        }
    }
}
