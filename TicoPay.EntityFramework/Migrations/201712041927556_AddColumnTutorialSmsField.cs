namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnTutorialSmsField : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Invoices", "SmsEnviado", c => c.Boolean(nullable: false));
            AddColumn("TicoPay.AbpTenants", "IsTutorialCompania", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("TicoPay.AbpTenants", "IsTutotialServices", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("TicoPay.AbpTenants", "IsTutorialClients", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("TicoPay.AbpTenants", "SmsNoficicarFacturaACobro", c => c.Boolean(nullable: false));
            AddColumn("TicoPay.AbpTenants", "CostoSms", c => c.Decimal(nullable: false, precision: 18, scale: 2,defaultValue:10));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "CostoSms");
            DropColumn("TicoPay.AbpTenants", "SmsNoficicarFacturaACobro");
            DropColumn("TicoPay.AbpTenants", "IsTutorialClients");
            DropColumn("TicoPay.AbpTenants", "IsTutotialServices");
            DropColumn("TicoPay.AbpTenants", "IsTutorialCompania");
            DropColumn("TicoPay.Invoices", "SmsEnviado");
        }
    }
}
