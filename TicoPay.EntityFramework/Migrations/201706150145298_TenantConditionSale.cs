namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenantConditionSale : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TicoPay.AbpTenants", "ConditionSaleID", "TicoPay.Tipos");
            DropIndex("TicoPay.AbpTenants", new[] { "ConditionSaleID" });
            AddColumn("TicoPay.AbpTenants", "ConditionSaleType", c => c.Int(nullable: false));
            DropColumn("TicoPay.AbpTenants", "ConditionSaleID");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.AbpTenants", "ConditionSaleID", c => c.Int());
            DropColumn("TicoPay.AbpTenants", "ConditionSaleType");
            CreateIndex("TicoPay.AbpTenants", "ConditionSaleID");
            AddForeignKey("TicoPay.AbpTenants", "ConditionSaleID", "TicoPay.Tipos", "Id");
        }
    }
}
