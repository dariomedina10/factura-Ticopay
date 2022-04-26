namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenthFourth : DbMigration
    {
        public override void Up()
        {
            Sql("Delete from TicoPay.ClientServices");

            Sql("Delete from TicoPay.InvoiceLines");

            Sql("Delete from TicoPay.Notes");

            Sql("Delete from TicoPay.Services");

            Sql("Delete from TicoPay.Taxes");

            Sql("Delete from TicoPay.Invoices");

            Sql("UPDATE TicoPay.Registers SET FirstInvoiceNumber = 1, LastInvoiceNumber = 0");

            AddColumn("TicoPay.Taxes", "TaxTypeID", c => c.Int(nullable: false));
            AddColumn("TicoPay.Taxes", "TaxesTypes_Id", c => c.Int());
            AlterColumn("TicoPay.Taxes", "Name", c => c.String(nullable: false));
            CreateIndex("TicoPay.Taxes", "TaxesTypes_Id");
            AddForeignKey("TicoPay.Taxes", "TaxesTypes_Id", "TicoPay.Tipos", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.Taxes", "TaxesTypes_Id", "TicoPay.Tipos");
            DropIndex("TicoPay.Taxes", new[] { "TaxesTypes_Id" });
            AlterColumn("TicoPay.Taxes", "Name", c => c.String());
            DropColumn("TicoPay.Taxes", "TaxesTypes_Id");
            DropColumn("TicoPay.Taxes", "TaxTypeID");
        }
    }
}
