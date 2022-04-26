namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnUnitMeasurementProductEstatusProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Products", "Estado", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Products", "Estado");
        }
    }
}
