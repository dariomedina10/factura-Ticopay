namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addvoucher : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TicoPay.Vouchers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TenantId = c.Int(nullable: false),
                        IdentificationSender = c.String(nullable: false, maxLength: 12),
                        NameSender = c.String(nullable: false, maxLength: 80),
                        Email = c.String(),
                        NameReceiver = c.String(),
                        IdentificationReceiver = c.String(),
                        ConsecutiveNumberInvoice = c.String(maxLength: 20),
                        DateInvoice = c.DateTime(nullable: false),
                        Coin = c.Int(nullable: false),
                        Totalinvoice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VoucherKey = c.String(maxLength: 50),
                        VoucherKeyRef = c.String(),
                        ConsecutiveNumber = c.String(maxLength: 20),
                        Message = c.Int(nullable: false),
                        DetailsMessage = c.String(maxLength: 80),
                        ElectronicBill = c.String(),
                        SendVoucher = c.Boolean(nullable: false),
                        StatusTribunet = c.Int(nullable: false),
                        StatusVoucher = c.Int(nullable: false),
                        TipoFirma = c.Int(),
                        StatusFirmaDigital = c.Int(),
                        MessageTaxAdministration = c.String(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.AbpTenants", t => t.TenantId, cascadeDelete: true)
                .Index(t => t.TenantId);
            
            AddColumn("TicoPay.Registers", "FirstVoucherNumber", c => c.Long(nullable: false));
            AddColumn("TicoPay.Registers", "LastVoucherNumber", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.Vouchers", "TenantId", "TicoPay.AbpTenants");
            DropIndex("TicoPay.Vouchers", new[] { "TenantId" });
            DropColumn("TicoPay.Registers", "LastVoucherNumber");
            DropColumn("TicoPay.Registers", "FirstVoucherNumber");
            DropTable("TicoPay.Vouchers");
        }
    }
}
