namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnIsNoteReceptionConfirmed : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Notes", "IsNoteReceptionConfirmed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Notes", "IsNoteReceptionConfirmed");
        }
    }
}
