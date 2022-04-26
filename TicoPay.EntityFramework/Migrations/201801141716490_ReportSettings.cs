namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class ReportSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TicoPay.ReportSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.Int(nullable: false),
                        ReportName = c.String(),
                        ReportTitle = c.String(),
                        ShowPrintDate = c.Boolean(nullable: false),
                        ShowPageNumber = c.Boolean(nullable: false),
                        UseWatermark = c.Boolean(nullable: false),
                        WatermarkImage = c.Binary(),
                        WatermarkPosition = c.Int(nullable: false),
                        IsCustomSize = c.Boolean(nullable: false),
                        PageSizeName = c.String(),
                        LowerLeftX = c.Single(nullable: false),
                        LowerLeftY = c.Single(nullable: false),
                        UpperRightX = c.Single(nullable: false),
                        UpperRightY = c.Single(nullable: false),
                        MarginLeft = c.Single(nullable: false),
                        MarginRight = c.Single(nullable: false),
                        MarginTop = c.Single(nullable: false),
                        MarginBottom = c.Single(nullable: false),
                        TitleFontName = c.String(),
                        SubTitleFontName = c.String(),
                        TableTitleFontName = c.String(),
                        TableFontName = c.String(),
                        BodyFontName = c.String(),
                        InfoMessageFontName = c.String(),
                        TitleFontSize = c.Int(nullable: false),
                        SubTitleFontSize = c.Int(nullable: false),
                        TableTitleFontSize = c.Int(nullable: false),
                        TableFontSize = c.Int(nullable: false),
                        BodyFontSize = c.Int(nullable: false),
                        InfoMessageFontSize = c.Int(nullable: false),
                        TitleFontArgbColor = c.Int(nullable: false),
                        SubTitleFontArgbColor = c.Int(nullable: false),
                        TableTitleFontArgbColor = c.Int(nullable: false),
                        TableFontArgbColor = c.Int(nullable: false),
                        BodyFontArgbColor = c.Int(nullable: false),
                        InfoMessageFontArgbColor = c.Int(nullable: false),
                        IsBoldTitleFont = c.Boolean(nullable: false),
                        IsBoldSubTitleFont = c.Boolean(nullable: false),
                        IsBoldTableTitleFont = c.Boolean(nullable: false),
                        IsBoldTableFont = c.Boolean(nullable: false),
                        IsBoldBodyFont = c.Boolean(nullable: false),
                        IsBoldInfoMessageFont = c.Boolean(nullable: false),
                        IsItalicTitleFont = c.Boolean(nullable: false),
                        IsItalicSubTitleFont = c.Boolean(nullable: false),
                        IsItalicTableTitleFont = c.Boolean(nullable: false),
                        IsItalicTableFont = c.Boolean(nullable: false),
                        IsItalicBodyFont = c.Boolean(nullable: false),
                        IsItalicInfoMessageFont = c.Boolean(nullable: false),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ReportSettings_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("TicoPay.ReportSettings",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ReportSettings_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
