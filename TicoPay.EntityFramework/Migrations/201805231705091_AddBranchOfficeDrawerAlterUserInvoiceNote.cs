namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class AddBranchOfficeDrawerAlterUserInvoiceNote : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TicoPay.Payments", "ClientId", "TicoPay.Client");
            DropIndex("TicoPay.Payments", new[] { "ClientId" });
            CreateTable(
                "TicoPay.Drawers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(),
                        Description = c.String(),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        BranchOfficeId = c.Guid(nullable: false),
                        IsOpen = c.Boolean(nullable: false,defaultValue:false),
                        UserIdOpen = c.Long(),
                        LastUserIdOpen = c.Long(),
                        OpenUserDate = c.DateTime(),
                        CloseUserDate = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Drawer_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Drawer_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.BranchOffices", t => t.BranchOfficeId, cascadeDelete: true)
                .Index(t => t.BranchOfficeId);
            
            CreateTable(
                "TicoPay.BranchOffices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Code = c.String(),
                        Location = c.String(),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_BranchOffice_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_BranchOffice_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "TicoPay.DrawerUsers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DrawerId = c.Guid(),
                        IsActive = c.Boolean(nullable: false),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                        User_Id = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_DrawerUser_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_DrawerUser_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Drawers", t => t.DrawerId)
                .ForeignKey("TicoPay.AbpUsers", t => t.User_Id)
                .Index(t => t.DrawerId)
                .Index(t => t.User_Id);
            
            AddColumn("TicoPay.Invoices", "TypeDocument", c => c.Int(nullable: false));
            AddColumn("TicoPay.Invoices", "ClientName", c => c.String());
            AddColumn("TicoPay.Invoices", "ClientIdentificationType", c => c.Int(nullable: false));
            AddColumn("TicoPay.Invoices", "ClientIdentification", c => c.String(maxLength: 20));
            AddColumn("TicoPay.Invoices", "ClientAddress", c => c.String());
            AddColumn("TicoPay.Invoices", "ClientPhoneNumber", c => c.String());
            AddColumn("TicoPay.Invoices", "ClientMobilNumber", c => c.String());
            AddColumn("TicoPay.Invoices", "ClientEmail", c => c.String());
            AddColumn("TicoPay.Invoices", "DrawerId", c => c.Guid());
            AddColumn("TicoPay.Products", "UnitMeasurement", c => c.Int(nullable: false));
            AddColumn("TicoPay.Notes", "DrawerId", c => c.Guid());
            AddColumn("TicoPay.Registers", "FirstTicketNumber", c => c.Long(nullable: false));
            AddColumn("TicoPay.Registers", "LastTicketNumber", c => c.Long(nullable: false));
            AlterColumn("TicoPay.Payments", "ClientId", c => c.Guid());
            CreateIndex("TicoPay.Invoices", "DrawerId");
            CreateIndex("TicoPay.Payments", "ClientId");
            CreateIndex("TicoPay.Notes", "DrawerId");
            AddForeignKey("TicoPay.Invoices", "DrawerId", "TicoPay.Drawers", "Id");
            AddForeignKey("TicoPay.Notes", "DrawerId", "TicoPay.Drawers", "Id");
            AddForeignKey("TicoPay.Payments", "ClientId", "TicoPay.Client", "Id");

            //Sql(@"update TicoPay.Invoices set ClientName =   c.[Name]+ case when c.LastName='N/D' then '' else ' '+c.LastName end, 
            //        ClientIdentificationType= c.IdentificationType,
            //        ClientIdentification= case when c.IdentificationType=4 then c.IdentificacionExtranjero else c.Identification end,
            //        ClientPhoneNumber=c.PhoneNumber,
            //        ClientMobilNumber= c.MobilNumber,
            //        ClientEmail= c.Email,
            //        TypeDocument=1
            //        from TicoPay.Invoices i inner join
            //        TicoPay.Client c on i.ClientId=c.Id");
        }
        
        public override void Down()
        {
            //DropForeignKey("TicoPay.Payments", "ClientId", "TicoPay.Client");
            DropForeignKey("TicoPay.DrawerUsers", "User_Id", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.Notes", "DrawerId", "TicoPay.Drawers");
            DropForeignKey("TicoPay.Invoices", "DrawerId", "TicoPay.Drawers");
            DropForeignKey("TicoPay.DrawerUsers", "DrawerId", "TicoPay.Drawers");
            DropForeignKey("TicoPay.Drawers", "BranchOfficeId", "TicoPay.BranchOffices");
            DropIndex("TicoPay.Notes", new[] { "DrawerId" });
            //DropIndex("TicoPay.Payments", new[] { "ClientId" });
            DropIndex("TicoPay.DrawerUsers", new[] { "User_Id" });
            DropIndex("TicoPay.DrawerUsers", new[] { "DrawerId" });
            DropIndex("TicoPay.Drawers", new[] { "BranchOfficeId" });
            DropIndex("TicoPay.Invoices", new[] { "DrawerId" });
            //AlterColumn("TicoPay.Payments", "ClientId", c => c.Guid(nullable: false));
            DropColumn("TicoPay.Registers", "LastTicketNumber");
            DropColumn("TicoPay.Registers", "FirstTicketNumber");
            DropColumn("TicoPay.Notes", "DrawerId");
            DropColumn("TicoPay.Products", "UnitMeasurement");
            DropColumn("TicoPay.Invoices", "DrawerId");
            DropColumn("TicoPay.Invoices", "ClientEmail");
            DropColumn("TicoPay.Invoices", "ClientMobilNumber");
            DropColumn("TicoPay.Invoices", "ClientPhoneNumber");
            DropColumn("TicoPay.Invoices", "ClientAddress");
            DropColumn("TicoPay.Invoices", "ClientIdentification");
            DropColumn("TicoPay.Invoices", "ClientIdentificationType");
            DropColumn("TicoPay.Invoices", "ClientName");
            DropColumn("TicoPay.Invoices", "TypeDocument");
            DropTable("TicoPay.DrawerUsers",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_DrawerUser_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_DrawerUser_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.BranchOffices",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_BranchOffice_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_BranchOffice_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Drawers",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Drawer_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Drawer_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            //CreateIndex("TicoPay.Payments", "ClientId");
            //AddForeignKey("TicoPay.Payments", "ClientId", "TicoPay.Client", "Id", cascadeDelete: true);
        }
    }
}
