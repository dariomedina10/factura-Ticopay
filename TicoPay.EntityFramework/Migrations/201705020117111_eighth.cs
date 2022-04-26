namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class eighth : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TicoPay.Invoices", "PaymetnMethodId", "TicoPay.PaymentMethods");
            DropIndex("TicoPay.Invoices", new[] { "PaymetnMethodId" });
            CreateTable(
                "TicoPay.Exonerations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DocumentType = c.String(maxLength: 2),
                        DocumentNumber = c.String(maxLength: 17),
                        InstitutionName = c.String(maxLength: 100),
                        ExonerationDate = c.DateTime(nullable: false),
                        TaxAmountExonerated = c.Decimal(nullable: false, precision: 18, scale: 5),
                        PercentagePurchaseExonerated = c.Int(nullable: false),
                        UserId = c.Guid(),
                        UserName = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Exoneration_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Exoneration_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            AddColumn("TicoPay.InvoiceLines", "ExonerationId", c => c.Guid());
            AlterColumn("TicoPay.Invoices", "PaymetnMethodId", c => c.Guid());
            CreateIndex("TicoPay.Invoices", "PaymetnMethodId");
            CreateIndex("TicoPay.InvoiceLines", "ExonerationId");
            AddForeignKey("TicoPay.InvoiceLines", "ExonerationId", "TicoPay.Exonerations", "Id");
            AddForeignKey("TicoPay.Invoices", "PaymetnMethodId", "TicoPay.PaymentMethods", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.Invoices", "PaymetnMethodId", "TicoPay.PaymentMethods");
            DropForeignKey("TicoPay.InvoiceLines", "ExonerationId", "TicoPay.Exonerations");
            DropIndex("TicoPay.InvoiceLines", new[] { "ExonerationId" });
            DropIndex("TicoPay.Invoices", new[] { "PaymetnMethodId" });
            AlterColumn("TicoPay.Invoices", "PaymetnMethodId", c => c.Guid(nullable: false));
            DropColumn("TicoPay.InvoiceLines", "ExonerationId");
            DropTable("TicoPay.Exonerations",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Exoneration_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Exoneration_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            CreateIndex("TicoPay.Invoices", "PaymetnMethodId");
            AddForeignKey("TicoPay.Invoices", "PaymetnMethodId", "TicoPay.PaymentMethods", "Id", cascadeDelete: true);
        }
    }
}
