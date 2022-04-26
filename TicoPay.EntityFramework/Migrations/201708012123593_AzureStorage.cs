namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AzureStorage : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Invoices", "ElectronicBillPDF", c => c.String());
            DropColumn("TicoPay.Invoices", "ElectronicBillDocPDF");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Invoices", "ElectronicBillDocPDF", c => c.Binary());
            DropColumn("TicoPay.Invoices", "ElectronicBillPDF");
        }
    }
}
