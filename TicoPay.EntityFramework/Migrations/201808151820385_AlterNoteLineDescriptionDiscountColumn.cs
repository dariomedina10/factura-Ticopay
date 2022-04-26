namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterNoteLineDescriptionDiscountColumn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TicoPay.NoteLines", "DescriptionDiscount", c => c.String(maxLength: 80));
        }
        
        public override void Down()
        {
            AlterColumn("TicoPay.NoteLines", "DescriptionDiscount", c => c.String(maxLength: 20));
        }
    }
}
