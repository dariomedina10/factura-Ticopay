namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StatusTaxAdministration : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Invoices", "SendInvoice", c => c.Boolean(nullable: false));
            AddColumn("TicoPay.Invoices", "StatusTribunet", c => c.Int(nullable: false));
            AddColumn("TicoPay.Notes", "SendInvoice", c => c.Boolean(nullable: false));
            AddColumn("TicoPay.Notes", "StatusTribunet", c => c.Int(nullable: false));
            AddColumn("TicoPay.Notes", "StatusVoucher", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Notes", "StatusVoucher");
            DropColumn("TicoPay.Notes", "StatusTribunet");
            DropColumn("TicoPay.Notes", "SendInvoice");
            DropColumn("TicoPay.Invoices", "StatusTribunet");
            DropColumn("TicoPay.Invoices", "SendInvoice");
        }
    }
}
