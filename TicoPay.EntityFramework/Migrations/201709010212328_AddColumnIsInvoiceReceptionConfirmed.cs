namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddColumnIsInvoiceReceptionConfirmed : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Invoices", "IsInvoiceReceptionConfirmed", c => c.Boolean(nullable: false, defaultValue: false));
        }

        public override void Down()
        {
            DropColumn("TicoPay.Invoices", "IsInvoiceReceptionConfirmed");
        }
    }
}
