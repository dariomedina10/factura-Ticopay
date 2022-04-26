namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddColumnasForInvoicesMonthlyLimitNotifications : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpTenants", "InvoicesMonthlyLimitNotificationInterval", c => c.Int(nullable: false, defaultValue: 7));
            AddColumn("TicoPay.AbpTenants", "NearInvoicesMonthlyLimitNotificationInterval", c => c.Int(nullable: false, defaultValue: 7));
            AddColumn("TicoPay.AbpTenants", "LastInvoicesMonthlyLimitNotificationDate", c => c.DateTime());
            AddColumn("TicoPay.AbpTenants", "LastNearInvoicesMonthlyLimitNotificationDate", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "LastNearInvoicesMonthlyLimitNotificationDate");
            DropColumn("TicoPay.AbpTenants", "LastInvoicesMonthlyLimitNotificationDate");
            DropColumn("TicoPay.AbpTenants", "NearInvoicesMonthlyLimitNotificationInterval");
            DropColumn("TicoPay.AbpTenants", "InvoicesMonthlyLimitNotificationInterval");
        }
    }
}
