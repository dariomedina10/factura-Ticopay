namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterColumnLastName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TicoPay.Client", "LastName", c => c.String(maxLength: 80, defaultValue: "N/D"));
        }
        
        public override void Down()
        {
            AlterColumn("TicoPay.Client", "LastName", c => c.String(nullable: false, maxLength: 80));
        }
    }
}
