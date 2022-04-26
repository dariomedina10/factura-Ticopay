namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteTableTipoyUnidad : DbMigration
    {
        public override void Up()
        {
            Sql("Delete from TicoPay.ClientServices");

            DropForeignKey("TicoPay.Invoices", "ConditionSales_Id", "TicoPay.Tipos");
            DropForeignKey("TicoPay.Invoices", "MoneyID", "TicoPay.Monedas");
            DropForeignKey("TicoPay.Services", "UnitMeasurementID", "TicoPay.UnitMeasurements");
            DropIndex("TicoPay.Services", new[] { "UnitMeasurementID" });
            DropIndex("TicoPay.Invoices", new[] { "MoneyID" });
            DropIndex("TicoPay.Invoices", new[] { "ConditionSales_Id" });
            AddColumn("TicoPay.Services", "UnitMeasurement", c => c.Int(nullable: false));
            AddColumn("TicoPay.Invoices", "ConditionSaleType", c => c.Int(nullable: false));
            AddColumn("TicoPay.Invoices", "CodigoMoneda", c => c.Int(nullable: false));
            AddColumn("TicoPay.InvoiceLines", "CodeTypes", c => c.Int(nullable: false));
            DropColumn("TicoPay.Services", "UnitMeasurementID");
            DropColumn("TicoPay.Invoices", "TypeId");
            DropColumn("TicoPay.Invoices", "MoneyID");
            DropColumn("TicoPay.Invoices", "ConditionSales_Id");
            DropColumn("TicoPay.InvoiceLines", "CodeType");
            DropTable("TicoPay.Tipos");
            DropTable("TicoPay.UnitMeasurements");
        }
        
        public override void Down()
        {
            CreateTable(
                "TicoPay.UnitMeasurements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Symbol = c.String(maxLength: 10),
                        Description = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "TicoPay.Tipos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GrupoTipo = c.Int(nullable: false),
                        name = c.String(nullable: false, maxLength: 150),
                        codigo = c.String(nullable: false, maxLength: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("TicoPay.InvoiceLines", "CodeType", c => c.String(maxLength: 2));
            AddColumn("TicoPay.Invoices", "ConditionSales_Id", c => c.Int());
            AddColumn("TicoPay.Invoices", "MoneyID", c => c.Int(nullable: false));
            AddColumn("TicoPay.Invoices", "TypeId", c => c.Int(nullable: false));
            AddColumn("TicoPay.Services", "UnitMeasurementID", c => c.Int());
            DropColumn("TicoPay.InvoiceLines", "CodeTypes");
            DropColumn("TicoPay.Invoices", "CodigoMoneda");
            DropColumn("TicoPay.Invoices", "ConditionSaleType");
            DropColumn("TicoPay.Services", "UnitMeasurement");
            CreateIndex("TicoPay.Invoices", "ConditionSales_Id");
            CreateIndex("TicoPay.Invoices", "MoneyID");
            CreateIndex("TicoPay.Services", "UnitMeasurementID");
            AddForeignKey("TicoPay.Services", "UnitMeasurementID", "TicoPay.UnitMeasurements", "Id");
            AddForeignKey("TicoPay.Invoices", "MoneyID", "TicoPay.Monedas", "Id", cascadeDelete: true);
            AddForeignKey("TicoPay.Invoices", "ConditionSales_Id", "TicoPay.Tipos", "Id");
        }
    }
}
