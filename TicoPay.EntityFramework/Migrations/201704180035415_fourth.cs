namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fourth : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TicoPay.Clients", "Code", c => c.Long(nullable: false, identity: true));
        }
        
        public override void Down()
        {
            AlterColumn("TicoPay.Clients", "Code", c => c.String(maxLength: 10));
        }
    }
}
