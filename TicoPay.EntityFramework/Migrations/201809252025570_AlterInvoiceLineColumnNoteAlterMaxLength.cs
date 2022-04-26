namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterInvoiceLineColumnNoteAlterMaxLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TicoPay.InvoiceLines", "Note", c => c.String(maxLength: 400));
            AlterColumn("TicoPay.NoteLines", "Notes", c => c.String(maxLength: 400));
        }
        
        public override void Down()
        {
            AlterColumn("TicoPay.NoteLines", "Notes", c => c.String(maxLength: 200));
            AlterColumn("TicoPay.InvoiceLines", "Note", c => c.String(maxLength: 200));
        }
    }
}
