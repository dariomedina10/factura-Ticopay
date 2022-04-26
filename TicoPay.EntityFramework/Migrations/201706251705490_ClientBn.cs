namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientBn : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Clients", "PagoAutomaticoBn", c => c.Boolean());
            AddColumn("TicoPay.Clients", "DiaPagoBn", c => c.Int());
            AddColumn("TicoPay.Clients", "MontoMaximoBn", c => c.Int());
            AddColumn("TicoPay.Clients", "FormaPagoBn", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Clients", "FormaPagoBn");
            DropColumn("TicoPay.Clients", "MontoMaximoBn");
            DropColumn("TicoPay.Clients", "DiaPagoBn");
            DropColumn("TicoPay.Clients", "PagoAutomaticoBn");
        }
    }
}
