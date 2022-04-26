namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeneralObservationsInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Invoices", "GeneralObservation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Invoices", "GeneralObservation");
        }
    }
}
