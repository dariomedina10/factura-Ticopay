namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nineth : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Registers", "RegisterCode", c => c.String(maxLength: 5));
            AddColumn("TicoPay.Registers", "FirstInvoiceNumber", c => c.Long(nullable: false));
            AddColumn("TicoPay.Registers", "LastInvoiceNumber", c => c.Long(nullable: false));
            DropColumn("TicoPay.Registers", "InvoiceNumber");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Registers", "InvoiceNumber", c => c.Int(nullable: false));
            DropColumn("TicoPay.Registers", "LastInvoiceNumber");
            DropColumn("TicoPay.Registers", "FirstInvoiceNumber");
            DropColumn("TicoPay.Registers", "RegisterCode");
        }
    }
}
