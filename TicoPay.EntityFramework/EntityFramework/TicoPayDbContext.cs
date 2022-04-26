using System.Data.Common;
using Abp.Zero.EntityFramework;
using TicoPay.Authorization.Roles;
using TicoPay.MultiTenancy;
using TicoPay.Users;
using System.Data.Entity;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.General;
using TicoPay.Inventory;
using TicoPay.Services;
using TicoPay.Taxes;
using TicoPay.BranchOffices;
using System.Data.Entity.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using Ninject;
using System.Linq;
using System;
using TicoPay.EntityFramework.Configurations;
using TicoPay.ReportsSettings;
using TicoPay.Sellers;
using TicoPay.Vouchers;
using System.Data.Entity.Infrastructure;
using TicoPay.Drawers;
using TicoPay.BranchOffices;

namespace TicoPay.EntityFramework
{
    public class TicoPayDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        public virtual IDbSet<Client> Clients { get; set; }

        public virtual IDbSet<Invoice> Invoices { get; set; }

        public virtual IDbSet<Address> Addresses { get; set; }

        public virtual IDbSet<Brand> Brandes { get; set; }

        public virtual IDbSet<ClientService> ClientServices { get; set; }

        public virtual IDbSet<ExchangeRate> ExchangeRates { get; set; }

        public virtual IDbSet<Group> Groupes { get; set; }

        public virtual IDbSet<InvoiceHistoryStatus> InvoiceHistoryStatuses { get; set; }

        public virtual IDbSet<InvoiceLine> InvoiceLines { get; set; }

        public virtual IDbSet<Note> Notes { get; set; }

        public virtual IDbSet<NoteLine> NoteLines { get; set; }

        public virtual IDbSet<Outlet> Outlets { get; set; }

        public virtual IDbSet<PaymentInvoice> PaymentInvoices { get; set; }

        public virtual IDbSet<Payment> Payments { get; set; }

        public virtual IDbSet<PaymentNote> PaymentNotes { get; set; }

        public virtual IDbSet<PaymentMethod> PaymentMethods { get; set; }

        public virtual IDbSet<Product> Products { get; set; }

        public virtual IDbSet<ProductTag> ProductTags { get; set; }

        public virtual IDbSet<ProductType> ProductTypes { get; set; }

        public virtual IDbSet<ProductVariant> ProductVariants { get; set; }

        public virtual IDbSet<RecordHistory> RecordHistories { get; set; }

        public virtual IDbSet<Register> Registers { get; set; }

        public virtual IDbSet<Service> Services { get; set; }

        public virtual IDbSet<Supplier> Suppliers { get; set; }

        public virtual IDbSet<Tag> Tags { get; set; }

        public virtual IDbSet<Tax> Taxes { get; set; }

        public virtual IDbSet<ClientGroup> ClientGroups { get; set; }

        public virtual IDbSet<ClientGroupConcept> ClientGroupConcepts { get; set; }

        //public virtual IDbSet<Tipos> Tipos { get; set; }

        public virtual IDbSet<Moneda> Monedas { get; set; }

        public virtual IDbSet<Provincia> Provincias { get; set; }

        public virtual IDbSet<Canton> Cantones { get; set; }

        public virtual IDbSet<Distrito> Distritos { get; set; }

        public virtual IDbSet<Barrio> Barrios { get; set; }

        public virtual IDbSet<GroupConcepts> GroupsConcepts { get; set; }

        public virtual IDbSet<Exoneration> Exonerations { get; set; }

        public virtual IDbSet<Country> Countries { get; set; }

        public virtual IDbSet<AgreementConectivity> AgreementsConectivities { get; set; }

        public virtual IDbSet<Certificate> Certificates { get; set; }

        public virtual IDbSet<ReportSettings> ReportSettings { get; set; }

        public virtual IDbSet<Seller> Sellers { get; set; }

        public virtual IDbSet<Voucher> Vouchers { get; set; }

        public virtual IDbSet<UnattendedApi> Unattended { get; set; }

        public virtual IDbSet<BranchOffice> BranchOffices { get; set; }

        public virtual IDbSet<Drawer> Drawers { get; set; }

        public virtual IDbSet<Bank> Banks { get; set; }

