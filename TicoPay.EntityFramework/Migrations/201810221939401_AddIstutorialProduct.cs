namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIstutorialProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "IsTutorialProduct", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "IsTutorialProduct");
        }
    }
}
