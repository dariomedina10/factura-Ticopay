namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenantType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TicoPay.AbpTenants", "IdentificationTypeID", "TicoPay.Tipos");
            DropIndex("TicoPay.AbpTenants", new[] { "IdentificationTypeID" });
            AddColumn("TicoPay.AbpTenants", "IdentificationType", c => c.Int(nullable: false));
            DropColumn("TicoPay.AbpTenants", "IdentificationTypeID");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.AbpTenants", "IdentificationTypeID", c => c.Int());
            DropColumn("TicoPay.AbpTenants", "IdentificationType");
            CreateIndex("TicoPay.AbpTenants", "IdentificationTypeID");
            AddForeignKey("TicoPay.AbpTenants", "IdentificationTypeID", "TicoPay.Tipos", "Id");
        }
    }
}
