namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterColumnTitleInvoiceLines : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TicoPay.InvoiceLines", "Title", c => c.String(maxLength: 160));
        }
        
        public override void Down()
        {
            AlterColumn("TicoPay.InvoiceLines", "Title", c => c.String(maxLength: 50));
        }
    }
}
