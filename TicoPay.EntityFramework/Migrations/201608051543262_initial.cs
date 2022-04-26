namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
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
                        Name = c.String(),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                "TicoPay.Invoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TenantId = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        Alphanumeric = c.String(maxLength: 50),
                        Note = c.String(maxLength: 500),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountPercentaje = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DueDate = c.DateTime(nullable: false),
                        PaymentDate = c.DateTime(),
                        Status = c.Int(nullable: false),
                        ClientId = c.Guid(),
                        RegisterId = c.Guid(),
                        UserId = c.Guid(),
                        UserName = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        CurrencyCode = c.String(maxLength: 3),
                        PaymetnMethodType = c.Int(nullable: false),
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
                .ForeignKey("TicoPay.Clients", t => t.ClientId)
                .ForeignKey("TicoPay.ClientServices", t => t.ClientService_Id)
                .ForeignKey("TicoPay.Registers", t => t.RegisterId)
                .Index(t => t.ClientId)
                .Index(t => t.RegisterId)
                .Index(t => t.ClientService_Id);
            
            CreateTable(
                "TicoPay.Clients",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 128),
                        IdentificationType = c.Int(nullable: false),
                        Identification = c.String(nullable: false, maxLength: 30),
                        Birthday = c.DateTime(),
                        Code = c.String(maxLength: 10),
                        Gender = c.Int(nullable: false),
                        PhoneNumber = c.String(maxLength: 20),
                        MobilNumber = c.String(maxLength: 20),
                        Fax = c.String(maxLength: 20),
                        Email = c.String(maxLength: 200),
                        WebSite = c.String(maxLength: 1000),
                        Street = c.String(maxLength: 150),
                        City = c.String(maxLength: 150),
                        Region = c.String(maxLength: 150),
                        PostalCode = c.String(maxLength: 15),
                        IsoCountry = c.String(maxLength: 3),
                        Address = c.String(maxLength: 1000),
                        Latitud = c.Long(),
                        Longitud = c.Long(),
                        ContactName = c.String(maxLength: 128),
                        ContactMobilNumber = c.String(maxLength: 20),
                        ContactPhoneNumber = c.String(maxLength: 20),
                        ContactEmail = c.String(maxLength: 200),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HasImage = c.Boolean(nullable: false),
                        ImageUrl = c.String(maxLength: 1000),
                        Note = c.String(maxLength: 2048),
                        AllowEmailNotifications = c.Boolean(nullable: false),
                        AllowSmsNotifications = c.Boolean(nullable: false),
                        State = c.Int(nullable: false),
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
                .ForeignKey("TicoPay.Clients", t => t.ClientId, cascadeDelete: true)
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
                "TicoPay.ClientServices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ServiceId = c.Guid(nullable: false),
                        ClientId = c.Guid(nullable: false),
                        Code = c.String(),
                        InitDate = c.DateTime(nullable: false),
                        AllowLatePayment = c.Boolean(nullable: false),
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
                .ForeignKey("TicoPay.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("TicoPay.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "TicoPay.Services",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CronExpression = c.String(),
                        Name = c.String(maxLength: 128),
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
                        PricePerUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Note = c.String(maxLength: 200),
                        Title = c.String(maxLength: 50),
                        Quantity = c.Int(nullable: false),
                        InvoiceId = c.Guid(nullable: false),
                        LineType = c.Int(nullable: false),
                        ServiceId = c.Guid(),
                        ProductId = c.Guid(),
                        CurrencyCode = c.String(maxLength: 3),
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
                    { "DynamicFilter_InvoiceLine_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_InvoiceLine_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("TicoPay.Products", t => t.ProductId)
                .ForeignKey("TicoPay.Services", t => t.ServiceId)
                .Index(t => t.InvoiceId)
                .Index(t => t.ServiceId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "TicoPay.PaymentInvoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TenantId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceId = c.Guid(nullable: false),
                        ExchangeRateId = c.Guid(nullable: false),
                        PaymentMethodId = c.Guid(nullable: false),
                        PaymentInvoiceType = c.Int(nullable: false),
                        Reference = c.String(),
                        CurrencyCode = c.String(),
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
                .ForeignKey("TicoPay.ExchangeRates", t => t.ExchangeRateId, cascadeDelete: true)
                .ForeignKey("TicoPay.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("TicoPay.PaymentMethods", t => t.PaymentMethodId, cascadeDelete: true)
                .Index(t => t.InvoiceId)
                .Index(t => t.ExchangeRateId)
                .Index(t => t.PaymentMethodId);
            
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
                "TicoPay.PaymentMethods",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TenantId = c.Int(nullable: false),
                        Name = c.String(),
                        GatewayUrl = c.String(),
                        Code = c.String(),
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
                "TicoPay.Notes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TenantId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceId = c.Guid(nullable: false),
                        ExchangeRateId = c.Guid(),
                        Reference = c.String(),
                        Description = c.String(),
                        CurrencyCode = c.String(maxLength: 3),
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
                        InvoiceNumber = c.Int(nullable: false),
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
                "TicoPay.AbpPermissions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TenantId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 128),
                        IsGranted = c.Boolean(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                        RoleId = c.Int(),
                        UserId = c.Long(),
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
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
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
                "TicoPay.AbpTenants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
                .ForeignKey("TicoPay.AbpUsers", t => t.CreatorUserId)
                .ForeignKey("TicoPay.AbpUsers", t => t.DeleterUserId)
                .ForeignKey("TicoPay.AbpEditions", t => t.EditionId)
                .ForeignKey("TicoPay.AbpUsers", t => t.LastModifierUserId)
                .Index(t => t.EditionId)
                .Index(t => t.DeleterUserId)
                .Index(t => t.LastModifierUserId)
                .Index(t => t.CreatorUserId);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("TicoPay.AbpTenants", "LastModifierUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpTenants", "EditionId", "TicoPay.AbpEditions");
            DropForeignKey("TicoPay.AbpTenants", "DeleterUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpTenants", "CreatorUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpPermissions", "RoleId", "TicoPay.AbpRoles");
            DropForeignKey("TicoPay.AbpRoles", "LastModifierUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpRoles", "DeleterUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpRoles", "CreatorUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpSettings", "UserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpUserRoles", "UserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpPermissions", "UserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpUserLogins", "UserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpUsers", "LastModifierUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpUsers", "DeleterUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.AbpUsers", "CreatorUserId", "TicoPay.AbpUsers");
            DropForeignKey("TicoPay.Outlets", "DefaultTaxId", "TicoPay.Taxes");
            DropForeignKey("TicoPay.AbpOrganizationUnits", "ParentId", "TicoPay.AbpOrganizationUnits");
            DropForeignKey("TicoPay.AbpFeatures", "EditionId", "TicoPay.AbpEditions");
            DropForeignKey("TicoPay.ProductVariants", "Product_Id", "TicoPay.Products");
            DropForeignKey("TicoPay.ProductVariants", "InvoiceId", "TicoPay.Invoices");
            DropForeignKey("TicoPay.Invoices", "RegisterId", "TicoPay.Registers");
            DropForeignKey("TicoPay.Notes", "InvoiceId", "TicoPay.Invoices");
            DropForeignKey("TicoPay.Notes", "ExchangeRateId", "TicoPay.ExchangeRates");
            DropForeignKey("TicoPay.PaymentInvoices", "PaymentMethodId", "TicoPay.PaymentMethods");
            DropForeignKey("TicoPay.PaymentInvoices", "InvoiceId", "TicoPay.Invoices");
            DropForeignKey("TicoPay.PaymentInvoices", "ExchangeRateId", "TicoPay.ExchangeRates");
            DropForeignKey("TicoPay.InvoiceLines", "ServiceId", "TicoPay.Services");
            DropForeignKey("TicoPay.InvoiceLines", "ProductId", "TicoPay.Products");
            DropForeignKey("TicoPay.InvoiceLines", "InvoiceId", "TicoPay.Invoices");
            DropForeignKey("TicoPay.InvoiceHistoryStatus", "InvoiceId", "TicoPay.Invoices");
            DropForeignKey("TicoPay.Services", "TaxId", "TicoPay.Taxes");
            DropForeignKey("TicoPay.ClientServices", "ServiceId", "TicoPay.Services");
            DropForeignKey("TicoPay.Invoices", "ClientService_Id", "TicoPay.ClientServices");
            DropForeignKey("TicoPay.ClientServices", "ClientId", "TicoPay.Clients");
            DropForeignKey("TicoPay.Invoices", "ClientId", "TicoPay.Clients");
            DropForeignKey("TicoPay.ClientGroups", "GroupId", "TicoPay.Groups");
            DropForeignKey("TicoPay.ClientGroups", "ClientId", "TicoPay.Clients");
            DropForeignKey("TicoPay.Products", "TaxId", "TicoPay.Taxes");
            DropForeignKey("TicoPay.ProductTags", "TagId", "TicoPay.Tags");
            DropForeignKey("TicoPay.ProductTags", "ProductId", "TicoPay.Products");
            DropForeignKey("TicoPay.Products", "SupplierId", "TicoPay.Suppliers");
            DropForeignKey("TicoPay.Products", "ProductTypeId", "TicoPay.ProductTypes");
            DropForeignKey("TicoPay.Products", "BrandId", "TicoPay.Brands");
            DropIndex("TicoPay.AbpUserNotifications", new[] { "UserId", "State", "CreationTime" });
            DropIndex("TicoPay.AbpUserLoginAttempts", new[] { "TenancyName", "UserNameOrEmailAddress", "Result" });
            DropIndex("TicoPay.AbpUserLoginAttempts", new[] { "UserId", "TenantId" });
            DropIndex("TicoPay.AbpTenants", new[] { "CreatorUserId" });
            DropIndex("TicoPay.AbpTenants", new[] { "LastModifierUserId" });
            DropIndex("TicoPay.AbpTenants", new[] { "DeleterUserId" });
            DropIndex("TicoPay.AbpTenants", new[] { "EditionId" });
            DropIndex("TicoPay.AbpSettings", new[] { "UserId" });
            DropIndex("TicoPay.AbpUserRoles", new[] { "UserId" });
            DropIndex("TicoPay.AbpUserLogins", new[] { "UserId" });
            DropIndex("TicoPay.AbpUsers", new[] { "CreatorUserId" });
            DropIndex("TicoPay.AbpUsers", new[] { "LastModifierUserId" });
            DropIndex("TicoPay.AbpUsers", new[] { "DeleterUserId" });
            DropIndex("TicoPay.AbpRoles", new[] { "CreatorUserId" });
            DropIndex("TicoPay.AbpRoles", new[] { "LastModifierUserId" });
            DropIndex("TicoPay.AbpRoles", new[] { "DeleterUserId" });
            DropIndex("TicoPay.AbpPermissions", new[] { "UserId" });
            DropIndex("TicoPay.AbpPermissions", new[] { "RoleId" });
            DropIndex("TicoPay.Outlets", new[] { "DefaultTaxId" });
            DropIndex("TicoPay.AbpOrganizationUnits", new[] { "ParentId" });
            DropIndex("TicoPay.AbpNotificationSubscriptions", new[] { "NotificationName", "EntityTypeName", "EntityId", "UserId" });
            DropIndex("TicoPay.AbpFeatures", new[] { "EditionId" });
            DropIndex("TicoPay.Notes", new[] { "ExchangeRateId" });
            DropIndex("TicoPay.Notes", new[] { "InvoiceId" });
            DropIndex("TicoPay.PaymentInvoices", new[] { "PaymentMethodId" });
            DropIndex("TicoPay.PaymentInvoices", new[] { "ExchangeRateId" });
            DropIndex("TicoPay.PaymentInvoices", new[] { "InvoiceId" });
            DropIndex("TicoPay.InvoiceLines", new[] { "ProductId" });
            DropIndex("TicoPay.InvoiceLines", new[] { "ServiceId" });
            DropIndex("TicoPay.InvoiceLines", new[] { "InvoiceId" });
            DropIndex("TicoPay.InvoiceHistoryStatus", new[] { "InvoiceId" });
            DropIndex("TicoPay.Services", new[] { "TaxId" });
            DropIndex("TicoPay.ClientServices", new[] { "ClientId" });
            DropIndex("TicoPay.ClientServices", new[] { "ServiceId" });
            DropIndex("TicoPay.ClientGroups", new[] { "ClientId" });
            DropIndex("TicoPay.ClientGroups", new[] { "GroupId" });
            DropIndex("TicoPay.Invoices", new[] { "ClientService_Id" });
            DropIndex("TicoPay.Invoices", new[] { "RegisterId" });
            DropIndex("TicoPay.Invoices", new[] { "ClientId" });
            DropIndex("TicoPay.ProductVariants", new[] { "Product_Id" });
            DropIndex("TicoPay.ProductVariants", new[] { "InvoiceId" });
            DropIndex("TicoPay.ProductTags", new[] { "TagId" });
            DropIndex("TicoPay.ProductTags", new[] { "ProductId" });
            DropIndex("TicoPay.Products", new[] { "ProductTypeId" });
            DropIndex("TicoPay.Products", new[] { "SupplierId" });
            DropIndex("TicoPay.Products", new[] { "BrandId" });
            DropIndex("TicoPay.Products", new[] { "TaxId" });
            DropIndex("TicoPay.AbpBackgroundJobs", new[] { "IsAbandoned", "NextTryTime" });
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
            DropTable("TicoPay.AbpTenants",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Tenant_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpTenantNotifications",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_TenantNotificationInfo_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
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
            DropTable("TicoPay.AbpPermissions",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_PermissionSetting_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_RolePermissionSetting_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_UserPermissionSetting_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
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
            DropTable("TicoPay.AbpEditions",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Edition_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpFeatures",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_TenantFeatureSetting_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
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
            DropTable("TicoPay.PaymentMethods",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_PaymentMethod_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_PaymentMethod_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
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
            DropTable("TicoPay.Services",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Service_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Service_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.ClientServices",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_ClientService_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_ClientService_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
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
            DropTable("TicoPay.Clients",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Client_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Client_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Invoices",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Invoice_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Invoice_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
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
            DropTable("TicoPay.Products",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Product_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Product_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Brands",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Brand_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Brand_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.AbpBackgroundJobs");
            DropTable("TicoPay.AbpAuditLogs",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_AuditLog_MayHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Addresses",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Address_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Address_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
