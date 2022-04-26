namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class addContingencydrawerRegisters : DbMigration
    {
        public override void Up()
        {
            AlterTableAnnotations(
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
                        DrawerId = c.Guid(),
                        MessageTaxAdministration = c.String(),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Voucher_MustHaveTenant",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                    { 
                        "DynamicFilter_Voucher_SoftDelete",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                });
            
            AddColumn("TicoPay.Invoices", "IsContingency", c => c.Boolean(nullable: false));
            AddColumn("TicoPay.Invoices", "ConsecutiveNumberContingency", c => c.String(maxLength: 50));
            AddColumn("TicoPay.Invoices", "ReasonContingency", c => c.String(maxLength: 180));
            AddColumn("TicoPay.Invoices", "DateContingency", c => c.DateTime());
            AddColumn("TicoPay.Notes", "IsContingency", c => c.Boolean(nullable: false));
            AddColumn("TicoPay.Notes", "ConsecutiveNumberContingency", c => c.String(maxLength: 50));
            AddColumn("TicoPay.Notes", "ReasonContingency", c => c.String(maxLength: 180));
            AddColumn("TicoPay.Notes", "DateContingency", c => c.DateTime());
            AddColumn("TicoPay.Registers", "DrawerId", c => c.Guid());
            AddColumn("TicoPay.Vouchers", "DrawerId", c => c.Guid());
            AddColumn("TicoPay.Vouchers", "DeleterUserId", c => c.Long());
            AddColumn("TicoPay.Vouchers", "DeletionTime", c => c.DateTime());
            AddColumn("TicoPay.Vouchers", "IsDeleted", c => c.Boolean(nullable: false));
            CreateIndex("TicoPay.Registers", "DrawerId");
            CreateIndex("TicoPay.Vouchers", "DrawerId");
            AddForeignKey("TicoPay.Registers", "DrawerId", "TicoPay.Drawers", "Id");
            AddForeignKey("TicoPay.Vouchers", "DrawerId", "TicoPay.Drawers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.Vouchers", "DrawerId", "TicoPay.Drawers");
            DropForeignKey("TicoPay.Registers", "DrawerId", "TicoPay.Drawers");
            DropIndex("TicoPay.Vouchers", new[] { "DrawerId" });
            DropIndex("TicoPay.Registers", new[] { "DrawerId" });
            DropColumn("TicoPay.Vouchers", "IsDeleted");
            DropColumn("TicoPay.Vouchers", "DeletionTime");
            DropColumn("TicoPay.Vouchers", "DeleterUserId");
            DropColumn("TicoPay.Vouchers", "DrawerId");
            DropColumn("TicoPay.Registers", "DrawerId");
            DropColumn("TicoPay.Notes", "DateContingency");
            DropColumn("TicoPay.Notes", "ReasonContingency");
            DropColumn("TicoPay.Notes", "ConsecutiveNumberContingency");
            DropColumn("TicoPay.Notes", "IsContingency");
            DropColumn("TicoPay.Invoices", "DateContingency");
            DropColumn("TicoPay.Invoices", "ReasonContingency");
            DropColumn("TicoPay.Invoices", "ConsecutiveNumberContingency");
            DropColumn("TicoPay.Invoices", "IsContingency");
            AlterTableAnnotations(
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
                        DrawerId = c.Guid(),
                        MessageTaxAdministration = c.String(),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Voucher_MustHaveTenant",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                    { 
                        "DynamicFilter_Voucher_SoftDelete",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                });
            
        }
    }
}
