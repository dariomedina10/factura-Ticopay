namespace TicoPay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCountryIdAlterProductRequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TicoPay.Products", "BrandId", "TicoPay.Brands");
            DropForeignKey("TicoPay.Products", "ProductTypeId", "TicoPay.ProductTypes");
            DropForeignKey("TicoPay.Products", "SupplierId", "TicoPay.Suppliers");
            DropIndex("TicoPay.Products", new[] { "BrandId" });
            DropIndex("TicoPay.Products", new[] { "SupplierId" });
            DropIndex("TicoPay.Products", new[] { "ProductTypeId" });
            AddColumn("TicoPay.Client", "CountryId", c => c.Int());
            AlterColumn("TicoPay.Products", "BrandId", c => c.Guid());
            AlterColumn("TicoPay.Products", "SupplierId", c => c.Guid());
            AlterColumn("TicoPay.Products", "ProductTypeId", c => c.Guid());
            CreateIndex("TicoPay.Client", "CountryId");
            CreateIndex("TicoPay.Products", "BrandId");
            CreateIndex("TicoPay.Products", "SupplierId");
            CreateIndex("TicoPay.Products", "ProductTypeId");
            AddForeignKey("TicoPay.Client", "CountryId", "TicoPay.Countries", "Id");
            AddForeignKey("TicoPay.Products", "BrandId", "TicoPay.Brands", "Id");
            AddForeignKey("TicoPay.Products", "ProductTypeId", "TicoPay.ProductTypes", "Id");
            AddForeignKey("TicoPay.Products", "SupplierId", "TicoPay.Suppliers", "Id");

            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('102','AUSTRIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('103','BELGICA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('104','BULGARIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('106','CHIPRE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('107','DINAMARCA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('108','ESPAÑA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('109','FINLANDIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('110','FRANCIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('111','GRECIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('112','HUNGRIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('113','IRLANDA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('115','ITALIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('117','LUXEMBURGO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('118','MALTA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('121','PAISES BAJOS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('122','POLONIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('123','PORTUGAL',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('125','REINO UNIDO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('126','ALEMANIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('128','RUMANIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('131','SUECIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('136','LETONIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('141','ESTONIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('142','LITUANIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('143','REPUBLICA CHECA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('144','REPUBLICA ESLOVACA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('147','ESLOVENIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('198','TERRITORIOS DE LA UNION EUROPEA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('101','ALBANIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('114','ISLANDIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('116','LIECHTENSTEIN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('119','MONACO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('120','NORUEGA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('124','ANDORRA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('129','SAN MARINO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('130','SANTA SEDE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('132','SUIZA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('135','UCRANIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('137','MOLDAVIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('138','BELARUS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('139','GEORGIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('145','BOSNIA Y HERZEGOVINA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('146','CROACIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('148','ARMENIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('154','RUSIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('156','MACEDONIA' ,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('157','SERBIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('158','MONTENEGRO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('170','GUERNESEY',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('171','SVALBARD Y JAN MAYEN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('172','ISLAS FEROE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('173','ISLA DE MAN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('174','GIBRALTAR',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('175','ISLAS DEL CANAL',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('176','JERSEY',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('177','ISLAS ALAND',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('436','TURQUIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('199','TERRITORIOS DEL RESTO DE EUROPA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('201','BURKINA FASO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('202','ANGOLA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('203','ARGELIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('204','BENIN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('205','BOTSWANA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('206','BURUNDI',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('207','CABO VERDE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('208','CAMERUN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('209','COMORES',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('210','CONGO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('211','COSTA DE MARFIL',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('212','DJIBOUTI',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('213','EGIPTO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('214','ETIOPIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('215','GABON',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('216','GAMBIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('217','GHANA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('218','GUINEA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('219','GUINEA-BISSAU',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('220','GUINEA ECUATORIAL',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('221','KENIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('222','LESOTHO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('223','LIBERIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('224','LIBIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('225','MADAGASCAR',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('226','MALAWI',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('227','MALI',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('228','MARRUECOS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('229','MAURICIO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('230','MAURITANIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('231','MOZAMBIQUE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('232','NAMIBIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('233','NIGER',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('234','NIGERIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('235','REPUBLICA CENTROAFRICANA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('236','SUDAFRICA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('237','RUANDA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('238','SANTO TOME Y PRINCIPE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('239','SENEGAL',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('240','SEYCHELLES',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('241','SIERRA LEONA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('242','SOMALIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('243','SUDAN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('244','SWAZILANDIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('245','TANZANIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('246','CHAD',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('247','TOGO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('248','TUNEZ',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('249','UGANDA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('250','REP.DEMOCRATICA DEL CONGO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('251','ZAMBIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('252','ZIMBABWE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('253','ERITREA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('260','SANTA HELENA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('261','REUNION',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('262','MAYOTTE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('263','SAHARA OCCIDENTAL',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('299','TERRITORIOS DE AFRICA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('301','CANADA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('302','ESTADOS UNIDOS DE AMERICA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('303','MEXICO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('370','SAN PEDRO Y MIQUELON' ,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('371','GROENLANDIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('396','TERRITORIOS DE AMERICA DEL NORTE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('310','ANTIGUA Y BARBUDA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('311','BAHAMAS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('312','BARBADOS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('313','BELICE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('315','CUBA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('316','DOMINICA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('317','EL SALVADOR',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('318','GRANADA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('319','GUATEMALA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('320','HAITI',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('321','HONDURAS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('322','JAMAICA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('323','NICARAGUA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('324','PANAMA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('325','SAN VICENTE Y LAS GRANADINAS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('326','REPUBLICA DOMINICANA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('327','TRINIDAD Y TOBAGO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('328','SANTA LUCIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('329','SAN CRISTOBAL Y NIEVES',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('380','ISLAS CAIMÁN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('381','ISLAS TURCAS Y CAICOS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('382','ISLAS VÍRGENES DE LOS ESTADOS UNIDOS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('383','GUADALUPE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('384','ANTILLAS HOLANDESAS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('385','SAN MARTIN (PARTE FRANCESA)',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('386','ARUBA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('387','MONTSERRAT',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('388','ANGUILLA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('389','SAN BARTOLOME',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('390','MARTINICA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('391','PUERTO RICO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('392','BERMUDAS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('393','ISLAS VIRGENES BRITANICAS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('398','TERRITORIOS DEL CARIBE Y AMERICA CENTRAL',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('340','ARGENTINA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('341','BOLIVIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('342','BRASIL',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('343','COLOMBIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('344','CHILE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('345','ECUADOR',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('346','GUYANA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('347','PARAGUAY',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('348','PERU',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('349','SURINAM',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('350','URUGUAY',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('351','VENEZUELA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('394','GUAYANA FRANCESA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('395','ISLAS MALVINAS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('399','TERRITORIOS  DE SUDAMERICA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('401','AFGANISTAN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('402','ARABIA SAUDI',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('403','BAHREIN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('404','BANGLADESH',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('405','MYANMAR',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('407','CHINA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('408','EMIRATOS ARABES UNIDOS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('409','FILIPINAS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('410','INDIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('411','INDONESIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('412','IRAQ',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('413','IRAN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('414','ISRAEL',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('415','JAPON',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('416','JORDANIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('417','CAMBOYA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('418','KUWAIT',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('419','LAOS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('420','LIBANO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('421','MALASIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('422','MALDIVAS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('423','MONGOLIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('424','NEPAL',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('425','OMAN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('426','PAKISTAN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('427','QATAR',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('430','COREA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('431','COREA DEL NORTE' ,GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('432','SINGAPUR',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('433','SIRIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('434','SRI LANKA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('435','TAILANDIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('437','VIETNAM',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('439','BRUNEI',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('440','ISLAS MARSHALL',GETDATE())");
            Sql(@"INSERT INTO[TicoPay].[Countries]([CountryCode],[CountryName],[ResolutionDate])
               Values('441', 'YEMEN', GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('442','AZERBAIYAN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('443','KAZAJSTAN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('444','KIRGUISTAN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('445','TADYIKISTAN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('446','TURKMENISTAN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('447','UZBEKISTAN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('448','ISLAS MARIANAS DEL NORTE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('449','PALESTINA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('450','HONG KONG',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('453','BHUTÁN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('454','GUAM',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('455','MACAO',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('499','TERRITORIOS DE ASIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('501','AUSTRALIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('502','FIJI',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('504','NUEVA ZELANDA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('505','PAPUA NUEVA GUINEA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('506','ISLAS SALOMON',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('507','SAMOA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('508','TONGA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('509','VANUATU',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('511','MICRONESIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('512','TUVALU',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('513','ISLAS COOK',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('515','NAURU',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('516','PALAOS',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('517','TIMOR ORIENTAL',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('520','POLINESIA FRANCESA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('521','ISLA NORFOLK',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('522','KIRIBATI',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('523','NIUE',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('524','ISLAS PITCAIRN',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('525','TOKELAU',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('526','NUEVA CALEDONIA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('527','WALLIS Y FORTUNA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('528','SAMOA AMERICANA',GETDATE())");
            Sql(@"INSERT INTO [TicoPay].[Countries] ([CountryCode],[CountryName],[ResolutionDate]) 
               Values('599','TERRITORIOS DE OCEANIA',GETDATE())");
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.Products", "SupplierId", "TicoPay.Suppliers");
            DropForeignKey("TicoPay.Products", "ProductTypeId", "TicoPay.ProductTypes");
            DropForeignKey("TicoPay.Products", "BrandId", "TicoPay.Brands");
            DropForeignKey("TicoPay.Client", "CountryId", "TicoPay.Countries");
            DropIndex("TicoPay.Products", new[] { "ProductTypeId" });
            DropIndex("TicoPay.Products", new[] { "SupplierId" });
            DropIndex("TicoPay.Products", new[] { "BrandId" });
            DropIndex("TicoPay.Client", new[] { "CountryId" });
            AlterColumn("TicoPay.Products", "ProductTypeId", c => c.Guid(nullable: false));
            AlterColumn("TicoPay.Products", "SupplierId", c => c.Guid(nullable: false));
            AlterColumn("TicoPay.Products", "BrandId", c => c.Guid(nullable: false));
            DropColumn("TicoPay.Client", "CountryId");
            CreateIndex("TicoPay.Products", "ProductTypeId");
            CreateIndex("TicoPay.Products", "SupplierId");
            CreateIndex("TicoPay.Products", "BrandId");
            AddForeignKey("TicoPay.Products", "SupplierId", "TicoPay.Suppliers", "Id", cascadeDelete: true);
            AddForeignKey("TicoPay.Products", "ProductTypeId", "TicoPay.ProductTypes", "Id", cascadeDelete: true);
            AddForeignKey("TicoPay.Products", "BrandId", "TicoPay.Brands", "Id", cascadeDelete: true);
        }
    }
}
