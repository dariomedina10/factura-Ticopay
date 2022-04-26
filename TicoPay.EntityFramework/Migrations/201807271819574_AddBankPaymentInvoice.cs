namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class AddBankPaymentInvoice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TicoPay.Banks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 60),
                        ShortName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
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
                    { "DynamicFilter_Bank_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Bank_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            AddColumn("TicoPay.PaymentInvoices", "BankId", c => c.Guid());
            AddColumn("TicoPay.PaymentInvoices", "UserCard", c => c.String(maxLength: 60));
            CreateIndex("TicoPay.PaymentInvoices", "BankId");
            AddForeignKey("TicoPay.PaymentInvoices", "BankId", "TicoPay.Banks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.PaymentInvoices", "BankId", "TicoPay.Banks");
            DropIndex("TicoPay.PaymentInvoices", new[] { "BankId" });
            DropColumn("TicoPay.PaymentInvoices", "UserCard");
            DropColumn("TicoPay.PaymentInvoices", "BankId");
            DropTable("TicoPay.Banks",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Bank_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Bank_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
