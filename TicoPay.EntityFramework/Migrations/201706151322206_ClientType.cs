namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TicoPay.Clients", "TipoId", "TicoPay.Tipos");
            DropIndex("TicoPay.Clients", new[] { "TipoId" });
            AddColumn("TicoPay.Clients", "IdentificationType", c => c.Int(nullable: false));
            DropColumn("TicoPay.Clients", "TipoId");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Clients", "TipoId", c => c.Int(nullable: false));
            DropColumn("TicoPay.Clients", "IdentificationType");
            CreateIndex("TicoPay.Clients", "TipoId");
            AddForeignKey("TicoPay.Clients", "TipoId", "TicoPay.Tipos", "Id", cascadeDelete: true);
        }
    }
}
