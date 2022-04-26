namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NoteXML : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Notes", "CodigoMoneda", c => c.Int(nullable: false));
            AddColumn("TicoPay.Notes", "ServiceId", c => c.Guid());
            AddColumn("TicoPay.Notes", "MessageTaxAdministration", c => c.String());
            AddColumn("TicoPay.Notes", "MessageReceiver", c => c.String());
            AddColumn("TicoPay.Notes", "ElectronicBill", c => c.String());
            AddColumn("TicoPay.Notes", "ElectronicBillDocPDF", c => c.Binary());
            AddColumn("TicoPay.Notes", "QRCode", c => c.Binary());
            AddColumn("TicoPay.Notes", "VoucherKey", c => c.String(maxLength: 50));
            AddColumn("TicoPay.Notes", "ConsecutiveNumber", c => c.String(maxLength: 20));
            AddColumn("TicoPay.Notes", "ChangeType", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("TicoPay.Notes", "ServiceId");
            AddForeignKey("TicoPay.Notes", "ServiceId", "TicoPay.Services", "Id");
            DropColumn("TicoPay.Notes", "CurrencyCode");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Notes", "CurrencyCode", c => c.String(maxLength: 3));
            DropForeignKey("TicoPay.Notes", "ServiceId", "TicoPay.Services");
            DropIndex("TicoPay.Notes", new[] { "ServiceId" });
            DropColumn("TicoPay.Notes", "ChangeType");
            DropColumn("TicoPay.Notes", "ConsecutiveNumber");
            DropColumn("TicoPay.Notes", "VoucherKey");
            DropColumn("TicoPay.Notes", "QRCode");
            DropColumn("TicoPay.Notes", "ElectronicBillDocPDF");
            DropColumn("TicoPay.Notes", "ElectronicBill");
            DropColumn("TicoPay.Notes", "MessageReceiver");
            DropColumn("TicoPay.Notes", "MessageTaxAdministration");
            DropColumn("TicoPay.Notes", "ServiceId");
            DropColumn("TicoPay.Notes", "CodigoMoneda");
        }
    }
}
