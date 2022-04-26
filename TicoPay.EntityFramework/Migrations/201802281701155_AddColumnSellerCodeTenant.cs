namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddColumnSellerCodeTenant : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TicoPay.Sellers",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                    InternalCode = c.String(nullable: false),
                    SellerType = c.Int(nullable: false),
                    SalesPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("TicoPay.AbpTenants", "Seller_Id", c => c.Int());
            CreateIndex("TicoPay.AbpTenants", "Seller_Id");
            AddForeignKey("TicoPay.AbpTenants", "Seller_Id", "TicoPay.Sellers", "Id");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Jessika Barreto','1000',0,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Joselis Flores','2000',0,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Alexander Muñoz','1010',0,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Maria Diaz','4000',0,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Jhony Mora','1020',1,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Eduardo Esquivel','1030',1,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Adriana','1050',1,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Daniel Cascante','2010',1,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Freddy','1040',1,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Carlos Gomez','2020',1,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Warner Villafranca','2030',1,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Jose Espinoza','2040',1,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Leonardo Jimenez','2050',1,0,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Sellers]([Name],[InternalCode],[SellerType],[SalesPercentage],[CreationTime]) 
               Values('Ninguno','999',0,0,GETDATE())");
            Sql(@"Update [TicoPay].[AbpTenants] Set [AbpTenants].[Seller_Id] = [Sellers].[Id] From [TicoPay].[AbpTenants]
                    inner join [TicoPay].[Sellers] on [AbpTenants].SellerCode = [Sellers].InternalCode");

            DropColumn("TicoPay.AbpTenants", "SellerCode");
        }

        public override void Down()
        {
            AddColumn("TicoPay.AbpTenants", "SellerCode", c => c.String());
            DropForeignKey("TicoPay.AbpTenants", "Seller_Id", "TicoPay.Sellers");
            DropIndex("TicoPay.AbpTenants", new[] { "Seller_Id" });
            DropColumn("TicoPay.AbpTenants", "Seller_Id");
            DropTable("TicoPay.Sellers");
        }
    }
}
