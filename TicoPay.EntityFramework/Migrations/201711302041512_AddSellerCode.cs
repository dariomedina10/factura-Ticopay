namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSellerCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "SellerCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "SellerCode");
        }
    }
}
