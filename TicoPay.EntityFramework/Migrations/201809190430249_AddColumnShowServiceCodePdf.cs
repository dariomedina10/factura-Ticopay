namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnShowServiceCodePdf : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "ShowServiceCodePdf", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "ShowServiceCodePdf");
        }
    }
}
