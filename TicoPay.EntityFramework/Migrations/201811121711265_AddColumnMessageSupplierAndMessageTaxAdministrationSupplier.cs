namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnMessageSupplierAndMessageTaxAdministrationSupplier : DbMigration
    {
        public override void Up()
        {
            AddColumn("TicoPay.Vouchers", "TypeVoucher", c => c.Int(nullable: false));
            AddColumn("TicoPay.Vouchers", "MessageSupplier", c => c.Int());
            AddColumn("TicoPay.Vouchers", "MessageTaxAdministrationSupplier", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("TicoPay.Vouchers", "MessageTaxAdministrationSupplier");
            DropColumn("TicoPay.Vouchers", "MessageSupplier");
            DropColumn("TicoPay.Vouchers", "TypeVoucher");
        }
    }
}
