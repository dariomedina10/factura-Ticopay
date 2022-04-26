namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fifty : DbMigration
    {
        public override void Up()
        {
            Sql("Delete from TicoPay.ClientGroups");

            Sql("Delete from TicoPay.ClientServices");

            Sql("Delete from TicoPay.InvoiceLines");

            Sql("Delete from TicoPay.Notes");

            Sql("Delete from TicoPay.Invoices");

            Sql("Delete from TicoPay.Clients");

            CreateTable(
                "TicoPay.Barrios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreBarrio = c.String(nullable: false, maxLength: 50),
                        codigobarrio = c.String(nullable: false, maxLength: 2),
                        DistritoID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Distritoes", t => t.DistritoID, cascadeDelete: true)
                .Index(t => t.DistritoID);
            
            CreateTable(
                "TicoPay.Distritoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreDistrito = c.String(nullable: false, maxLength: 50),
                        codigodistrito = c.String(nullable: false, maxLength: 2),
                        CantonID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Cantons", t => t.CantonID, cascadeDelete: true)
                .Index(t => t.CantonID);
            
            CreateTable(
                "TicoPay.Cantons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreCanton = c.String(nullable: false, maxLength: 50),
                        codigocanton = c.String(nullable: false, maxLength: 2),
                        ProvinciaID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Provincias", t => t.ProvinciaID, cascadeDelete: true)
                .Index(t => t.ProvinciaID);
            
            CreateTable(
                "TicoPay.Provincias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreProvincia = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "TicoPay.Tipos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GrupoTipo = c.Int(nullable: false),
                        name = c.String(nullable: false, maxLength: 150),
                        codigo = c.String(nullable: false, maxLength: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "TicoPay.Monedas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombrePais = c.String(nullable: false, maxLength: 50),
                        NombreMoneda = c.String(nullable: false, maxLength: 50),
                        codigoMoneda = c.String(nullable: false, maxLength: 3),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("TicoPay.Clients", "NameComercial", c => c.String(maxLength: 80));
            AddColumn("TicoPay.Clients", "TipoId", c => c.Int(nullable: false));
            AlterColumn("TicoPay.Clients", "Name", c => c.String(nullable: false, maxLength: 80));
            AlterColumn("TicoPay.Clients", "Identification", c => c.String(nullable: false, maxLength: 12));
            AlterColumn("TicoPay.Clients", "PhoneNumber", c => c.String(maxLength: 23));
            AlterColumn("TicoPay.Clients", "MobilNumber", c => c.String(maxLength: 23));
            AlterColumn("TicoPay.Clients", "Fax", c => c.String(maxLength: 23));
            AlterColumn("TicoPay.Clients", "Email", c => c.String(maxLength: 60));
            AlterColumn("TicoPay.Clients", "ContactName", c => c.String(maxLength: 80));
            AlterColumn("TicoPay.Clients", "ContactMobilNumber", c => c.String(maxLength: 23));
            AlterColumn("TicoPay.Clients", "ContactPhoneNumber", c => c.String(maxLength: 23));
            AlterColumn("TicoPay.Clients", "ContactEmail", c => c.String(maxLength: 60));
            CreateIndex("TicoPay.Clients", "TipoId");
            AddForeignKey("TicoPay.Clients", "TipoId", "TicoPay.Tipos", "Id", cascadeDelete: true);
            DropColumn("TicoPay.Clients", "IdentificationType");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Clients", "IdentificationType", c => c.Int(nullable: false));
            DropForeignKey("TicoPay.Clients", "TipoId", "TicoPay.Tipos");
            DropForeignKey("TicoPay.Cantons", "ProvinciaID", "TicoPay.Provincias");
            DropForeignKey("TicoPay.Distritoes", "CantonID", "TicoPay.Cantons");
            DropForeignKey("TicoPay.Barrios", "DistritoID", "TicoPay.Distritoes");
            DropIndex("TicoPay.Clients", new[] { "TipoId" });
            DropIndex("TicoPay.Cantons", new[] { "ProvinciaID" });
            DropIndex("TicoPay.Distritoes", new[] { "CantonID" });
            DropIndex("TicoPay.Barrios", new[] { "DistritoID" });
            AlterColumn("TicoPay.Clients", "ContactEmail", c => c.String(maxLength: 200));
            AlterColumn("TicoPay.Clients", "ContactPhoneNumber", c => c.String(maxLength: 20));
            AlterColumn("TicoPay.Clients", "ContactMobilNumber", c => c.String(maxLength: 20));
            AlterColumn("TicoPay.Clients", "ContactName", c => c.String(maxLength: 128));
            AlterColumn("TicoPay.Clients", "Email", c => c.String(maxLength: 200));
            AlterColumn("TicoPay.Clients", "Fax", c => c.String(maxLength: 20));
            AlterColumn("TicoPay.Clients", "MobilNumber", c => c.String(maxLength: 20));
            AlterColumn("TicoPay.Clients", "PhoneNumber", c => c.String(maxLength: 20));
            AlterColumn("TicoPay.Clients", "Identification", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("TicoPay.Clients", "Name", c => c.String(nullable: false, maxLength: 128));
            DropColumn("TicoPay.Clients", "TipoId");
            DropColumn("TicoPay.Clients", "NameComercial");
            DropTable("TicoPay.Monedas");
            DropTable("TicoPay.Tipos");
            DropTable("TicoPay.Provincias");
            DropTable("TicoPay.Cantons");
            DropTable("TicoPay.Distritoes");
            DropTable("TicoPay.Barrios");
        }
    }
}
