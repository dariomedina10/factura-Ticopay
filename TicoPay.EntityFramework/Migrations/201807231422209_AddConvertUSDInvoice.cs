namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConvertUSDInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "IsConvertUSD", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "IsConvertUSD");
        }
    }
}
