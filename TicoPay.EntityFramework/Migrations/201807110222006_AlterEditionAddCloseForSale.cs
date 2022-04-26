namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterEditionAddCloseForSale : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpEditions", "CloseForSale", c => c.Boolean(defaultValue: true, nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpEditions", "CloseForSale");
        }
    }
}
