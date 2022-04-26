namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seventh : DbMigration
    {
        public override void Up()
        {
            Sql("Delete from TicoPay.InvoiceLines");

            Sql("Delete from TicoPay.Notes");

            Sql("Delete from TicoPay.Invoices");

            AddColumn("TicoPay.Invoices", "PaymetnMethodId", c => c.Guid(nullable: false));
            AddColumn("TicoPay.Invoices", "TypeConditionSale", c => c.String());
            AddColumn("TicoPay.Invoices", "OtherConditionSale", c => c.String(maxLength: 100));
            AddColumn("TicoPay.Invoices", "CreditTerm", c => c.Int(nullable: false));
            AddColumn("TicoPay.Invoices", "MoneyID", c => c.Int(nullable: false));
            AddColumn("TicoPay.Invoices", "StatusVoucher", c => c.Int(nullable: false));
            AddColumn("TicoPay.Invoices", "MessageTaxAdministration", c => c.String());
            AddColumn("TicoPay.Invoices", "MessageReceiver", c => c.String());
            AddColumn("TicoPay.Invoices", "ElectronicBill", c => c.String());
            AddColumn("TicoPay.Invoices", "ElectronicBillPDF", c => c.String());
            AddColumn("TicoPay.Invoices", "QRCode", c => c.String(maxLength: 50));
            AddColumn("TicoPay.Invoices", "VoucherKey", c => c.String(maxLength: 50));
            AddColumn("TicoPay.Invoices", "ConsecutiveNumber", c => c.String(maxLength: 20));
            AddColumn("TicoPay.InvoiceLines", "LineNumber", c => c.Int(nullable: false));
            AddColumn("TicoPay.InvoiceLines", "CodeType", c => c.String(maxLength: 2));
            AddColumn("TicoPay.InvoiceLines", "DescriptionDiscount", c => c.String(maxLength: 20));
            AddColumn("TicoPay.InvoiceLines", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("TicoPay.InvoiceLines", "LineTotal", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("TicoPay.PaymentInvoices", "Moneda_Id", c => c.Int());
            AlterColumn("TicoPay.InvoiceLines", "PricePerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.InvoiceLines", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.InvoiceLines", "DiscountPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.InvoiceLines", "Quantity", c => c.Decimal(nullable: false, precision: 16, scale: 3));
            AlterColumn("TicoPay.PaymentMethods", "Name", c => c.String(maxLength: 50));
            AlterColumn("TicoPay.PaymentMethods", "Code", c => c.String(maxLength: 2));
            CreateIndex("TicoPay.Invoices", "PaymetnMethodId");
            CreateIndex("TicoPay.Invoices", "MoneyID");
            CreateIndex("TicoPay.PaymentInvoices", "Moneda_Id");
            AddForeignKey("TicoPay.Invoices", "PaymetnMethodId", "TicoPay.PaymentMethods", "Id", cascadeDelete: false);
            AddForeignKey("TicoPay.Invoices", "MoneyID", "TicoPay.Monedas", "Id", cascadeDelete: true);
            AddForeignKey("TicoPay.PaymentInvoices", "Moneda_Id", "TicoPay.Monedas", "Id");
            DropColumn("TicoPay.Invoices", "CurrencyCode");
            DropColumn("TicoPay.InvoiceLines", "CurrencyCode");
            DropColumn("TicoPay.PaymentInvoices", "CurrencyCode");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.PaymentInvoices", "CurrencyCode", c => c.String());
            AddColumn("TicoPay.InvoiceLines", "CurrencyCode", c => c.String(maxLength: 3));
            AddColumn("TicoPay.Invoices", "CurrencyCode", c => c.String(maxLength: 3));
            DropForeignKey("TicoPay.PaymentInvoices", "Moneda_Id", "TicoPay.Monedas");
            DropForeignKey("TicoPay.Invoices", "MoneyID", "TicoPay.Monedas");
            DropForeignKey("TicoPay.Invoices", "PaymetnMethodId", "TicoPay.PaymentMethods");
            DropIndex("TicoPay.PaymentInvoices", new[] { "Moneda_Id" });
            DropIndex("TicoPay.Invoices", new[] { "MoneyID" });
            DropIndex("TicoPay.Invoices", new[] { "PaymetnMethodId" });
            AlterColumn("TicoPay.PaymentMethods", "Code", c => c.String());
            AlterColumn("TicoPay.PaymentMethods", "Name", c => c.String());
            AlterColumn("TicoPay.InvoiceLines", "Quantity", c => c.Int(nullable: false));
            AlterColumn("TicoPay.InvoiceLines", "DiscountPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.InvoiceLines", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.InvoiceLines", "PricePerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("TicoPay.PaymentInvoices", "Moneda_Id");
            DropColumn("TicoPay.InvoiceLines", "LineTotal");
            DropColumn("TicoPay.InvoiceLines", "SubTotal");
            DropColumn("TicoPay.InvoiceLines", "DescriptionDiscount");
            DropColumn("TicoPay.InvoiceLines", "CodeType");
            DropColumn("TicoPay.InvoiceLines", "LineNumber");
            DropColumn("TicoPay.Invoices", "ConsecutiveNumber");
            DropColumn("TicoPay.Invoices", "VoucherKey");
            DropColumn("TicoPay.Invoices", "QRCode");
            DropColumn("TicoPay.Invoices", "ElectronicBillPDF");
            DropColumn("TicoPay.Invoices", "ElectronicBill");
            DropColumn("TicoPay.Invoices", "MessageReceiver");
            DropColumn("TicoPay.Invoices", "MessageTaxAdministration");
            DropColumn("TicoPay.Invoices", "StatusVoucher");
            DropColumn("TicoPay.Invoices", "MoneyID");
            DropColumn("TicoPay.Invoices", "CreditTerm");
            DropColumn("TicoPay.Invoices", "OtherConditionSale");
            DropColumn("TicoPay.Invoices", "TypeConditionSale");
            DropColumn("TicoPay.Invoices", "PaymetnMethodId");
        }
    }
}
