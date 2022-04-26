namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class third : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.ClientServices", "WorkerFirstEjecutionDate", c => c.DateTime());
            AddColumn("TicoPay.ClientServices", "WorkerLastEjecutionDate", c => c.DateTime());
            AddColumn("TicoPay.ClientServices", "WorkerNextEjecutionDate", c => c.DateTime());
            AddColumn("TicoPay.ClientServices", "LastGeneratedInvoice", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.ClientServices", "LastGeneratedInvoice");
            DropColumn("TicoPay.ClientServices", "WorkerNextEjecutionDate");
            DropColumn("TicoPay.ClientServices", "WorkerLastEjecutionDate");
            DropColumn("TicoPay.ClientServices", "WorkerFirstEjecutionDate");
        }
    }
}
