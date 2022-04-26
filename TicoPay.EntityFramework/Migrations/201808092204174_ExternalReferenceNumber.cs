namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExternalReferenceNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Invoices", "ExternalReferenceNumber", c => c.String());
            AddColumn("TicoPay.Notes", "ExternalReferenceNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Notes", "ExternalReferenceNumber");
            DropColumn("TicoPay.Invoices", "ExternalReferenceNumber");
        }
    }
}
