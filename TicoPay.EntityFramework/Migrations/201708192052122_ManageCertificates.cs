namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManageCertificates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TicoPay.Certificates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantID = c.Int(nullable: false),
                        Certified = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.AbpTenants", t => t.TenantID, cascadeDelete: true)
                .Index(t => t.TenantID);
            
            AddColumn("TicoPay.AbpTenants", "ValidateHacienda", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.Certificates", "TenantID", "TicoPay.AbpTenants");
            DropIndex("TicoPay.Certificates", new[] { "TenantID" });
            DropColumn("TicoPay.AbpTenants", "ValidateHacienda");
            DropTable("TicoPay.Certificates");
        }
    }
}
