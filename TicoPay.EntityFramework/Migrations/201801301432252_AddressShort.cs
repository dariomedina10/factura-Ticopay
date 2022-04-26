namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddressShort : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "IsAddressShort", c => c.Boolean(nullable: false,defaultValue:false));
            AddColumn("TicoPay.AbpTenants", "AddressShort", c => c.String(maxLength: 160));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "AddressShort");
            DropColumn("TicoPay.AbpTenants", "IsAddressShort");
        }
    }
}
