namespace TicoPay.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class paymentsNote : DbMigration
    {

        public override void Up()
        {
            DropForeignKey("TicoPay.PaymentInvoices", "ExchangeRateId", "TicoPay.ExchangeRates");
            DropIndex("TicoPay.PaymentInvoices", new[] { "ExchangeRateId" });
            CreateTable(
                "TicoPay.Payments",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    ClientId = c.Guid(nullable: false),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PaymentDate = c.DateTime(nullable: false),
                    ExchangeRateId = c.Guid(),
                    CodigoMoneda = c.Int(nullable: false),
                    PaymentType = c.Int(nullable: false),
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
                    IsPaymentReversed = c.Boolean(),
                    IsPaymentUsed = c.Boolean(),
                    ParentPaymentInvoiceId = c.Guid(),
                    DeletionTime = c.DateTime(),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Payment_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Payment_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Client", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("TicoPay.ExchangeRates", t => t.ExchangeRateId)
                .Index(t => t.ClientId)
                .Index(t => t.ExchangeRateId);

            CreateTable(
                "TicoPay.PaymentNotes",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TenantId = c.Int(nullable: false),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PaymetnMethodType = c.Int(nullable: false),
                    NoteId = c.Guid(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    DeleterUserId = c.Long(),
                    IsPaymentReversed = c.Boolean(),
                    DeletionTime = c.DateTime(),
                    PaymentId = c.Guid(nullable: false),
                    LastModificationTime = c.DateTime(),
                    LastModifierUserId = c.Long(),
                    CreationTime = c.DateTime(nullable: false),
                    CreatorUserId = c.Long(),
                },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_PaymentNote_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_PaymentNote_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TicoPay.Notes", t => t.NoteId, cascadeDelete: true)
                .ForeignKey("TicoPay.Payments", t => t.PaymentId, cascadeDelete: true)
                .Index(t => t.NoteId)
                .Index(t => t.PaymentId);

            Sql(@"insert into [TicoPay].[Payments] ([Id] ,[TenantId],[ClientId], [Amount], [PaymentDate], [ExchangeRateId], 
                            [CodigoMoneda], [PaymentType], [PaymetnMethodType], [Transaction], [Reference], [CodigoBanco],
                            [CodigoAgencia], [CodigoTransaccion], [ConsecutivoTransaccion], [NotaCredito], [CodigoBancoEmisor], 
                            [NumeroCuenta], [NumeroCheque], [IsDeleted], [DeleterUserId], [DeletionTime], [LastModificationTime], 
                            [LastModifierUserId], [CreationTime], [CreatorUserId], [IsPaymentReversed], [IsPaymentUsed], [ParentPaymentInvoiceId],
                            [Balance])
                  select NEWID(), p.[TenantId],i.ClientId, [Amount], [PaymentDate], [ExchangeRateId], p.[CodigoMoneda], [PaymentInvoiceType],
                        [PaymetnMethodType], [Transaction], [Reference], [CodigoBanco], [CodigoAgencia], [CodigoTransaccion],
                        [ConsecutivoTransaccion], [NotaCredito], [CodigoBancoEmisor], [NumeroCuenta], [NumeroCheque], p.[IsDeleted],
                        p.[DeleterUserId], p.[DeletionTime], p.[LastModificationTime], p.[LastModifierUserId], p.[CreationTime],
                        p.[CreatorUserId], [IsPaymentReversed], [IsPaymentUsed], [ParentPaymentInvoiceId],
                        (case when IsPaymentReversed=1 then Amount else 0 end)
                        from [TicoPay].[PaymentInvoices] p
                        inner join [TicoPay].[Invoices]  i on p.InvoiceId=i.Id");

            AddColumn("TicoPay.PaymentInvoices", "PaymentId", c => c.Guid(nullable: false));
            AddColumn("TicoPay.Notes", "Status", c => c.Int(nullable: false));
            AddColumn("TicoPay.Notes", "DueDate", c => c.DateTime(nullable: false));
            AddColumn("TicoPay.Notes", "CreditTerm", c => c.Int(nullable: false));
            AddColumn("TicoPay.Notes", "ConditionSaleType", c => c.Int(nullable: false));
            AddColumn("TicoPay.Notes", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));

            Sql(@"update [TicoPay].[PaymentInvoices] set PaymentId=b.Id
                        from [TicoPay].[PaymentInvoices] a inner join  [TicoPay].[Payments] b 
                        on a.TenantId=b.TenantId and  ( (a.CreatorUserId=b.CreatorUserId) or ((a.CreatorUserId is null) and (b.CreatorUserId is null)) ) 
                        and a.CreationTime=b.CreationTime
                        and  a.Amount=b.Amount and a.PaymetnMethodType=b.PaymetnMethodType");

            CreateIndex("TicoPay.PaymentInvoices", "PaymentId");
            CreateIndex("TicoPay.Notes", "Status");
            CreateIndex("TicoPay.Notes", "DueDate");



            AddForeignKey("TicoPay.PaymentInvoices", "PaymentId", "TicoPay.Payments", "Id", cascadeDelete: true);
            DropColumn("TicoPay.PaymentInvoices", "PaymentDate");
            DropColumn("TicoPay.PaymentInvoices", "ExchangeRateId");
            DropColumn("TicoPay.PaymentInvoices", "CodigoMoneda");
            DropColumn("TicoPay.PaymentInvoices", "PaymentInvoiceType");
            DropColumn("TicoPay.PaymentInvoices", "Transaction");
            DropColumn("TicoPay.PaymentInvoices", "Reference");
            DropColumn("TicoPay.PaymentInvoices", "CodigoBanco");
            DropColumn("TicoPay.PaymentInvoices", "CodigoAgencia");
            DropColumn("TicoPay.PaymentInvoices", "CodigoTransaccion");
            DropColumn("TicoPay.PaymentInvoices", "ConsecutivoTransaccion");
            DropColumn("TicoPay.PaymentInvoices", "NotaCredito");
            DropColumn("TicoPay.PaymentInvoices", "CodigoBancoEmisor");
            DropColumn("TicoPay.PaymentInvoices", "NumeroCuenta");
            DropColumn("TicoPay.PaymentInvoices", "NumeroCheque");
            DropColumn("TicoPay.PaymentInvoices", "IsPaymentUsed");
            DropColumn("TicoPay.PaymentInvoices", "ParentPaymentInvoiceId");
        }

        public override void Down()
        {
            AddColumn("TicoPay.PaymentInvoices", "ParentPaymentInvoiceId", c => c.Guid());
            AddColumn("TicoPay.PaymentInvoices", "IsPaymentUsed", c => c.Boolean());
            AddColumn("TicoPay.PaymentInvoices", "NumeroCheque", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "NumeroCuenta", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "CodigoBancoEmisor", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "NotaCredito", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "ConsecutivoTransaccion", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "CodigoTransaccion", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "CodigoAgencia", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "CodigoBanco", c => c.Int());
            AddColumn("TicoPay.PaymentInvoices", "Reference", c => c.String());
            AddColumn("TicoPay.PaymentInvoices", "Transaction", c => c.String());
            AddColumn("TicoPay.PaymentInvoices", "PaymentInvoiceType", c => c.Int(nullable: false));
            AddColumn("TicoPay.PaymentInvoices", "CodigoMoneda", c => c.Int(nullable: false));
            AddColumn("TicoPay.PaymentInvoices", "ExchangeRateId", c => c.Guid());
            AddColumn("TicoPay.PaymentInvoices", "PaymentDate", c => c.DateTime(nullable: false));
            Sql(@"update [TicoPay].[PaymentInvoices] set  [TenantId]=b.TenantId, [Amount]=b.[Amount], [PaymentDate]=b.PaymentDate,
                        [ExchangeRateId]= b.ExchangeRateId, [CodigoMoneda]=b.CodigoMoneda, [PaymentInvoiceType]=b.[PaymentType], 
                        [PaymetnMethodType]=b.[PaymetnMethodType], [Transaction]=b.[Transaction], [Reference]=b.[Reference], 
                        [CodigoBanco]=b.[CodigoBanco], [CodigoAgencia]=b.[CodigoAgencia], [CodigoTransaccion]=b.[CodigoTransaccion],
                        [ConsecutivoTransaccion]=b.[ConsecutivoTransaccion], [NotaCredito]=b.[NotaCredito], [CodigoBancoEmisor]=b.[CodigoBancoEmisor],
                        [NumeroCuenta]=b.[NumeroCuenta], [NumeroCheque]=b.[NumeroCheque], [IsDeleted]=b.[IsDeleted], [DeleterUserId]=b.[DeleterUserId], 
                        [DeletionTime]=b.[DeletionTime], [LastModificationTime]=b.[LastModificationTime], [LastModifierUserId]=b.[LastModifierUserId],
                        [CreationTime]=b.[CreationTime], [CreatorUserId]=b.[CreatorUserId], [IsPaymentReversed]=b.[IsPaymentReversed], 
                        [IsPaymentUsed]=b.[IsPaymentUsed], [ParentPaymentInvoiceId]=b.[ParentPaymentInvoiceId]
                        from [TicoPay].[PaymentInvoices] a inner join  [TicoPay].[Payments] b 
                        on a.PaymentId=b.Id");

            DropForeignKey("TicoPay.PaymentNotes", "PaymentId", "TicoPay.Payments");
            DropForeignKey("TicoPay.PaymentNotes", "NoteId", "TicoPay.Notes");
            DropForeignKey("TicoPay.PaymentInvoices", "PaymentId", "TicoPay.Payments");
            DropForeignKey("TicoPay.Payments", "ExchangeRateId", "TicoPay.ExchangeRates");
            DropForeignKey("TicoPay.Payments", "ClientId", "TicoPay.Client");
            DropIndex("TicoPay.Notes", new[] { "DueDate" });
            DropIndex("TicoPay.Notes", new[] { "Status" });
            DropIndex("TicoPay.PaymentNotes", new[] { "PaymentId" });
            DropIndex("TicoPay.PaymentNotes", new[] { "NoteId" });
            DropIndex("TicoPay.Payments", new[] { "ExchangeRateId" });
            DropIndex("TicoPay.Payments", new[] { "ClientId" });
            DropIndex("TicoPay.PaymentInvoices", new[] { "PaymentId" });
            DropColumn("TicoPay.Notes", "Balance");
            DropColumn("TicoPay.Notes", "ConditionSaleType");
            DropColumn("TicoPay.Notes", "CreditTerm");
            DropColumn("TicoPay.Notes", "DueDate");
            DropColumn("TicoPay.Notes", "Status");
            DropColumn("TicoPay.PaymentInvoices", "PaymentId");
            DropTable("TicoPay.PaymentNotes",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_PaymentNote_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_PaymentNote_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            DropTable("TicoPay.Payments",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_Payment_MustHaveTenant", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                    { "DynamicFilter_Payment_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
            CreateIndex("TicoPay.PaymentInvoices", "ExchangeRateId");
            AddForeignKey("TicoPay.PaymentInvoices", "ExchangeRateId", "TicoPay.ExchangeRates", "Id");
        }
    }
}
