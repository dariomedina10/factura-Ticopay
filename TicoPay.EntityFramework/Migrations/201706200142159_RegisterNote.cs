namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegisterNote : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Registers", "FirstNoteNumber", c => c.Long(nullable: false));
            AddColumn("TicoPay.Registers", "LastNoteNumber", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Registers", "LastNoteNumber");
            DropColumn("TicoPay.Registers", "FirstNoteNumber");
        }
    }
}
