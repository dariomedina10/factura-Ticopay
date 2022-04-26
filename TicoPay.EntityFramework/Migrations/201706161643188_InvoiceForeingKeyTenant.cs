namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceForeingKeyTenant : DbMigration
    {
        public override void Up()
        {
            CreateIndex("TicoPay.Invoices", "TenantId");
            AddForeignKey("TicoPay.Invoices", "TenantId", "TicoPay.AbpTenants", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.Invoices", "TenantId", "TicoPay.AbpTenants");
            DropIndex("TicoPay.Invoices", new[] { "TenantId" });
        }
    }
}
