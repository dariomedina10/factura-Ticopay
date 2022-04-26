namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class invoicelineUnitMeasurement : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.InvoiceLines", "TaxId", c => c.Guid());
            AddColumn("TicoPay.InvoiceLines", "UnitMeasurement", c => c.Int(nullable: false, defaultValue: 0));
            AddColumn("TicoPay.InvoiceLines", "UnitMeasurementOthers", c => c.String());
            AddColumn("TicoPay.AbpTenants", "UnitMeasurementDefault", c => c.Int());
            AddColumn("TicoPay.AbpTenants", "UnitMeasurementOthersDefault", c => c.String());
            CreateIndex("TicoPay.InvoiceLines", "TaxId");
            AddForeignKey("TicoPay.InvoiceLines", "TaxId", "TicoPay.Taxes", "Id");

            Sql(@"update [TicoPay].[InvoiceLines] set UnitMeasurement=s.UnitMeasurement, UnitMeasurementOthers=s.UnitMeasurementOthers, 
                TaxId = s.TaxId from[TicoPay].[InvoiceLines] il inner join[TicoPay].[Services] s on il.ServiceId = s.Id");
        }

        public override void Down()
        {
            DropForeignKey("TicoPay.InvoiceLines", "TaxId", "TicoPay.Taxes");
            DropIndex("TicoPay.InvoiceLines", new[] { "TaxId" });
            DropColumn("TicoPay.AbpTenants", "UnitMeasurementOthersDefault");
            DropColumn("TicoPay.AbpTenants", "UnitMeasurementDefault");
            DropColumn("TicoPay.InvoiceLines", "UnitMeasurementOthers");
            DropColumn("TicoPay.InvoiceLines", "UnitMeasurement");
            DropColumn("TicoPay.InvoiceLines", "TaxId");
        }
    }
}