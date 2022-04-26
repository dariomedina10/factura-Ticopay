namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MotiveSuspension : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "MotiveSuspension", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "MotiveSuspension");
        }
    }
}
