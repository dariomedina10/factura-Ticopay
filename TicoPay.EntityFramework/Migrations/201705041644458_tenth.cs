namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tenth : DbMigration
    {
        public override void Up()
        {
            Sql("Delete from TicoPay.InvoiceLines");

            Sql("Delete from TicoPay.Notes");

            Sql("Delete from TicoPay.Invoices");

            CreateTable(
                "TicoPay.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CountryName = c.String(nullable: false, maxLength: 50),
                        CountryCode = c.String(nullable: false, maxLength: 3),
                        ResolutionNumber = c.String(maxLength: 13),
                        ResolutionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("TicoPay.Invoices", "ChangeType", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("TicoPay.Invoices", "TotalServGravados", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("TicoPay.Invoices", "TotalServExento", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("TicoPay.Invoices", "TotalProductExento", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("TicoPay.Invoices", "TotalProductGravado", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("TicoPay.Invoices", "TotalGravado", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("TicoPay.Invoices", "TotalExento", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("TicoPay.Invoices", "SaleTotal", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("TicoPay.Invoices", "NetaSale", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AddColumn("TicoPay.AbpTenants", "CoinsID", c => c.Int());
            AddColumn("TicoPay.AbpTenants", "local", c => c.String(maxLength: 3));
            AddColumn("TicoPay.AbpTenants", "ConditionSaleID", c => c.Int());
            AddColumn("TicoPay.AbpTenants", "CountryID", c => c.Int());
            AddColumn("TicoPay.AbpTenants", "BussinesName", c => c.String(maxLength: 80));
            AddColumn("TicoPay.AbpTenants", "IdentificationTypeID", c => c.Int());
            AddColumn("TicoPay.AbpTenants", "IdentificationNumber", c => c.String(maxLength: 12));
            AddColumn("TicoPay.AbpTenants", "ComercialName", c => c.String(maxLength: 80));
            AddColumn("TicoPay.AbpTenants", "BarrioId", c => c.Int());
            AddColumn("TicoPay.AbpTenants", "Address", c => c.String(maxLength: 160));
            AddColumn("TicoPay.AbpTenants", "PhoneNumber", c => c.String(maxLength: 23));
            AddColumn("TicoPay.AbpTenants", "Fax", c => c.String(maxLength: 23));
            AddColumn("TicoPay.AbpTenants", "Email", c => c.String(maxLength: 60));
            AlterColumn("TicoPay.Invoices", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Invoices", "DiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Invoices", "TotalTax", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.Invoices", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("TicoPay.InvoiceLines", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            CreateIndex("TicoPay.AbpTenants", "CoinsID");
            CreateIndex("TicoPay.AbpTenants", "ConditionSaleID");
            CreateIndex("TicoPay.AbpTenants", "CountryID");
            CreateIndex("TicoPay.AbpTenants", "IdentificationTypeID");
            CreateIndex("TicoPay.AbpTenants", "BarrioId");
            AddForeignKey("TicoPay.AbpTenants", "BarrioId", "TicoPay.Barrios", "Id");
            AddForeignKey("TicoPay.AbpTenants", "CoinsID", "TicoPay.Monedas", "Id");
            AddForeignKey("TicoPay.AbpTenants", "ConditionSaleID", "TicoPay.Tipos", "Id");
            AddForeignKey("TicoPay.AbpTenants", "CountryID", "TicoPay.Countries", "Id");
            AddForeignKey("TicoPay.AbpTenants", "IdentificationTypeID", "TicoPay.Tipos", "Id");
            DropColumn("TicoPay.Invoices", "QRCode");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Invoices", "QRCode", c => c.String(maxLength: 50));
            DropForeignKey("TicoPay.AbpTenants", "IdentificationTypeID", "TicoPay.Tipos");
            DropForeignKey("TicoPay.AbpTenants", "CountryID", "TicoPay.Countries");
            DropForeignKey("TicoPay.AbpTenants", "ConditionSaleID", "TicoPay.Tipos");
            DropForeignKey("TicoPay.AbpTenants", "CoinsID", "TicoPay.Monedas");
            DropForeignKey("TicoPay.AbpTenants", "BarrioId", "TicoPay.Barrios");
            DropIndex("TicoPay.AbpTenants", new[] { "BarrioId" });
            DropIndex("TicoPay.AbpTenants", new[] { "IdentificationTypeID" });
            DropIndex("TicoPay.AbpTenants", new[] { "CountryID" });
            DropIndex("TicoPay.AbpTenants", new[] { "ConditionSaleID" });
            DropIndex("TicoPay.AbpTenants", new[] { "CoinsID" });
            AlterColumn("TicoPay.InvoiceLines", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Invoices", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Invoices", "TotalTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Invoices", "DiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("TicoPay.Invoices", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("TicoPay.AbpTenants", "Email");
            DropColumn("TicoPay.AbpTenants", "Fax");
            DropColumn("TicoPay.AbpTenants", "PhoneNumber");
            DropColumn("TicoPay.AbpTenants", "Address");
            DropColumn("TicoPay.AbpTenants", "BarrioId");
            DropColumn("TicoPay.AbpTenants", "ComercialName");
            DropColumn("TicoPay.AbpTenants", "IdentificationNumber");
            DropColumn("TicoPay.AbpTenants", "IdentificationTypeID");
            DropColumn("TicoPay.AbpTenants", "BussinesName");
            DropColumn("TicoPay.AbpTenants", "CountryID");
            DropColumn("TicoPay.AbpTenants", "ConditionSaleID");
            DropColumn("TicoPay.AbpTenants", "local");
            DropColumn("TicoPay.AbpTenants", "CoinsID");
            DropColumn("TicoPay.Invoices", "NetaSale");
            DropColumn("TicoPay.Invoices", "SaleTotal");
            DropColumn("TicoPay.Invoices", "TotalExento");
            DropColumn("TicoPay.Invoices", "TotalGravado");
            DropColumn("TicoPay.Invoices", "TotalProductGravado");
            DropColumn("TicoPay.Invoices", "TotalProductExento");
            DropColumn("TicoPay.Invoices", "TotalServExento");
            DropColumn("TicoPay.Invoices", "TotalServGravados");
            DropColumn("TicoPay.Invoices", "ChangeType");
            DropTable("TicoPay.Countries");
        }
    }
}
