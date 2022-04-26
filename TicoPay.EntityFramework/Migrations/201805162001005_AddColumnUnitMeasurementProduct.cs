namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnUnitMeasurementProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Products", "UnitMeasurement", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Products", "UnitMeasurement");
        }
    }
}
