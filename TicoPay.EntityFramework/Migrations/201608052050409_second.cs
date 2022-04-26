namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Invoices", "Transaction", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Invoices", "Transaction");
        }
    }
}
