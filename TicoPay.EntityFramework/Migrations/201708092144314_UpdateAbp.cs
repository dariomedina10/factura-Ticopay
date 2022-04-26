namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;

    public partial class UpdateAbp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TicoPay.AbpUserClaims",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    UserId = c.Long(nullable: false),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserClaim_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.AbpUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            AlterTableAnnotations(
                "TicoPay.AbpUserOrganizationUnits",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    UserId = c.Long(nullable: false),
                    OrganizationUnitId = c.Long(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    {
                        "DynamicFilter_UserOrganizationUnit_SoftDelete",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                });

            AddColumn("TicoPay.AbpUsers", "LockoutEndDateUtc", c => c.DateTime());
            AddColumn("TicoPay.AbpUsers", "AccessFailedCount", c => c.Int(nullable: false));
            AddColumn("TicoPay.AbpUsers", "IsLockoutEnabled", c => c.Boolean(nullable: false));
            AddColumn("TicoPay.AbpUsers", "PhoneNumber", c => c.String());
            AddColumn("TicoPay.AbpUsers", "IsPhoneNumberConfirmed", c => c.Boolean(nullable: false));
            AddColumn("TicoPay.AbpUsers", "SecurityStamp", c => c.String());
            AddColumn("TicoPay.AbpUsers", "IsTwoFactorEnabled", c => c.Boolean(nullable: false));
            AddColumn("TicoPay.AbpLanguages", "IsDisabled", c => c.Boolean(nullable: false));
            AddColumn("TicoPay.AbpUserOrganizationUnits", "IsDeleted", c => c.Boolean(nullable: false));
            AlterColumn("TicoPay.AbpUsers", "EmailConfirmationCode", c => c.String(maxLength: 328));
            AlterColumn("TicoPay.AbpOrganizationUnits", "Code", c => c.String(nullable: false, maxLength: 95));
        }

        public override void Down()
        {
            DropForeignKey("TicoPay.AbpUserClaims", "UserId", "TicoPay.AbpUsers");
            DropIndex("TicoPay.AbpUserClaims", new[] { "UserId" });
            AlterColumn("TicoPay.AbpOrganizationUnits", "Code", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("TicoPay.AbpUsers", "EmailConfirmationCode", c => c.String(maxLength: 128));
            DropColumn("TicoPay.AbpUserOrganizationUnits", "IsDeleted");
            DropColumn("TicoPay.AbpLanguages", "IsDisabled");
            DropColumn("TicoPay.AbpUsers", "IsTwoFactorEnabled");
            DropColumn("TicoPay.AbpUsers", "SecurityStamp");
            DropColumn("TicoPay.AbpUsers", "IsPhoneNumberConfirmed");
            DropColumn("TicoPay.AbpUsers", "PhoneNumber");
            DropColumn("TicoPay.AbpUsers", "IsLockoutEnabled");
            DropColumn("TicoPay.AbpUsers", "AccessFailedCount");
            DropColumn("TicoPay.AbpUsers", "LockoutEndDateUtc");
            AlterTableAnnotations(
                "TicoPay.AbpUserOrganizationUnits",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    UserId = c.Long(nullable: false),
                    OrganizationUnitId = c.Long(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    {
                        "DynamicFilter_UserOrganizationUnit_SoftDelete",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                });

            DropTable("TicoPay.AbpUserClaims",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserClaim_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
