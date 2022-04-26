namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCLientLastName : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Client", "LastName", c => c.String(nullable: false, maxLength: 80,defaultValue:"N/D"));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Client", "LastName");
        }
    }
}
