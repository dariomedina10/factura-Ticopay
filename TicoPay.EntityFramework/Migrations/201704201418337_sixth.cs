namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class sixth : DbMigration
    {
        public override void Up()
        {
            Sql("Delete from TicoPay.ClientGroups");

            Sql("Delete from TicoPay.ClientServices");

            Sql("Delete from TicoPay.InvoiceLines");

            Sql("Delete from TicoPay.Notes");

            Sql("Delete from TicoPay.Invoices");

            Sql("Delete from TicoPay.Clients");

            CreateTable(
                "TicoPay.ClientGroupConcepts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        GroupId = c.Guid(nullable: false),
                        ClientId = c.Guid(nullable: false),
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
                    { "DynamicFilter_ClientGroupConcept_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ClientGroupConcept_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("TicoPay.GroupConcepts", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "TicoPay.GroupConcepts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TenantId = c.Int(nullable: false),
                        Name = c.String(maxLength: 60),
                        Description = c.String(maxLength: 500),
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
                    { "DynamicFilter_GroupConcepts_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_GroupConcepts_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "TicoPay.GroupServices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        GroupId = c.Guid(nullable: false),
                        ServiceId = c.Guid(nullable: false),
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
                    { "DynamicFilter_GroupServices_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_GroupServices_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.GroupConcepts", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("TicoPay.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.ServiceId);
            
            AddColumn("TicoPay.Clients", "IdentificacionExtranjero", c => c.String(maxLength: 20));
            AddColumn("TicoPay.Clients", "BarrioId", c => c.Int(nullable: false));
            AlterColumn("TicoPay.Clients", "Address", c => c.String(maxLength: 160));
            AlterColumn("TicoPay.Monedas", "NombrePais", c => c.String(nullable: false, maxLength: 70));
            AlterColumn("TicoPay.Monedas", "NombreMoneda", c => c.String(nullable: false, maxLength: 70));
            CreateIndex("TicoPay.Clients", "BarrioId");
            AddForeignKey("TicoPay.Clients", "BarrioId", "TicoPay.Barrios", "Id", cascadeDelete: true);
            DropColumn("TicoPay.Clients", "Street");
            DropColumn("TicoPay.Clients", "City");
            DropColumn("TicoPay.Clients", "Region");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Clients", "Region", c => c.String(maxLength: 150));
            AddColumn("TicoPay.Clients", "City", c => c.String(maxLength: 150));
            AddColumn("TicoPay.Clients", "Street", c => c.String(maxLength: 150));
            DropForeignKey("TicoPay.GroupServices", "ServiceId", "TicoPay.Services");
            DropForeignKey("TicoPay.GroupServices", "GroupId", "TicoPay.GroupConcepts");
            DropForeignKey("TicoPay.ClientGroupConcepts", "GroupId", "TicoPay.GroupConcepts");
            DropForeignKey("TicoPay.ClientGroupConcepts", "ClientId", "TicoPay.Clients");
            DropForeignKey("TicoPay.Clients", "BarrioId", "TicoPay.Barrios");
            DropIndex("TicoPay.GroupServices", new[] { "ServiceId" });
            DropIndex("TicoPay.GroupServices", new[] { "GroupId" });
            DropIndex("TicoPay.ClientGroupConcepts", new[] { "ClientId" });
            DropIndex("TicoPay.ClientGroupConcepts", new[] { "GroupId" });
            DropIndex("TicoPay.Clients", new[] { "BarrioId" });
            AlterColumn("TicoPay.Monedas", "NombreMoneda", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("TicoPay.Monedas", "NombrePais", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("TicoPay.Clients", "Address", c => c.String(maxLength: 1000));
            DropColumn("TicoPay.Clients", "BarrioId");
            DropColumn("TicoPay.Clients", "IdentificacionExtranjero");
            DropTable("TicoPay.GroupServices",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_GroupServices_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_GroupServices_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.GroupConcepts",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_GroupConcepts_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_GroupConcepts_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.ClientGroupConcepts",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ClientGroupConcept_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ClientGroupConcept_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
