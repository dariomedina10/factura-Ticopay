namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConvenioConectividad : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TicoPay.AgreementConectivities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantID = c.Int(nullable: false),
                        AgreementNumber = c.String(),
                        Port = c.Int(nullable: false),
                        KeyType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("TicoPay.AgreementConectivities");
        }
    }
}
