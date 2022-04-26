namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenthThird : DbMigration
    {
        public override void Up()
        {
            Sql("Delete from TicoPay.ClientServices");

            Sql("Delete from TicoPay.InvoiceLines");

            Sql("Delete from TicoPay.Notes");

            Sql("Delete from TicoPay.Services");

            Sql("Delete from TicoPay.Invoices");

            CreateTable(
                "TicoPay.UnitMeasurements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Symbol = c.String(maxLength: 10),
                        Description = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("TicoPay.Services", "UnitMeasurementID", c => c.Int());
            AddColumn("TicoPay.Services", "UnitMeasurementOthers", c => c.String());
            AlterColumn("TicoPay.Services", "Name", c => c.String(maxLength: 160));
            CreateIndex("TicoPay.Services", "UnitMeasurementID");
            AddForeignKey("TicoPay.Services", "UnitMeasurementID", "TicoPay.UnitMeasurements", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.Services", "UnitMeasurementID", "TicoPay.UnitMeasurements");
            DropIndex("TicoPay.Services", new[] { "UnitMeasurementID" });
            AlterColumn("TicoPay.Services", "Name", c => c.String(maxLength: 128));
            DropColumn("TicoPay.Services", "UnitMeasurementOthers");
            DropColumn("TicoPay.Services", "UnitMeasurementID");
            DropTable("TicoPay.UnitMeasurements");
        }
    }
}
