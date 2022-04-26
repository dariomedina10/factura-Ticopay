namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NoteAzureStorage : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Notes", "ElectronicBillPDF", c => c.String());
            DropColumn("TicoPay.Notes", "ElectronicBillDocPDF");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Notes", "ElectronicBillDocPDF", c => c.Binary());
            DropColumn("TicoPay.Notes", "ElectronicBillPDF");
        }
    }
}
