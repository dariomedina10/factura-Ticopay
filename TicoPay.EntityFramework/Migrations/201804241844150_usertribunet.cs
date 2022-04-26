namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usertribunet : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "UserTribunet", c => c.String());
            AddColumn("TicoPay.AbpTenants", "PasswordTribunet", c => c.String());
            Sql(@"update [TicoPay].[AbpTenants] set UserTribunet='cpj-3-101-741788@stag.comprobanteselectronicos.go.cr', PasswordTribunet='iu6XEyXU46P6RvPoxgbt/tZbotHwZuaD' 
                    where Id = 2");

        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "PasswordTribunet");
            DropColumn("TicoPay.AbpTenants", "UserTribunet");
        }
    }
}
