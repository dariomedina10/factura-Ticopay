namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NoteReasons : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TicoPay.Notes", "ServiceId", "TicoPay.Services");
            DropIndex("TicoPay.Notes", new[] { "ServiceId" });
            AddColumn("TicoPay.Notes", "NoteReasons", c => c.Int(nullable: false));
            DropColumn("TicoPay.Notes", "Description");
            DropColumn("TicoPay.Notes", "ServiceId");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Notes", "ServiceId", c => c.Guid());
            AddColumn("TicoPay.Notes", "Description", c => c.String());
            DropColumn("TicoPay.Notes", "NoteReasons");
            CreateIndex("TicoPay.Notes", "ServiceId");
            AddForeignKey("TicoPay.Notes", "ServiceId", "TicoPay.Services", "Id");
        }
    }
}
