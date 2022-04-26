namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnPaymentsPaymentOrigin : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Payments", "PaymentOrigin", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Payments", "PaymentOrigin");
        }
    }
}
