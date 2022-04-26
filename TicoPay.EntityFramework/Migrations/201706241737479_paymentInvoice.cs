namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentInvoice : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TicoPay.Invoices", "PaymetnMethodId", "TicoPay.PaymentMethods");
            DropForeignKey("TicoPay.PaymentInvoices", "PaymentMethodId", "TicoPay.PaymentMethods");
            DropForeignKey("TicoPay.PaymentInvoices", "ExchangeRateId", "TicoPay.ExchangeRates");
            DropIndex("TicoPay.Invoices", new[] { "PaymetnMethodId" });
            DropIndex("TicoPay.PaymentInvoices", new[] { "ExchangeRateId" });
            DropIndex("TicoPay.PaymentInvoices", new[] { "PaymentMethodId" });
            AddColumn("TicoPay.AgreementConectivities", "AgreementNumbers", c => c.Int(nullable: false));
            AddColumn("TicoPay.PaymentInvoices", "PaymentDate", c => c.DateTime(nullable: false));
            AddColumn("TicoPay.PaymentInvoices", "PaymetnMethodType", c => c.Int(nullable: false));
            AddColumn("TicoPay.PaymentInvoices", "Transaction", c => c.String());
            AddColumn("TicoPay.PaymentInvoices", "CodigoBanco", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "CodigoAgencia", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "CodigoTransaccion", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "ConsecutivoTransaccion", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "NotaCredito", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "CodigoBancoEmisor", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "NumeroCuenta", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "NumeroCheque", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "UserId", c => c.Guid());
            AddColumn("TicoPay.PaymentInvoices", "UserName", c => c.String());
            AlterColumn("TicoPay.PaymentInvoices", "ExchangeRateId", c => c.Guid());
            CreateIndex("TicoPay.PaymentInvoices", "ExchangeRateId");
            AddForeignKey("TicoPay.PaymentInvoices", "ExchangeRateId", "TicoPay.ExchangeRates", "Id");
            DropColumn("TicoPay.AgreementConectivities", "AgreementNumber");
            DropColumn("TicoPay.Invoices", "Transaction");
            DropColumn("TicoPay.Invoices", "PaymentDate");
            DropColumn("TicoPay.Invoices", "PaymetnMethodType");
            DropColumn("TicoPay.Invoices", "PaymetnMethodId");
            DropColumn("TicoPay.PaymentInvoices", "PaymentMethodId");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.PaymentInvoices", "PaymentMethodId", c => c.Guid(nullable: false));
            AddColumn("TicoPay.Invoices", "PaymetnMethodId", c => c.Guid());
            AddColumn("TicoPay.Invoices", "PaymetnMethodType", c => c.Int(nullable: false));
            AddColumn("TicoPay.Invoices", "PaymentDate", c => c.DateTime());
            AddColumn("TicoPay.Invoices", "Transaction", c => c.String(maxLength: 50));
            AddColumn("TicoPay.AgreementConectivities", "AgreementNumber", c => c.String());
            DropForeignKey("TicoPay.PaymentInvoices", "ExchangeRateId", "TicoPay.ExchangeRates");
            DropIndex("TicoPay.PaymentInvoices", new[] { "ExchangeRateId" });
            AlterColumn("TicoPay.PaymentInvoices", "ExchangeRateId", c => c.Guid(nullable: false));
            DropColumn("TicoPay.PaymentInvoices", "UserName");
            DropColumn("TicoPay.PaymentInvoices", "UserId");
            DropColumn("TicoPay.PaymentInvoices", "NumeroCheque");
            DropColumn("TicoPay.PaymentInvoices", "NumeroCuenta");
            DropColumn("TicoPay.PaymentInvoices", "CodigoBancoEmisor");
            DropColumn("TicoPay.PaymentInvoices", "NotaCredito");
            DropColumn("TicoPay.PaymentInvoices", "ConsecutivoTransaccion");
            DropColumn("TicoPay.PaymentInvoices", "CodigoTransaccion");
            DropColumn("TicoPay.PaymentInvoices", "CodigoAgencia");
            DropColumn("TicoPay.PaymentInvoices", "CodigoBanco");
            DropColumn("TicoPay.PaymentInvoices", "Transaction");
            DropColumn("TicoPay.PaymentInvoices", "PaymetnMethodType");
            DropColumn("TicoPay.PaymentInvoices", "PaymentDate");
            DropColumn("TicoPay.AgreementConectivities", "AgreementNumbers");
            CreateIndex("TicoPay.PaymentInvoices", "PaymentMethodId");
            CreateIndex("TicoPay.PaymentInvoices", "ExchangeRateId");
            CreateIndex("TicoPay.Invoices", "PaymetnMethodId");
            AddForeignKey("TicoPay.PaymentInvoices", "ExchangeRateId", "TicoPay.ExchangeRates", "Id", cascadeDelete: true);
            AddForeignKey("TicoPay.PaymentInvoices", "PaymentMethodId", "TicoPay.PaymentMethods", "Id", cascadeDelete: true);
            AddForeignKey("TicoPay.Invoices", "PaymetnMethodId", "TicoPay.PaymentMethods", "Id");
        }
    }
}
