namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenthFirst : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Invoices", "QRCode", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Invoices", "QRCode");
        }
    }
}
