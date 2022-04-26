namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileNameCertified : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Certificates", "FileName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Certificates", "FileName");
        }
    }
}
