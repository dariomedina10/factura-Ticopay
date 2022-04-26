namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnDayCreditInvoiceExpirationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Client", "CreditDays", c => c.Int());
            AddColumn("TicoPay.Invoices", "ExpirationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Invoices", "ExpirationDate");
            DropColumn("TicoPay.Client", "CreditDays");
        }
    }
}
