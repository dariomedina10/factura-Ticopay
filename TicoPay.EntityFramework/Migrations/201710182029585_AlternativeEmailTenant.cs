namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlternativeEmailTenant : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "AlternativeEmail", c => c.String(maxLength: 60));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "AlternativeEmail");
        }
    }
}
