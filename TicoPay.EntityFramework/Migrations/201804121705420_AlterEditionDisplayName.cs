namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterEditionDisplayName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TicoPay.AbpEditions", "DisplayName", c => c.String(nullable: false, maxLength: 160));
        }
        
        public override void Down()
        {
            AlterColumn("TicoPay.AbpEditions", "DisplayName", c => c.String(nullable: false, maxLength: 64));
        }
    }
}
