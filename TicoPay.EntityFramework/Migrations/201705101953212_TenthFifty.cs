namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenthFifty : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TicoPay.Taxes", "Rate", c => c.Decimal(nullable: false, precision: 4, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("TicoPay.Taxes", "Rate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
