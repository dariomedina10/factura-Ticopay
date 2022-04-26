namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddColumnIsRecurrentService : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO [TicoPay].[AbpPermissions]([TenantId],[RoleId],[Name],[IsGranted],[CreationTime],[CreatorUserId],[UserId],[Discriminator]) " +
                "VALUES (null,1, 'Reports.StatusInvoices', 1, GETDATE(), null, null, 'RolePermissionSetting')");

            Sql("INSERT INTO [TicoPay].[AbpPermissions]([TenantId],[RoleId],[Name],[IsGranted],[CreationTime],[CreatorUserId],[UserId],[Discriminator]) " +
                "(select t.Id as TenantId, r.Id as RolId, 'Reports.StatusInvoices', 1, GETDATE(), null, null, 'RolePermissionSetting' " +
                "from [TicoPay].AbpTenants t inner join[TicoPay].[AbpRoles] r on t.Id = r.TenantId where t.IsActive = 1 and r.Name = 'Admin')");

            AddColumn("TicoPay.Services", "IsRecurrent", c => c.Boolean(nullable: false, defaultValue: true));
        }

        public override void Down()
        {
            DropColumn("TicoPay.Services", "IsRecurrent");
        }
    }
}
