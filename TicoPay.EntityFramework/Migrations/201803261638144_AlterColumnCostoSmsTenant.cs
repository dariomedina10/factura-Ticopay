namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterColumnCostoSmsTenant : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TicoPay.AbpTenants", "CostoSms", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("TicoPay.AbpTenants", "CostoSms", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
