namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDigitDecimalNoteNoteLineInvoice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TicoPay.Notes", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Notes", "DiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Notes", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Notes", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Notes", "TotalServGravados", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Notes", "TotalServExento", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Notes", "TotalProductExento", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Notes", "TotalProductGravado", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Notes", "TotalGravado", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Notes", "TotalExento", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Notes", "SaleTotal", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.NoteLines", "PricePerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.NoteLines", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.NoteLines", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.NoteLines", "DiscountPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.NoteLines", "Quantity", c => c.Decimal(nullable: false, precision: 16, scale: 3));
            AlterColumn("TicoPay.NoteLines", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.NoteLines", "LineTotal", c => c.Decimal(nullable: false, precision: 18, scale: 5));
        }
        
        public override void Down()
        {
            AlterColumn("TicoPay.NoteLines", "LineTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.NoteLines", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.NoteLines", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.NoteLines", "DiscountPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.NoteLines", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.NoteLines", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.NoteLines", "PricePerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Notes", "SaleTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Notes", "TotalExento", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Notes", "TotalGravado", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Notes", "TotalProductGravado", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Notes", "TotalProductExento", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Notes", "TotalServExento", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Notes", "TotalServGravados", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Notes", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Notes", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Notes", "DiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Notes", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
