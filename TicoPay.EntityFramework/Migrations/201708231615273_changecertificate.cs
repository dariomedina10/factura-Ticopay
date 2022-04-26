namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecertificate : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Certificates", "CertifiedRoute", c => c.Binary(nullable: false));
            DropColumn("TicoPay.Certificates", "Certified");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Certificates", "Certified", c => c.String(nullable: false));
            DropColumn("TicoPay.Certificates", "CertifiedRoute");
        }
    }
}
