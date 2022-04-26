namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaxTypeFE : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TicoPay.Taxes", "TaxesTypes_Id", "TicoPay.Tipos");
            DropIndex("TicoPay.Taxes", new[] { "TaxesTypes_Id" });
            AddColumn("TicoPay.Taxes", "TaxTypes", c => c.Int(nullable: false));
            DropColumn("TicoPay.Taxes", "TaxTypeID");
            DropColumn("TicoPay.Taxes", "TaxesTypes_Id");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Taxes", "TaxesTypes_Id", c => c.Int());
            AddColumn("TicoPay.Taxes", "TaxTypeID", c => c.Int(nullable: false));
            DropColumn("TicoPay.Taxes", "TaxTypes");
            CreateIndex("TicoPay.Taxes", "TaxesTypes_Id");
            AddForeignKey("TicoPay.Taxes", "TaxesTypes_Id", "TicoPay.Tipos", "Id");
        }
    }
}
