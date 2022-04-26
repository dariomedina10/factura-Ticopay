namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterTicopayEditionAddTypeEdition : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpEditions", "EditionType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpEditions", "EditionType");
        }
    }
}
