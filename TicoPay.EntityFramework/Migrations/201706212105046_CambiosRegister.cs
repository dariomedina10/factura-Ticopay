namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambiosRegister : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Registers", "FirstNoteDebitNumber", c => c.Long(nullable: false));
            AddColumn("TicoPay.Registers", "LastNoteDebitNumber", c => c.Long(nullable: false));
            AddColumn("TicoPay.Registers", "FirstNoteCreditNumber", c => c.Long(nullable: false));
            AddColumn("TicoPay.Registers", "LastNoteCreditNumber", c => c.Long(nullable: false));
            DropColumn("TicoPay.Registers", "FirstNoteNumber");
            DropColumn("TicoPay.Registers", "LastNoteNumber");
        }
        
        public override void Down()
        {
            AddColumn("TicoPay.Registers", "LastNoteNumber", c => c.Long(nullable: false));
            AddColumn("TicoPay.Registers", "FirstNoteNumber", c => c.Long(nullable: false));
            DropColumn("TicoPay.Registers", "LastNoteCreditNumber");
            DropColumn("TicoPay.Registers", "FirstNoteCreditNumber");
            DropColumn("TicoPay.Registers", "LastNoteDebitNumber");
            DropColumn("TicoPay.Registers", "FirstNoteDebitNumber");
        }
    }
}