        //public virtual IDbSet<UnitMeasurement> UnitsMeasurements { get; set; }

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public TicoPayDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in TicoPayDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of TicoPayDbContext since ABP automatically handles it.
         */
        public TicoPayDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180000;
        }

        //This constructor is used in tests
        public TicoPayDbContext(DbConnection connection)
            : base(connection, true)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("TicoPay");
            modelBuilder.Configurations.Add(new ClientConfiguration(modelBuilder));

            modelBuilder.Entity<Moneda>().Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Certificate>().Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Provincia>().Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Canton>().Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Distrito>().Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Barrio>().Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Country>().Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<InvoiceLine>().Property(c => c.Quantity).HasPrecision(16, 3);
            modelBuilder.Entity<InvoiceLine>().Property(c => c.PricePerUnit).HasPrecision(18, 5);
            modelBuilder.Entity<InvoiceLine>().Property(c => c.Total).HasPrecision(18, 5);
            modelBuilder.Entity<InvoiceLine>().Property(c => c.DiscountPercentage).HasPrecision(18, 5);
            modelBuilder.Entity<InvoiceLine>().Property(c => c.SubTotal).HasPrecision(18, 5);
            modelBuilder.Entity<InvoiceLine>().Property(c => c.TaxAmount).HasPrecision(18, 5);
            modelBuilder.Entity<InvoiceLine>().Property(c => c.LineTotal).HasPrecision(18, 5);
            modelBuilder.Entity<Exoneration>().Property(c => c.TaxAmountExonerated).HasPrecision(18, 5);
            modelBuilder.Entity<Invoice>().Property(c => c.Total).HasPrecision(18, 5);
            modelBuilder.Entity<Invoice>().Property(c => c.SubTotal).HasPrecision(18, 5);
            modelBuilder.Entity<Invoice>().Property(c => c.TotalExento).HasPrecision(18, 5);
            modelBuilder.Entity<Invoice>().Property(c => c.TotalGravado).HasPrecision(18, 5);
            modelBuilder.Entity<Invoice>().Property(c => c.TotalProductExento).HasPrecision(18, 5);
            modelBuilder.Entity<Invoice>().Property(c => c.TotalProductGravado).HasPrecision(18, 5);
            modelBuilder.Entity<Invoice>().Property(c => c.TotalServExento).HasPrecision(18, 5);
            modelBuilder.Entity<Invoice>().Property(c => c.TotalServGravados).HasPrecision(18, 5);
            modelBuilder.Entity<Invoice>().Property(c => c.TotalTax).HasPrecision(18, 5);
            modelBuilder.Entity<Invoice>().Property(c => c.SaleTotal).HasPrecision(18, 5);
            modelBuilder.Entity<Invoice>().Property(c => c.NetaSale).HasPrecision(18, 5);
            modelBuilder.Entity<Invoice>().Property(c => c.DiscountAmount).HasPrecision(18, 5);
            modelBuilder.Entity<Tax>().Property(c => c.Rate).HasPrecision(4, 2);
            modelBuilder.Entity<Tenant>().Property(c => c.CostoSms).HasPrecision(18, 4);
            modelBuilder.Entity<NoteLine>().Property(c => c.Quantity).HasPrecision(16, 3);
            modelBuilder.Entity<NoteLine>().Property(c => c.PricePerUnit).HasPrecision(18, 5);
            modelBuilder.Entity<NoteLine>().Property(c => c.Total).HasPrecision(18, 5);
            modelBuilder.Entity<NoteLine>().Property(c => c.DiscountPercentage).HasPrecision(18, 5);
            modelBuilder.Entity<NoteLine>().Property(c => c.SubTotal).HasPrecision(18, 5);
            modelBuilder.Entity<NoteLine>().Property(c => c.TaxAmount).HasPrecision(18, 5);
            modelBuilder.Entity<NoteLine>().Property(c => c.LineTotal).HasPrecision(18, 5);
            modelBuilder.Entity<Note>().Property(c => c.Amount).HasPrecision(18, 5);
            modelBuilder.Entity<Note>().Property(c => c.TaxAmount).HasPrecision(18, 5);
            modelBuilder.Entity<Note>().Property(c => c.Total).HasPrecision(18, 5);
            modelBuilder.Entity<Note>().Property(c => c.TotalExento).HasPrecision(18, 5);
            modelBuilder.Entity<Note>().Property(c => c.TotalGravado).HasPrecision(18, 5);
            modelBuilder.Entity<Note>().Property(c => c.TotalProductExento).HasPrecision(18, 5);
            modelBuilder.Entity<Note>().Property(c => c.TotalProductGravado).HasPrecision(18, 5);
            modelBuilder.Entity<Note>().Property(c => c.TotalServExento).HasPrecision(18, 5);
            modelBuilder.Entity<Note>().Property(c => c.TotalServGravados).HasPrecision(18, 5);
            modelBuilder.Entity<Note>().Property(c => c.SaleTotal).HasPrecision(18, 5);
            modelBuilder.Entity<Note>().Property(c => c.DiscountAmount).HasPrecision(18, 5);

            modelBuilder.Entity<Service>()
                .HasMany(s => s.GroupConcepts)
                .WithMany(c => c.Services)
                .Map(cs =>
                {
                    cs.MapLeftKey("ServiceId");
                    cs.MapRightKey("GroupConceptsId");
                    cs.ToTable("GroupServices");
                });

            //modelBuilder.Entity<Client>()
            //    .HasMany(s => s.GroupConcepts)
            //    .WithMany(c => c.Client)
            //    .Map(cs =>
            //    {
            //        cs.MapLeftKey("ServiceId");
            //        cs.MapRightKey("GroupConceptsId");
            //        cs.ToTable("ClientGroupConcept");
            //    });
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
