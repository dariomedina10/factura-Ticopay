namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PosPrinter : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "IsPos", c => c.Boolean(nullable: false));
            AddColumn("TicoPay.AbpTenants", "PrinterType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "PrinterType");
            DropColumn("TicoPay.AbpTenants", "IsPos");
        }
    }
}
