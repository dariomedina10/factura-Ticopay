namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterClientColumnNameAlterInvoiceLineDescriptionDiscountColumn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TicoPay.Client", "LastName", c => c.String(maxLength: 200));
            AlterColumn("TicoPay.Client", "NameComercial", c => c.String(maxLength: 200));
            AlterColumn("TicoPay.Client", "ContactName", c => c.String(maxLength: 200));
            AlterColumn("TicoPay.InvoiceLines", "DescriptionDiscount", c => c.String(maxLength: 80));
        }
        
        public override void Down()
        {
            AlterColumn("TicoPay.InvoiceLines", "DescriptionDiscount", c => c.String(maxLength: 20));
            AlterColumn("TicoPay.Client", "ContactName", c => c.String(maxLength: 80));
            AlterColumn("TicoPay.Client", "NameComercial", c => c.String(maxLength: 80));
            AlterColumn("TicoPay.Client", "LastName", c => c.String(maxLength: 80));
        }
    }
}
