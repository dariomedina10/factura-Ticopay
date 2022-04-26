namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnPaymentInvoice : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO [TicoPay].[AbpPermissions]([TenantId],[RoleId],[Name],[IsGranted],[CreationTime],[CreatorUserId],[UserId],[Discriminator]) " +
                "VALUES (null,1, 'Reports.InvoicesNotes', 1, GETDATE(), null, null, 'RolePermissionSetting')");

            Sql("INSERT INTO [TicoPay].[AbpPermissions]([TenantId],[RoleId],[Name],[IsGranted],[CreationTime],[CreatorUserId],[UserId],[Discriminator]) " +
                "(select t.Id as TenantId, r.Id as RolId, 'Reports.InvoicesNotes', 1, GETDATE(), null, null, 'RolePermissionSetting' " +
                "from [TicoPay].AbpTenants t inner join[TicoPay].[AbpRoles] r on t.Id = r.TenantId where t.IsActive = 1 and r.Name = 'Admin')");

            AddColumn("TicoPay.PaymentInvoices", "IsPaymentReversed", c => c.Boolean());
            AddColumn("TicoPay.PaymentInvoices", "IsPaymentUsed", c => c.Boolean());
            AddColumn("TicoPay.PaymentInvoices", "ParentPaymentInvoiceId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.PaymentInvoices", "ParentPaymentInvoiceId");
            DropColumn("TicoPay.PaymentInvoices", "IsPaymentUsed");
            DropColumn("TicoPay.PaymentInvoices", "IsPaymentReversed");
        }
    }
}
