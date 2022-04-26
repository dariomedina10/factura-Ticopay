using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using TicoPay.Clients;

namespace TicoPay.EntityFramework.Configurations
{
    public class ClientConfiguration: EntityTypeConfiguration<Client>
    {
        public ClientConfiguration(DbModelBuilder modelBuilder)
        {
            ToTable("Client", "TicoPay");
            Property(c => c.Name).IsRequired().HasMaxLength(200);
            Property(c => c.Identification).HasMaxLength(50);
            Property(c => c.Code).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(c => c.Note).HasMaxLength(250);
            Property(c => c.IsoCountry).HasMaxLength(3);
            Property(c => c.MobilNumber).HasMaxLength(15);
            Property(c => c.PhoneNumber).HasMaxLength(15);
            Property(c => c.Address).HasMaxLength(200);
            Property(c => c.Email).HasMaxLength(100);
        }
    }
}
