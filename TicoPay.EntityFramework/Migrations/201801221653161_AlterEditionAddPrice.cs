namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class AlterEditionAddPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.AbpEditions", "Price", c => c.Int(nullable: false, defaultValue: 0));
            AddColumn("TicoPay.AbpEditions", "Discriminator", c => c.String(nullable: false, maxLength: 128,defaultValue: "TicoPayEdition"));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.AbpEditions", "Discriminator");
            DropColumn("TicoPay.AbpEditions", "Price");           
        }
    }
}
