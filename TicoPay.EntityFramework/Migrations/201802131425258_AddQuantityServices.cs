namespace TicoPay.Migrations
{
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    public partial class AddQuantityServices : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TicoPay.ClientGroupConcept", "ServiceId", "TicoPay.Client");
            DropForeignKey("TicoPay.ClientGroupConcept", "GroupConceptsId", "TicoPay.GroupConcepts");
            DropIndex("TicoPay.ClientGroupConcept", new[] { "ServiceId" });
            DropIndex("TicoPay.ClientGroupConcept", new[] { "GroupConceptsId" });
            CreateTable(
                "TicoPay.ClientGroupConcepts",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    GroupId = c.Guid(nullable: false),
                    ClientId = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    Quantity = c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 1),
                    DiscountPercentage = c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0),
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
                .ForeignKey("TicoPay.Client", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("TicoPay.GroupConcepts", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.ClientId);

            AddColumn("TicoPay.GroupConcepts", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 1));
            AddColumn("TicoPay.GroupConcepts", "DiscountPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0));
            AddColumn("TicoPay.Services", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 1));
            AddColumn("TicoPay.Services", "DiscountPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0));
            AddColumn("TicoPay.ClientServices", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 1));
            AddColumn("TicoPay.ClientServices", "DiscountPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0));
            AlterColumn("TicoPay.Client", "Identification", c => c.String(maxLength: 50));

            Sql(@"INSERT INTO [TicoPay].[ClientGroupConcepts]([Id],[GroupId],[ClientId],[TenantId],[CreationTime]) 
               (select NEWID(), cc.GroupConceptsId,cc.ServiceId, gc.TenantId, case when gc.CreationTime> c.CreationTime 
                    then c.CreationTime else gc.CreationTime end CreationTime from[TicoPay].[ClientGroupConcept] cc
                    inner join[TicoPay].[GroupConcepts] gc on cc.GroupConceptsId = gc.Id
                    inner join[TicoPay].[Client] c on cc.ServiceId = c.Id)");


            DropTable("TicoPay.ClientGroupConcept");
        }

        public override void Down()
        {
            CreateTable(
                "TicoPay.ClientGroupConcept",
                c => new
                {
                    ServiceId = c.Guid(nullable: false),
                    GroupConceptsId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => new { t.ServiceId, t.GroupConceptsId });

            DropForeignKey("TicoPay.ClientGroupConcepts", "GroupId", "TicoPay.GroupConcepts");
            DropForeignKey("TicoPay.ClientGroupConcepts", "ClientId", "TicoPay.Client");
            DropIndex("TicoPay.ClientGroupConcepts", new[] { "ClientId" });
            DropIndex("TicoPay.ClientGroupConcepts", new[] { "GroupId" });
            AlterColumn("TicoPay.Client", "Identification", c => c.String(nullable: false, maxLength: 50));
            DropColumn("TicoPay.ClientServices", "DiscountPercentage");
            DropColumn("TicoPay.ClientServices", "Quantity");
            DropColumn("TicoPay.Services", "DiscountPercentage");
            DropColumn("TicoPay.Services", "Quantity");
            DropColumn("TicoPay.GroupConcepts", "DiscountPercentage");
            DropColumn("TicoPay.GroupConcepts", "Quantity");

            Sql(@"INSERT INTO [TicoPay].[ClientGroupConcept]([ServiceId],[GroupConceptsId]) 
               (select ClientId ,GroupID from [TicoPay].[ClientGroupConcepts] where IsDeleted=0)");

            DropTable("TicoPay.ClientGroupConcepts",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ClientGroupConcept_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ClientGroupConcept_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            CreateIndex("TicoPay.ClientGroupConcept", "GroupConceptsId");
            CreateIndex("TicoPay.ClientGroupConcept", "ServiceId");
            AddForeignKey("TicoPay.ClientGroupConcept", "GroupConceptsId", "TicoPay.GroupConcepts", "Id", cascadeDelete: true);
            AddForeignKey("TicoPay.ClientGroupConcept", "ServiceId", "TicoPay.Client", "Id", cascadeDelete: true);
        }
    }

}
