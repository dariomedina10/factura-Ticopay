namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Column_Logo : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "Logo", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "Logo");
        }
    }
}
