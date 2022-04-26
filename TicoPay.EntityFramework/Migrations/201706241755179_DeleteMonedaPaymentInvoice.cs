namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteMonedaPaymentInvoice : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TicoPay.PaymentInvoices", "Moneda_Id", "TicoPay.Monedas");
            DropIndex("TicoPay.PaymentInvoices", new[] { "Moneda_Id" });
            AddColumn("TicoPay.PaymentInvoices", "CodigoMoneda", c => c.Int(nullable: false));
            DropColumn("TicoPay.PaymentInvoices", "UserId");
            DropColumn("TicoPay.PaymentInvoices", "UserName");
            DropColumn("TicoPay.PaymentInvoices", "Moneda_Id");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.PaymentInvoices", "Moneda_Id", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "UserName", c => c.String());
            AddColumn("TicoPay.PaymentInvoices", "UserId", c => c.Guid());
            DropColumn("TicoPay.PaymentInvoices", "CodigoMoneda");
            CreateIndex("TicoPay.PaymentInvoices", "Moneda_Id");
            AddForeignKey("TicoPay.PaymentInvoices", "Moneda_Id", "TicoPay.Monedas", "Id");
        }
    }
}
