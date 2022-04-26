namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InstallCertificate : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Certificates", "Installed", c => c.Boolean(nullable: false));
            AlterColumn("TicoPay.Certificates", "Certified", c => c.String(nullable: false));
            AlterColumn("TicoPay.Certificates", "Password", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("TicoPay.Certificates", "Password", c => c.String());
            AlterColumn("TicoPay.Certificates", "Certified", c => c.String());
            DropColumn("TicoPay.Certificates", "Installed");
        }
    }
}
