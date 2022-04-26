namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnFirmaDigital : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Invoices", "TipoFirma", c => c.Int());
            AddColumn("TicoPay.Invoices", "StatusFirmaDigital", c => c.Int());
            AddColumn("TicoPay.Notes", "TipoFirma", c => c.Int());
            AddColumn("TicoPay.Notes", "StatusFirmaDigital", c => c.Int());
            AddColumn("TicoPay.AbpTenants", "TipoFirma", c => c.Int());
            AddColumn("TicoPay.AbpTenants", "FirmaRecurrente", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpTenants", "FirmaRecurrente");
            DropColumn("TicoPay.AbpTenants", "TipoFirma");
            DropColumn("TicoPay.Notes", "StatusFirmaDigital");
            DropColumn("TicoPay.Notes", "TipoFirma");
            DropColumn("TicoPay.Invoices", "StatusFirmaDigital");
            DropColumn("TicoPay.Invoices", "TipoFirma");
        }
    }
}
