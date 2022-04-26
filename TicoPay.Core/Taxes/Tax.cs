using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using TicoPay.General;
using System.ComponentModel.DataAnnotations;
using TicoPay.Invoices.XSD;
using System.Collections.Generic;
using TicoPay.Services;

namespace TicoPay.Taxes
{
    public class Tax: AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {

        public int TenantId { get; set; }
        /// <summary>
        /// Gets or sets the tax Name
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the Rate
        /// </summary>
        [Required]
        public decimal Rate { get; set; }

        public ImpuestoTypeCodigo TaxTypes { get;  set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public virtual ICollection<Service> Services { get; protected set; }

        public static Tax Create(int tenantId, string name, decimal rate, ImpuestoTypeCodigo taxtype)
        {
            var @tax = new Tax
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = name,
                Rate = rate,
                IsDeleted = false,
                TaxTypes = taxtype
            };
            //@service.Taxs = new Collection<ClientService>();
            //@client.Invoices = new Collection<Invoice>();
            return @tax;
        }
        public static Tax Create(decimal rate)
        {
            var @tax = new Tax
            {
                Id = Guid.NewGuid(),
                Rate = rate
            };
            return @tax;
        }
    }

    //public enum TaxesTypes
    //{
    //    ImpuestoGeneralVentas=12,
    //    Exento=13
    //}
}
