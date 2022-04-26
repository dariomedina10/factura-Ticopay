namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TicoPay.Addresses",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    City = c.String(),
                    State = c.String(),
                    Country = c.String(),
                    PostCode = c.String(),
                    Suburb = c.String(),
                    StreetNumber = c.String(),
                    Address1 = c.String(),
                    Address2 = c.String(),
                    Primary = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    TenantId = c.Int(nullable: false),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Address_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Address_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.AgreementConectivities",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    TenantID = c.Int(nullable: false),
                    AgreementNumbers = c.Int(nullable: false),
                    Port = c.Int(nullable: false),
                    KeyType = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.AbpAuditLogs",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    UserId = c.Long(),
                    ServiceName = c.String(maxLength: 256),
                    MethodName = c.String(maxLength: 256),
                    Parameters = c.String(maxLength: 1024),
                    ExecutionTime = c.DateTime(nullable: false),
                    ExecutionDuration = c.Int(nullable: false),
                    ClientIpAddress = c.String(maxLength: 64),
                    ClientName = c.String(maxLength: 128),
                    BrowserInfo = c.String(maxLength: 256),
                    Exception = c.String(maxLength: 2000),
                    ImpersonatorUserId = c.Long(),
                    ImpersonatorTenantId = c.Int(),
                    CustomData = c.String(maxLength: 2000),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_AuditLog_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.AbpBackgroundJobs",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    JobType = c.String(nullable: false, maxLength: 512),
                    JobArgs = c.String(nullable: false),
                    TryCount = c.Short(nullable: false),
                    NextTryTime = c.DateTime(nullable: false),
                    LastTryTime = c.DateTime(),
                    IsAbandoned = c.Boolean(nullable: false),
                    Priority = c.Byte(nullable: false),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.IsAbandoned, t.NextTryTime });

            CreateTable(
                "TicoPay.Barrios",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    NombreBarrio = c.String(nullable: false, maxLength: 50),
                    codigobarrio = c.String(nullable: false, maxLength: 2),
                    DistritoID = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Distritoes", t => t.DistritoID, cascadeDelete: true)
                .Index(t => t.DistritoID);

            CreateTable(
                "TicoPay.Client",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 200),
                    NameComercial = c.String(maxLength: 80),
                    IdentificationType = c.Int(nullable: false),
                    Identification = c.String(nullable: false, maxLength: 50),
                    IdentificacionExtranjero = c.String(maxLength: 20),
                    Birthday = c.DateTime(),
                    Code = c.Int(nullable: false, identity: true),
                    Gender = c.Int(nullable: false),
                    PhoneNumber = c.String(maxLength: 15),
                    MobilNumber = c.String(maxLength: 15),
                    Fax = c.String(maxLength: 23),
                    Email = c.String(maxLength: 100),
                    WebSite = c.String(maxLength: 1000),
                    BarrioId = c.Int(nullable: false),
                    PostalCode = c.String(maxLength: 15),
                    IsoCountry = c.String(maxLength: 3),
                    Address = c.String(maxLength: 200),
                    Latitud = c.Long(),
                    Longitud = c.Long(),
                    ContactName = c.String(maxLength: 80),
                    ContactMobilNumber = c.String(maxLength: 23),
                    ContactPhoneNumber = c.String(maxLength: 23),
                    ContactEmail = c.String(maxLength: 60),
                    Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    HasImage = c.Boolean(nullable: false),
                    ImageUrl = c.String(maxLength: 1000),
                    Note = c.String(maxLength: 250),
                    AllowEmailNotifications = c.Boolean(nullable: false),
                    AllowSmsNotifications = c.Boolean(nullable: false),
                    State = c.Int(nullable: false),
                    PagoAutomaticoBn = c.Boolean(),
                    DiaPagoBn = c.Int(),
                    MontoMaximoBn = c.Int(),
                    FormaPagoBn = c.Int(),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    TenantId = c.Int(nullable: false),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Client_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Client_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Barrios", t => t.BarrioId, cascadeDelete: true)
                .Index(t => t.Code, unique: true)
                .Index(t => t.BarrioId);

            CreateTable(
                "TicoPay.GroupConcepts",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Name = c.String(maxLength: 60),
                    Description = c.String(maxLength: 500),
                    CronExpression = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    WorkerFirstEjecutionDate = c.DateTime(),
                    WorkerLastEjecutionDate = c.DateTime(),
                    WorkerNextEjecutionDate = c.DateTime(),
                    LastGeneratedInvoice = c.Int(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_GroupConcepts_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_GroupConcepts_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.Services",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    CronExpression = c.String(),
                    Name = c.String(maxLength: 160),
                    UnitMeasurement = c.Int(nullable: false),
                    UnitMeasurementOthers = c.String(),
                    Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    TaxId = c.Guid(),
                    TenantId = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    DeletionTime = c.DateTime(),
                    DeleterUserId = c.Long(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Service_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Service_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Taxes", t => t.TaxId)
                .Index(t => t.TaxId);

            CreateTable(
                "TicoPay.ClientServices",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    ServiceId = c.Guid(nullable: false),
                    ClientId = c.Guid(nullable: false),
                    Code = c.String(),
                    InitDate = c.DateTime(nullable: false),
                    AllowLatePayment = c.Boolean(nullable: false),
                    WorkerFirstEjecutionDate = c.DateTime(),
                    WorkerLastEjecutionDate = c.DateTime(),
                    WorkerNextEjecutionDate = c.DateTime(),
                    LastGeneratedInvoice = c.Int(),
                    State = c.Int(nullable: false),
                    GeneratingInvoice = c.Boolean(nullable: false),
                    TenantId = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    CronExpression = c.String(),
                    DateEvent = c.DateTime(nullable: false),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ClientService_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ClientService_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Client", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("TicoPay.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId)
                .Index(t => t.ClientId);

            CreateTable(
                "TicoPay.Invoices",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Number = c.Int(nullable: false),
                    Alphanumeric = c.String(maxLength: 50),
                    Note = c.String(maxLength: 500),
                    SubTotal = c.Decimal(nullable: false, precision: 18, scale: 5),
                    DiscountPercentaje = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DiscountAmount = c.Decimal(nullable: false, precision: 18, scale: 5),
                    TotalTax = c.Decimal(nullable: false, precision: 18, scale: 5),
                    Total = c.Decimal(nullable: false, precision: 18, scale: 5),
                    Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DueDate = c.DateTime(nullable: false),
                    Status = c.Int(nullable: false),
                    ClientId = c.Guid(),
                    RegisterId = c.Guid(),
                    UserId = c.Guid(),
                    UserName = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    ConditionSaleType = c.Int(nullable: false),
                    OtherConditionSale = c.String(maxLength: 100),
                    CreditTerm = c.Int(nullable: false),
                    CodigoMoneda = c.Int(nullable: false),
                    StatusVoucher = c.Int(nullable: false),
                    MessageTaxAdministration = c.String(),
                    MessageReceiver = c.String(),
                    ElectronicBill = c.String(),
                    ElectronicBillDocPDF = c.Binary(),
                    QRCode = c.Binary(),
                    VoucherKey = c.String(maxLength: 50),
                    ConsecutiveNumber = c.String(maxLength: 20),
                    ChangeType = c.Decimal(nullable: false, precision: 18, scale: 2),
                    TotalServGravados = c.Decimal(nullable: false, precision: 18, scale: 5),
                    TotalServExento = c.Decimal(nullable: false, precision: 18, scale: 5),
                    TotalProductExento = c.Decimal(nullable: false, precision: 18, scale: 5),
                    TotalProductGravado = c.Decimal(nullable: false, precision: 18, scale: 5),
                    TotalGravado = c.Decimal(nullable: false, precision: 18, scale: 5),
                    TotalExento = c.Decimal(nullable: false, precision: 18, scale: 5),
                    SaleTotal = c.Decimal(nullable: false, precision: 18, scale: 5),
                    NetaSale = c.Decimal(nullable: false, precision: 18, scale: 5),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                    ClientService_Id = c.Guid(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Invoice_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Invoice_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Client", t => t.ClientId)
                .ForeignKey("TicoPay.Registers", t => t.RegisterId)
                .ForeignKey("TicoPay.AbpTenants", t => t.TenantId, cascadeDelete: true)
                .ForeignKey("TicoPay.ClientServices", t => t.ClientService_Id)
                .Index(t => t.TenantId)
                .Index(t => t.Number)
                .Index(t => t.Balance)
                .Index(t => t.DueDate)
                .Index(t => t.Status)
                .Index(t => t.ClientId)
                .Index(t => t.RegisterId)
                .Index(t => t.ClientService_Id);

            CreateTable(
                "TicoPay.InvoiceHistoryStatus",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Status = c.Int(nullable: false),
                    InvoiceId = c.Guid(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_InvoiceHistoryStatus_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_InvoiceHistoryStatus_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .Index(t => t.InvoiceId);

            CreateTable(
                "TicoPay.InvoiceLines",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    PricePerUnit = c.Decimal(nullable: false, precision: 18, scale: 5),
                    TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 5),
                    Total = c.Decimal(nullable: false, precision: 18, scale: 5),
                    DiscountPercentage = c.Decimal(nullable: false, precision: 18, scale: 5),
                    Note = c.String(maxLength: 200),
                    Title = c.String(maxLength: 50),
                    Quantity = c.Decimal(nullable: false, precision: 16, scale: 3),
                    InvoiceId = c.Guid(nullable: false),
                    LineType = c.Int(nullable: false),
                    ServiceId = c.Guid(),
                    ProductId = c.Guid(),
                    LineNumber = c.Int(nullable: false),
                    CodeTypes = c.Int(nullable: false),
                    DescriptionDiscount = c.String(maxLength: 20),
                    SubTotal = c.Decimal(nullable: false, precision: 18, scale: 5),
                    LineTotal = c.Decimal(nullable: false, precision: 18, scale: 5),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    ExonerationId = c.Guid(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_InvoiceLine_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_InvoiceLine_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Exonerations", t => t.ExonerationId)
                .ForeignKey("TicoPay.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("TicoPay.Products", t => t.ProductId)
                .ForeignKey("TicoPay.Services", t => t.ServiceId)
                .Index(t => t.InvoiceId)
                .Index(t => t.ServiceId)
                .Index(t => t.ProductId)
                .Index(t => t.ExonerationId);

            CreateTable(
                "TicoPay.Exonerations",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    DocumentType = c.String(maxLength: 2),
                    DocumentNumber = c.String(maxLength: 17),
                    InstitutionName = c.String(maxLength: 100),
                    ExonerationDate = c.DateTime(nullable: false),
                    TaxAmountExonerated = c.Decimal(nullable: false, precision: 18, scale: 5),
                    PercentagePurchaseExonerated = c.Int(nullable: false),
                    UserId = c.Guid(),
                    UserName = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Exoneration_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Exoneration_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.Products",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Name = c.String(),
                    Note = c.String(),
                    RetailPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    SupplyPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Markup = c.Decimal(nullable: false, precision: 18, scale: 2),
                    TotalInStock = c.Int(nullable: false),
                    TaxId = c.Guid(nullable: false),
                    BrandId = c.Guid(nullable: false),
                    SupplierId = c.Guid(nullable: false),
                    SupplierCode = c.String(),
                    SalesAccountCode = c.String(),
                    ProductTypeId = c.Guid(nullable: false),
                    CanBeSold = c.Boolean(nullable: false),
                    HasVariants = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Product_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Product_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Brands", t => t.BrandId, cascadeDelete: true)
                .ForeignKey("TicoPay.ProductTypes", t => t.ProductTypeId, cascadeDelete: true)
                .ForeignKey("TicoPay.Suppliers", t => t.SupplierId, cascadeDelete: true)
                .ForeignKey("TicoPay.Taxes", t => t.TaxId, cascadeDelete: true)
                .Index(t => t.TaxId)
                .Index(t => t.BrandId)
                .Index(t => t.SupplierId)
                .Index(t => t.ProductTypeId);

            CreateTable(
                "TicoPay.Brands",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Name = c.String(),
                    Description = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Brand_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Brand_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.ProductTypes",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Name = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ProductType_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ProductType_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.Suppliers",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Name = c.String(),
                    Description = c.String(),
                    DefaultMarkup = c.Decimal(nullable: false, precision: 18, scale: 2),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Supplier_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Supplier_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.ProductTags",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    ProductId = c.Guid(nullable: false),
                    TagId = c.Guid(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ProductTag_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ProductTag_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("TicoPay.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.TagId);

            CreateTable(
                "TicoPay.Tags",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Name = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Tag_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Tag_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.Taxes",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Name = c.String(nullable: false),
                    Rate = c.Decimal(nullable: false, precision: 4, scale: 2),
                    TaxTypes = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Tax_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Tax_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.ProductVariants",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    ProductVariantType = c.Int(nullable: false),
                    Value = c.String(),
                    InvoiceId = c.Guid(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                    Product_Id = c.Guid(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ProductVariant_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ProductVariant_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("TicoPay.Products", t => t.Product_Id)
                .Index(t => t.InvoiceId)
                .Index(t => t.Product_Id);

            CreateTable(
                "TicoPay.PaymentInvoices",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PaymentDate = c.DateTime(nullable: false),
                    InvoiceId = c.Guid(nullable: false),
                    ExchangeRateId = c.Guid(),
                    CodigoMoneda = c.Int(nullable: false),
                    PaymentInvoiceType = c.Int(nullable: false),
                    PaymetnMethodType = c.Int(nullable: false),
                    Transaction = c.String(),
                    Reference = c.String(),
                    CodigoBanco = c.Int(),
                    CodigoAgencia = c.Int(),
                    CodigoTransaccion = c.Int(),
                    ConsecutivoTransaccion = c.Int(),
                    NotaCredito = c.Int(),
                    CodigoBancoEmisor = c.Int(),
                    NumeroCuenta = c.Int(),
                    NumeroCheque = c.Int(),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_PaymentInvoice_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_PaymentInvoice_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.ExchangeRates", t => t.ExchangeRateId)
                .ForeignKey("TicoPay.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .Index(t => t.InvoiceId)
                .Index(t => t.ExchangeRateId);

            CreateTable(
                "TicoPay.ExchangeRates",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    FromCurrencyCode = c.String(nullable: false, maxLength: 3),
                    ToCurrencyCode = c.String(nullable: false, maxLength: 3),
                    AverageRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    EndOfDayRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CurrencyRateDate = c.DateTime(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ExchangeRate_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ExchangeRate_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.Notes",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                    InvoiceId = c.Guid(nullable: false),
                    ExchangeRateId = c.Guid(),
                    Reference = c.String(),
                    CodigoMoneda = c.Int(nullable: false),
                    NoteReasons = c.Int(nullable: false),
                    MessageTaxAdministration = c.String(),
                    MessageReceiver = c.String(),
                    ElectronicBill = c.String(),
                    ElectronicBillDocPDF = c.Binary(),
                    QRCode = c.Binary(),
                    VoucherKey = c.String(maxLength: 50),
                    ConsecutiveNumber = c.String(maxLength: 20),
                    ChangeType = c.Decimal(nullable: false, precision: 18, scale: 2),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    NoteType = c.Int(nullable: false),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Note_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Note_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.ExchangeRates", t => t.ExchangeRateId)
                .ForeignKey("TicoPay.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .Index(t => t.InvoiceId)
                .Index(t => t.ExchangeRateId);

            CreateTable(
                "TicoPay.Registers",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Name = c.String(maxLength: 60),
                    RegisterCode = c.String(maxLength: 5),
                    FirstInvoiceNumber = c.Long(nullable: false),
                    LastInvoiceNumber = c.Long(nullable: false),
                    FirstNoteDebitNumber = c.Long(nullable: false),
                    LastNoteDebitNumber = c.Long(nullable: false),
                    FirstNoteCreditNumber = c.Long(nullable: false),
                    LastNoteCreditNumber = c.Long(nullable: false),
                    InvoicePrefix = c.String(maxLength: 10),
                    InvoiceSufix = c.String(maxLength: 10),
                    ShowDiscountOnInvoice = c.Boolean(nullable: false),
                    AskForANoteOnSLAR = c.Boolean(nullable: false),
                    AskForANoteOnAllSales = c.Boolean(nullable: false),
                    PrintInvoice = c.Boolean(nullable: false),
                    EmailInvoice = c.Boolean(nullable: false),
                    SelectUserForNextSale = c.Boolean(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Register_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Register_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.AbpTenants",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    CodigoMoneda = c.Int(nullable: false),
                    local = c.String(maxLength: 3),
                    ConditionSaleType = c.Int(nullable: false),
                    CountryID = c.Int(),
                    BussinesName = c.String(maxLength: 80),
                    IdentificationType = c.Int(nullable: false),
                    IdentificationNumber = c.String(maxLength: 12),
                    ComercialName = c.String(maxLength: 80),
                    BarrioId = c.Int(),
                    Address = c.String(maxLength: 160),
                    PhoneNumber = c.String(maxLength: 23),
                    Fax = c.String(maxLength: 23),
                    Email = c.String(maxLength: 60),
                    GeneratesAutomaticClientCodeSetting = c.Boolean(nullable: false),
                    EditionId = c.Int(),
                    Name = c.String(nullable: false, maxLength: 128),
                    IsActive = c.Boolean(nullable: false),
                    TenancyName = c.String(nullable: false, maxLength: 64),
                    ConnectionString = c.String(maxLength: 1024),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Tenant_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Barrios", t => t.BarrioId)
                .ForeignKey("TicoPay.Countries", t => t.CountryID)
                .ForeignKey("TicoPay.AbpUsers", t => t.CreatorUserId)
                .ForeignKey("TicoPay.AbpUsers", t => t.DeleterUserId)
                .ForeignKey("TicoPay.AbpEditions", t => t.EditionId)
                .ForeignKey("TicoPay.AbpUsers", t => t.LastModifierUserId)
                .Index(t => t.CountryID)
                .Index(t => t.BarrioId)
                .Index(t => t.EditionId)
                .Index(t => t.DeleterUserId)
                .Index(t => t.LastModifierUserId)
                .Index(t => t.CreatorUserId);

            CreateTable(
                "TicoPay.Countries",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    CountryName = c.String(nullable: false, maxLength: 50),
                    CountryCode = c.String(nullable: false, maxLength: 3),
                    ResolutionNumber = c.String(maxLength: 13),
                    ResolutionDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.AbpUsers",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    AuthenticationSource = c.String(maxLength: 64),
                    Name = c.String(nullable: false, maxLength: 32),
                    Surname = c.String(nullable: false, maxLength: 32),
                    Password = c.String(nullable: false, maxLength: 128),
                    IsEmailConfirmed = c.Boolean(nullable: false),
                    EmailConfirmationCode = c.String(maxLength: 128),
                    PasswordResetCode = c.String(maxLength: 328),
                    IsActive = c.Boolean(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 32),
                    TenantId = c.Int(),
                    EmailAddress = c.String(nullable: false, maxLength: 256),
                    LastLoginTime = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_User_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_User_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.AbpUsers", t => t.CreatorUserId)
                .ForeignKey("TicoPay.AbpUsers", t => t.DeleterUserId)
                .ForeignKey("TicoPay.AbpUsers", t => t.LastModifierUserId)
                .Index(t => t.DeleterUserId)
                .Index(t => t.LastModifierUserId)
                .Index(t => t.CreatorUserId);

            CreateTable(
                "TicoPay.AbpUserLogins",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    UserId = c.Long(nullable: false),
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 256),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserLogin_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.AbpUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "TicoPay.AbpPermissions",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    Name = c.String(nullable: false, maxLength: 128),
                    IsGranted = c.Boolean(nullable: false),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                    UserId = c.Long(),
                    RoleId = c.Int(),
                    Discriminator = c.String(nullable: false, maxLength: 128),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_PermissionSetting_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_RolePermissionSetting_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_UserPermissionSetting_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.AbpUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("TicoPay.AbpRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "TicoPay.AbpUserRoles",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    UserId = c.Long(nullable: false),
                    RoleId = c.Int(nullable: false),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserRole_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.AbpUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "TicoPay.AbpSettings",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    UserId = c.Long(),
                    Name = c.String(nullable: false, maxLength: 256),
                    Value = c.String(maxLength: 2000),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Setting_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.AbpUsers", t => t.UserId)
                .Index(t => t.UserId);

            CreateTable(
                "TicoPay.AbpEditions",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 32),
                    DisplayName = c.String(nullable: false, maxLength: 64),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Edition_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.ClientGroups",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    GroupId = c.Guid(nullable: false),
                    ClientId = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ClientGroup_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ClientGroup_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Client", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("TicoPay.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.ClientId);

            CreateTable(
                "TicoPay.Groups",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Name = c.String(maxLength: 60),
                    Description = c.String(maxLength: 1024),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Group_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Group_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.Distritoes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    NombreDistrito = c.String(nullable: false, maxLength: 50),
                    codigodistrito = c.String(nullable: false, maxLength: 2),
                    CantonID = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Cantons", t => t.CantonID, cascadeDelete: true)
                .Index(t => t.CantonID);

            CreateTable(
                "TicoPay.Cantons",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    NombreCanton = c.String(nullable: false, maxLength: 50),
                    codigocanton = c.String(nullable: false, maxLength: 2),
                    ProvinciaID = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Provincias", t => t.ProvinciaID, cascadeDelete: true)
                .Index(t => t.ProvinciaID);

            CreateTable(
                "TicoPay.Provincias",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    NombreProvincia = c.String(nullable: false, maxLength: 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.AbpFeatures",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 128),
                    Value = c.String(nullable: false, maxLength: 2000),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                    EditionId = c.Int(),
                    TenantId = c.Int(),
                    Discriminator = c.String(nullable: false, maxLength: 128),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_TenantFeatureSetting_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.AbpEditions", t => t.EditionId, cascadeDelete: true)
                .Index(t => t.EditionId);

            CreateTable(
                "TicoPay.AbpLanguages",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    TenantId = c.Int(),
                    Name = c.String(nullable: false, maxLength: 10),
                    DisplayName = c.String(nullable: false, maxLength: 64),
                    Icon = c.String(maxLength: 128),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ApplicationLanguage_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ApplicationLanguage_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.AbpLanguageTexts",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    LanguageName = c.String(nullable: false, maxLength: 10),
                    Source = c.String(nullable: false, maxLength: 128),
                    Key = c.String(nullable: false, maxLength: 256),
                    Value = c.String(nullable: false),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ApplicationLanguageText_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.Monedas",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    NombrePais = c.String(nullable: false, maxLength: 70),
                    NombreMoneda = c.String(nullable: false, maxLength: 70),
                    codigoMoneda = c.String(nullable: false, maxLength: 3),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.AbpNotifications",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    NotificationName = c.String(nullable: false, maxLength: 96),
                    Data = c.String(),
                    DataTypeName = c.String(maxLength: 512),
                    EntityTypeName = c.String(maxLength: 250),
                    EntityTypeAssemblyQualifiedName = c.String(maxLength: 512),
                    EntityId = c.String(maxLength: 96),
                    Severity = c.Byte(nullable: false),
                    UserIds = c.String(),
                    ExcludedUserIds = c.String(),
                    TenantIds = c.String(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.AbpNotificationSubscriptions",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(),
                    UserId = c.Long(nullable: false),
                    NotificationName = c.String(maxLength: 96),
                    EntityTypeName = c.String(maxLength: 250),
                    EntityTypeAssemblyQualifiedName = c.String(maxLength: 512),
                    EntityId = c.String(maxLength: 96),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_NotificationSubscriptionInfo_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.NotificationName, t.EntityTypeName, t.EntityId, t.UserId });

            CreateTable(
                "TicoPay.AbpOrganizationUnits",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    ParentId = c.Long(),
                    Code = c.String(nullable: false, maxLength: 128),
                    DisplayName = c.String(nullable: false, maxLength: 128),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_OrganizationUnit_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_OrganizationUnit_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.AbpOrganizationUnits", t => t.ParentId)
                .Index(t => t.ParentId);

            CreateTable(
                "TicoPay.Outlets",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Name = c.String(),
                    DefaultTaxId = c.Guid(),
                    OrderNumberPrefix = c.String(),
                    SupplierReturnNumberPrefix = c.String(),
                    SupplierReturnNumberId = c.Guid(nullable: false),
                    SupplierReturnNumber = c.Guid(nullable: false),
                    PhoneNumber = c.String(),
                    Address = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Outlet_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Outlet_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Taxes", t => t.DefaultTaxId)
                .Index(t => t.DefaultTaxId);

            CreateTable(
                "TicoPay.PaymentMethods",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Name = c.String(maxLength: 50),
                    GatewayUrl = c.String(),
                    Code = c.String(maxLength: 2),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_PaymentMethod_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_PaymentMethod_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.RecordHistories",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    ReferenceToRecordId = c.Guid(nullable: false),
                    ReferenceToTableName = c.String(),
                    UserId = c.Guid(),
                    UserName = c.String(),
                    Note = c.String(),
                    ChangeType = c.Int(nullable: false),
                    OldValue = c.String(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_RecordHistory_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.AbpRoles",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DisplayName = c.String(nullable: false, maxLength: 64),
                    IsStatic = c.Boolean(nullable: false),
                    IsDefault = c.Boolean(nullable: false),
                    TenantId = c.Int(),
                    Name = c.String(nullable: false, maxLength: 32),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Role_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Role_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.AbpUsers", t => t.CreatorUserId)
                .ForeignKey("TicoPay.AbpUsers", t => t.DeleterUserId)
                .ForeignKey("TicoPay.AbpUsers", t => t.LastModifierUserId)
                .Index(t => t.DeleterUserId)
                .Index(t => t.LastModifierUserId)
                .Index(t => t.CreatorUserId);

            CreateTable(
                "TicoPay.AbpTenantNotifications",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(),
                    NotificationName = c.String(nullable: false, maxLength: 96),
                    Data = c.String(),
                    DataTypeName = c.String(maxLength: 512),
                    EntityTypeName = c.String(maxLength: 250),
                    EntityTypeAssemblyQualifiedName = c.String(maxLength: 512),
                    EntityId = c.String(maxLength: 96),
                    Severity = c.Byte(nullable: false),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_TenantNotificationInfo_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.AbpUserAccounts",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    UserId = c.Long(nullable: false),
                    UserLinkId = c.Long(),
                    UserName = c.String(),
                    EmailAddress = c.String(),
                    LastLoginTime = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserAccount_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.AbpUserLoginAttempts",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    TenancyName = c.String(maxLength: 64),
                    UserId = c.Long(),
                    UserNameOrEmailAddress = c.String(maxLength: 255),
                    ClientIpAddress = c.String(maxLength: 64),
                    ClientName = c.String(maxLength: 128),
                    BrowserInfo = c.String(maxLength: 256),
                    Result = c.Byte(nullable: false),
                    CreationTime = c.DateTime(nullable: false),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserLoginAttempt_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.UserId, t.TenantId })
                .Index(t => new { t.TenancyName, t.UserNameOrEmailAddress, t.Result });

            CreateTable(
                "TicoPay.AbpUserNotifications",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(),
                    UserId = c.Long(nullable: false),
                    TenantNotificationId = c.Guid(nullable: false),
                    State = c.Int(nullable: false),
                    CreationTime = c.DateTime(nullable: false),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserNotificationInfo_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.UserId, t.State, t.CreationTime });

            CreateTable(
                "TicoPay.AbpUserOrganizationUnits",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    TenantId = c.Int(),
                    UserId = c.Long(nullable: false),
                    OrganizationUnitId = c.Long(nullable: false),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserOrganizationUnit_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "TicoPay.GroupServices",
                c => new
                {
                    ServiceId = c.Guid(nullable: false),
                    GroupConceptsId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => new { t.ServiceId, t.GroupConceptsId })
                .ForeignKey("TicoPay.Services", t => t.ServiceId, cascadeDelete: true)
                .ForeignKey("TicoPay.GroupConcepts", t => t.GroupConceptsId, cascadeDelete: true)
                .Index(t => t.ServiceId)
                .Index(t => t.GroupConceptsId);

            CreateTable(
                "TicoPay.ClientGroupConcept",
                c => new
                {
                    ServiceId = c.Guid(nullable: false),
                    GroupConceptsId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => new { t.ServiceId, t.GroupConceptsId })
                .ForeignKey("TicoPay.Client", t => t.ServiceId, cascadeDelete: true)
                .ForeignKey("TicoPay.GroupConcepts", t => t.GroupConceptsId, cascadeDelete: true)
                .Index(t => t.ServiceId)
                .Index(t => t.GroupConceptsId);

        }

        public override void Down()
        {
            DropForeignKey("TicoPay.AbpPermissions", "RoleId", "TicoPay.AbpRoles");
            DropForeignKey("TicoPay.AbpRoles", "LastModifierUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpRoles", "DeleterUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpRoles", "CreatorUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.Outlets", "DefaultTaxId", "TicoPay.Taxes");
            DropForeignKey("TicoPay.AbpOrganizationUnits", "ParentId", "TicoPay.AbpOrganizationUnits");
            DropForeignKey("TicoPay.AbpFeatures", "EditionId", "TicoPay.AbpEditions");
            DropForeignKey("TicoPay.Cantons", "ProvinciaID", "TicoPay.Provincias");
            DropForeignKey("TicoPay.Distritoes", "CantonID", "TicoPay.Cantons");
            DropForeignKey("TicoPay.Barrios", "DistritoID", "TicoPay.Distritoes");
            DropForeignKey("TicoPay.ClientGroups", "GroupId", "TicoPay.Groups");
            DropForeignKey("TicoPay.ClientGroups", "ClientId", "TicoPay.Client");
            DropForeignKey("TicoPay.ClientGroupConcept", "GroupConceptsId", "TicoPay.GroupConcepts");
            DropForeignKey("TicoPay.ClientGroupConcept", "ServiceId", "TicoPay.Client");
            DropForeignKey("TicoPay.GroupServices", "GroupConceptsId", "TicoPay.GroupConcepts");
            DropForeignKey("TicoPay.GroupServices", "ServiceId", "TicoPay.Services");
            DropForeignKey("TicoPay.ClientServices", "ServiceId", "TicoPay.Services");
            DropForeignKey("TicoPay.Invoices", "ClientService_Id", "TicoPay.ClientServices");
            DropForeignKey("TicoPay.AbpTenants", "LastModifierUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.Invoices", "TenantId", "TicoPay.AbpTenants");
            DropForeignKey("TicoPay.AbpTenants", "EditionId", "TicoPay.AbpEditions");
            DropForeignKey("TicoPay.AbpTenants", "DeleterUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpTenants", "CreatorUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpSettings", "UserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpUserRoles", "UserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpPermissions", "UserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpUserLogins", "UserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpUsers", "LastModifierUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpUsers", "DeleterUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpUsers", "CreatorUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpTenants", "CountryID", "TicoPay.Countries");
            DropForeignKey("TicoPay.AbpTenants", "BarrioId", "TicoPay.Barrios");
            DropForeignKey("TicoPay.Invoices", "RegisterId", "TicoPay.Registers");
            DropForeignKey("TicoPay.Notes", "InvoiceId", "TicoPay.Invoices");
            DropForeignKey("TicoPay.Notes", "ExchangeRateId", "TicoPay.ExchangeRates");
            DropForeignKey("TicoPay.PaymentInvoices", "InvoiceId", "TicoPay.Invoices");
            DropForeignKey("TicoPay.PaymentInvoices", "ExchangeRateId", "TicoPay.ExchangeRates");
            DropForeignKey("TicoPay.InvoiceLines", "ServiceId", "TicoPay.Services");
            DropForeignKey("TicoPay.InvoiceLines", "ProductId", "TicoPay.Products");
            DropForeignKey("TicoPay.ProductVariants", "Product_Id", "TicoPay.Products");
            DropForeignKey("TicoPay.ProductVariants", "InvoiceId", "TicoPay.Invoices");
            DropForeignKey("TicoPay.Products", "TaxId", "TicoPay.Taxes");
            DropForeignKey("TicoPay.Services", "TaxId", "TicoPay.Taxes");
            DropForeignKey("TicoPay.ProductTags", "TagId", "TicoPay.Tags");
            DropForeignKey("TicoPay.ProductTags", "ProductId", "TicoPay.Products");
            DropForeignKey("TicoPay.Products", "SupplierId", "TicoPay.Suppliers");
            DropForeignKey("TicoPay.Products", "ProductTypeId", "TicoPay.ProductTypes");
            DropForeignKey("TicoPay.Products", "BrandId", "TicoPay.Brands");
            DropForeignKey("TicoPay.InvoiceLines", "InvoiceId", "TicoPay.Invoices");
            DropForeignKey("TicoPay.InvoiceLines", "ExonerationId", "TicoPay.Exonerations");
            DropForeignKey("TicoPay.InvoiceHistoryStatus", "InvoiceId", "TicoPay.Invoices");
            DropForeignKey("TicoPay.Invoices", "ClientId", "TicoPay.Client");
            DropForeignKey("TicoPay.ClientServices", "ClientId", "TicoPay.Client");
            DropForeignKey("TicoPay.Client", "BarrioId", "TicoPay.Barrios");
            DropIndex("TicoPay.ClientGroupConcept", new[] { "GroupConceptsId" });
            DropIndex("TicoPay.ClientGroupConcept", new[] { "ServiceId" });
            DropIndex("TicoPay.GroupServices", new[] { "GroupConceptsId" });
            DropIndex("TicoPay.GroupServices", new[] { "ServiceId" });
            DropIndex("TicoPay.AbpUserNotifications", new[] { "UserId", "State", "CreationTime" });
            DropIndex("TicoPay.AbpUserLoginAttempts", new[] { "TenancyName", "UserNameOrEmailAddress", "Result" });
            DropIndex("TicoPay.AbpUserLoginAttempts", new[] { "UserId", "TenantId" });
            DropIndex("TicoPay.AbpRoles", new[] { "CreatorUserId" });
            DropIndex("TicoPay.AbpRoles", new[] { "LastModifierUserId" });
            DropIndex("TicoPay.AbpRoles", new[] { "DeleterUserId" });
            DropIndex("TicoPay.Outlets", new[] { "DefaultTaxId" });
            DropIndex("TicoPay.AbpOrganizationUnits", new[] { "ParentId" });
            DropIndex("TicoPay.AbpNotificationSubscriptions", new[] { "NotificationName", "EntityTypeName", "EntityId", "UserId" });
            DropIndex("TicoPay.AbpFeatures", new[] { "EditionId" });
            DropIndex("TicoPay.Cantons", new[] { "ProvinciaID" });
            DropIndex("TicoPay.Distritoes", new[] { "CantonID" });
            DropIndex("TicoPay.ClientGroups", new[] { "ClientId" });
            DropIndex("TicoPay.ClientGroups", new[] { "GroupId" });
            DropIndex("TicoPay.AbpSettings", new[] { "UserId" });
            DropIndex("TicoPay.AbpUserRoles", new[] { "UserId" });
            DropIndex("TicoPay.AbpPermissions", new[] { "RoleId" });
            DropIndex("TicoPay.AbpPermissions", new[] { "UserId" });
            DropIndex("TicoPay.AbpUserLogins", new[] { "UserId" });
            DropIndex("TicoPay.AbpUsers", new[] { "CreatorUserId" });
            DropIndex("TicoPay.AbpUsers", new[] { "LastModifierUserId" });
            DropIndex("TicoPay.AbpUsers", new[] { "DeleterUserId" });
            DropIndex("TicoPay.AbpTenants", new[] { "CreatorUserId" });
            DropIndex("TicoPay.AbpTenants", new[] { "LastModifierUserId" });
            DropIndex("TicoPay.AbpTenants", new[] { "DeleterUserId" });
            DropIndex("TicoPay.AbpTenants", new[] { "EditionId" });
            DropIndex("TicoPay.AbpTenants", new[] { "BarrioId" });
            DropIndex("TicoPay.AbpTenants", new[] { "CountryID" });
            DropIndex("TicoPay.Notes", new[] { "ExchangeRateId" });
            DropIndex("TicoPay.Notes", new[] { "InvoiceId" });
            DropIndex("TicoPay.PaymentInvoices", new[] { "ExchangeRateId" });
            DropIndex("TicoPay.PaymentInvoices", new[] { "InvoiceId" });
            DropIndex("TicoPay.ProductVariants", new[] { "Product_Id" });
            DropIndex("TicoPay.ProductVariants", new[] { "InvoiceId" });
            DropIndex("TicoPay.ProductTags", new[] { "TagId" });
            DropIndex("TicoPay.ProductTags", new[] { "ProductId" });
            DropIndex("TicoPay.Products", new[] { "ProductTypeId" });
            DropIndex("TicoPay.Products", new[] { "SupplierId" });
            DropIndex("TicoPay.Products", new[] { "BrandId" });
            DropIndex("TicoPay.Products", new[] { "TaxId" });
            DropIndex("TicoPay.InvoiceLines", new[] { "ExonerationId" });
            DropIndex("TicoPay.InvoiceLines", new[] { "ProductId" });
            DropIndex("TicoPay.InvoiceLines", new[] { "ServiceId" });
            DropIndex("TicoPay.InvoiceLines", new[] { "InvoiceId" });
            DropIndex("TicoPay.InvoiceHistoryStatus", new[] { "InvoiceId" });
            DropIndex("TicoPay.Invoices", new[] { "ClientService_Id" });
            DropIndex("TicoPay.Invoices", new[] { "RegisterId" });
            DropIndex("TicoPay.Invoices", new[] { "ClientId" });
            DropIndex("TicoPay.Invoices", new[] { "Status" });
            DropIndex("TicoPay.Invoices", new[] { "DueDate" });
            DropIndex("TicoPay.Invoices", new[] { "Balance" });
            DropIndex("TicoPay.Invoices", new[] { "Number" });
            DropIndex("TicoPay.Invoices", new[] { "TenantId" });
            DropIndex("TicoPay.ClientServices", new[] { "ClientId" });
            DropIndex("TicoPay.ClientServices", new[] { "ServiceId" });
            DropIndex("TicoPay.Services", new[] { "TaxId" });
            DropIndex("TicoPay.Client", new[] { "BarrioId" });
            DropIndex("TicoPay.Client", new[] { "Code" });
            DropIndex("TicoPay.Barrios", new[] { "DistritoID" });
            DropIndex("TicoPay.AbpBackgroundJobs", new[] { "IsAbandoned", "NextTryTime" });
            DropTable("TicoPay.ClientGroupConcept");
            DropTable("TicoPay.GroupServices");
            DropTable("TicoPay.AbpUserOrganizationUnits",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserOrganizationUnit_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpUserNotifications",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserNotificationInfo_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpUserLoginAttempts",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserLoginAttempt_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpUserAccounts",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserAccount_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpTenantNotifications",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_TenantNotificationInfo_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpRoles",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Role_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Role_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.RecordHistories",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_RecordHistory_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.PaymentMethods",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_PaymentMethod_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_PaymentMethod_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Outlets",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Outlet_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Outlet_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpOrganizationUnits",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_OrganizationUnit_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_OrganizationUnit_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpNotificationSubscriptions",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_NotificationSubscriptionInfo_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpNotifications");
            DropTable("TicoPay.Monedas");
            DropTable("TicoPay.AbpLanguageTexts",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ApplicationLanguageText_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpLanguages",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ApplicationLanguage_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ApplicationLanguage_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpFeatures",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_TenantFeatureSetting_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Provincias");
            DropTable("TicoPay.Cantons");
            DropTable("TicoPay.Distritoes");
            DropTable("TicoPay.Groups",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Group_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Group_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.ClientGroups",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ClientGroup_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ClientGroup_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpEditions",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Edition_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpSettings",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Setting_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpUserRoles",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserRole_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpPermissions",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_PermissionSetting_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_RolePermissionSetting_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_UserPermissionSetting_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpUserLogins",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_UserLogin_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpUsers",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_User_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_User_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Countries");
            DropTable("TicoPay.AbpTenants",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Tenant_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Registers",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Register_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Register_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Notes",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Note_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Note_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.ExchangeRates",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ExchangeRate_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ExchangeRate_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.PaymentInvoices",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_PaymentInvoice_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_PaymentInvoice_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.ProductVariants",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ProductVariant_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ProductVariant_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Taxes",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Tax_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Tax_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Tags",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Tag_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Tag_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.ProductTags",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ProductTag_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ProductTag_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Suppliers",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Supplier_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Supplier_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.ProductTypes",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ProductType_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ProductType_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Brands",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Brand_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Brand_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Products",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Product_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Product_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Exonerations",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Exoneration_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Exoneration_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.InvoiceLines",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_InvoiceLine_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_InvoiceLine_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.InvoiceHistoryStatus",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_InvoiceHistoryStatus_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_InvoiceHistoryStatus_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Invoices",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Invoice_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Invoice_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.ClientServices",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ClientService_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ClientService_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Services",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Service_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Service_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.GroupConcepts",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_GroupConcepts_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_GroupConcepts_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Client",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Client_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Client_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Barrios");
            DropTable("TicoPay.AbpBackgroundJobs");
            DropTable("TicoPay.AbpAuditLogs",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_AuditLog_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AgreementConectivities");
            DropTable("TicoPay.Addresses",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Address_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Address_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
