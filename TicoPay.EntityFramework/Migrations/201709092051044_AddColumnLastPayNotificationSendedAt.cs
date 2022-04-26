namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnLastPayNotificationSendedAt : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "LastPayNotificationSendedAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "LastPayNotificationSendedAt");
        }
    }
}
