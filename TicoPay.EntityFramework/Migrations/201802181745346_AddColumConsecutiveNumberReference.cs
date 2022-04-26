namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumConsecutiveNumberReference : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Notes", "ConsecutiveNumberReference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Notes", "ConsecutiveNumberReference");
        }
    }
}
