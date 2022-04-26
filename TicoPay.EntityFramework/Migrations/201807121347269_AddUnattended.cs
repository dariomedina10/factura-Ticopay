namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class AddUnattended : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TicoPay.UnattendedApis",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TenantId = c.Int(nullable: false),
                        VoucherKey = c.String(maxLength: 50),
                        ConsecutiveNumber = c.String(maxLength: 20),
                        ElectronicBill = c.String(),
                        QRCode = c.Binary(),
                        SendInvoice = c.Boolean(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        StatusTribunet = c.Int(nullable: false),
                        StatusVoucher = c.Int(nullable: false),
                        MessageTaxAdministration = c.String(),
                        Number = c.Int(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UnattendedApi_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_UnattendedApi_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.AbpTenants", t => t.TenantId, cascadeDelete: true)
                .Index(t => t.TenantId)
                .Index(t => t.Number);
            
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.UnattendedApis", "TenantId", "TicoPay.AbpTenants");
            DropIndex("TicoPay.UnattendedApis", new[] { "Number" });
            DropIndex("TicoPay.UnattendedApis", new[] { "TenantId" });
            DropTable("TicoPay.UnattendedApis",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UnattendedApi_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_UnattendedApi_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
