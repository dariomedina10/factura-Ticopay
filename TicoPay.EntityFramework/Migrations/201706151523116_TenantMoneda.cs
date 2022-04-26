namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenantMoneda : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TicoPay.AbpTenants", "CoinsID", "TicoPay.Monedas");
            DropIndex("TicoPay.AbpTenants", new[] { "CoinsID" });
            AddColumn("TicoPay.AbpTenants", "CodigoMoneda", c => c.Int(nullable: false));
            DropColumn("TicoPay.AbpTenants", "CoinsID");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.AbpTenants", "CoinsID", c => c.Int());
            DropColumn("TicoPay.AbpTenants", "CodigoMoneda");
            CreateIndex("TicoPay.AbpTenants", "CoinsID");
            AddForeignKey("TicoPay.AbpTenants", "CoinsID", "TicoPay.Monedas", "Id");
        }
    }
}
