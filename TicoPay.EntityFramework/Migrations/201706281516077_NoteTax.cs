namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NoteTax : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Notes", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TicoPay.Notes", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Notes", "Total");
            DropColumn("TicoPay.Notes", "TaxAmount");
        }
    }
}
