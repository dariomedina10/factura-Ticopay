namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnSectorTenant : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "Sector", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "Sector");
        }
    }
}
