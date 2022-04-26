namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenthSecond : DbMigration
    {
        public override void Up()
        {
            Sql("Delete from TicoPay.InvoiceLines");

            Sql("Delete from TicoPay.Notes");

            Sql("Delete from TicoPay.Invoices");

            AddColumn("TicoPay.Invoices", "TypeId", c => c.Int(nullable: false));
            AddColumn("TicoPay.Invoices", "ConditionSales_Id", c => c.Int());
            CreateIndex("TicoPay.Invoices", "ConditionSales_Id");
            AddForeignKey("TicoPay.Invoices", "ConditionSales_Id", "TicoPay.Tipos", "Id");
            DropColumn("TicoPay.Invoices", "TypeConditionSale");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Invoices", "TypeConditionSale", c => c.String());
            DropForeignKey("TicoPay.Invoices", "ConditionSales_Id", "TicoPay.Tipos");
            DropIndex("TicoPay.Invoices", new[] { "ConditionSales_Id" });
            DropColumn("TicoPay.Invoices", "ConditionSales_Id");
            DropColumn("TicoPay.Invoices", "TypeId");
        }
    }
}
