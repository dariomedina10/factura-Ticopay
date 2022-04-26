namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnSaveToAzureFlagsExceptionTribunet : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Invoices", "SavedInvoiceOrTicketPDF", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("TicoPay.Invoices", "SavedInvoiceOrTicketXML", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("TicoPay.Invoices", "ResponseTribunetExepcion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Invoices", "ResponseTribunetExepcion");
            DropColumn("TicoPay.Invoices", "SavedInvoiceOrTicketXML");
            DropColumn("TicoPay.Invoices", "SavedInvoiceOrTicketPDF");
        }
    }
}
