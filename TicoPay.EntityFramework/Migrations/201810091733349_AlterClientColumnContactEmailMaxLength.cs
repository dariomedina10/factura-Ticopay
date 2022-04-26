namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterClientColumnContactEmailMaxLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TicoPay.Client", "ContactEmail", c => c.String(maxLength: 180));
        }
        
        public override void Down()
        {
            AlterColumn("TicoPay.Client", "ContactEmail", c => c.String(maxLength: 60));
        }
    }
}
