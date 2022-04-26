namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class notelines : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TicoPay.NoteLines",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TenantId = c.Int(nullable: false),
                        PricePerUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Notes = c.String(maxLength: 200),
                        Title = c.String(maxLength: 160),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoteId = c.Guid(nullable: false),
                        LineType = c.Int(nullable: false),
                        ServiceId = c.Guid(),
                        ProductId = c.Guid(),
                        TaxId = c.Guid(),
                        UnitMeasurement = c.Int(nullable: false),
                        UnitMeasurementOthers = c.String(),
                        LineNumber = c.Int(nullable: false),
                        CodeTypes = c.Int(nullable: false),
                        DescriptionDiscount = c.String(maxLength: 20),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        ExonerationId = c.Guid(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_NoteLine_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_NoteLine_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Exonerations", t => t.ExonerationId)
                .ForeignKey("TicoPay.Notes", t => t.NoteId, cascadeDelete: true)
                .ForeignKey("TicoPay.Products", t => t.ProductId)
                .ForeignKey("TicoPay.Services", t => t.ServiceId)
                .ForeignKey("TicoPay.Taxes", t => t.TaxId)
                .Index(t => t.NoteId)
                .Index(t => t.ServiceId)
                .Index(t => t.ProductId)
                .Index(t => t.TaxId)
                .Index(t => t.ExonerationId);
            
            AddColumn("TicoPay.Notes", "DiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TicoPay.Notes", "TotalServGravados", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TicoPay.Notes", "TotalServExento", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TicoPay.Notes", "TotalProductExento", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TicoPay.Notes", "TotalProductGravado", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TicoPay.Notes", "TotalGravado", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TicoPay.Notes", "TotalExento", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TicoPay.Notes", "SaleTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TicoPay.Notes", "NoteReasonsOthers", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.NoteLines", "TaxId", "TicoPay.Taxes");
            DropForeignKey("TicoPay.NoteLines", "ServiceId", "TicoPay.Services");
            DropForeignKey("TicoPay.NoteLines", "ProductId", "TicoPay.Products");
            DropForeignKey("TicoPay.NoteLines", "NoteId", "TicoPay.Notes");
            DropForeignKey("TicoPay.NoteLines", "ExonerationId", "TicoPay.Exonerations");
            DropIndex("TicoPay.NoteLines", new[] { "ExonerationId" });
            DropIndex("TicoPay.NoteLines", new[] { "TaxId" });
            DropIndex("TicoPay.NoteLines", new[] { "ProductId" });
            DropIndex("TicoPay.NoteLines", new[] { "ServiceId" });
            DropIndex("TicoPay.NoteLines", new[] { "NoteId" });
            DropColumn("TicoPay.Notes", "NoteReasonsOthers");
            DropColumn("TicoPay.Notes", "SaleTotal");
            DropColumn("TicoPay.Notes", "TotalExento");
            DropColumn("TicoPay.Notes", "TotalGravado");
            DropColumn("TicoPay.Notes", "TotalProductGravado");
            DropColumn("TicoPay.Notes", "TotalProductExento");
            DropColumn("TicoPay.Notes", "TotalServExento");
            DropColumn("TicoPay.Notes", "TotalServGravados");
            DropColumn("TicoPay.Notes", "DiscountAmount");
            DropTable("TicoPay.NoteLines",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_NoteLine_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_NoteLine_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
