namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullAddressClient : DbMigration
    {
        public override void Up()
        {
            Sql("Update TicoPay.AbpSettings set value = 'es' where id= 3");

            DropForeignKey("TicoPay.Client", "BarrioId", "TicoPay.Barrios");
            DropIndex("TicoPay.Client", new[] { "BarrioId" });
            AlterColumn("TicoPay.Client", "BarrioId", c => c.Int());
            CreateIndex("TicoPay.Client", "BarrioId");
            AddForeignKey("TicoPay.Client", "BarrioId", "TicoPay.Barrios", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.Client", "BarrioId", "TicoPay.Barrios");
            DropIndex("TicoPay.Client", new[] { "BarrioId" });
            AlterColumn("TicoPay.Client", "BarrioId", c => c.Int(nullable: false));
            CreateIndex("TicoPay.Client", "BarrioId");
            AddForeignKey("TicoPay.Client", "BarrioId", "TicoPay.Barrios", "Id", cascadeDelete: true);
        }
    }
}
