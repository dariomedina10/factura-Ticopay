using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using TicoPay.Invoices;

namespace TicoPay.Inventory
{
    public class ProductVariant: AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }

        public ProductVariantType ProductVariantType { get; set; }
        public string Value { get; set; }
        /// <summary>Gets or sets the InvoiceId. </summary>
        public Guid InvoiceId { get; set; }

        /// <summary>Gets or sets the Invoice. </summary>
        public Invoice Invoice { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }
    }

    public enum ProductVariantType
    {
        Colour,
        Fabric,
        Material,
        Season,
        Size,
        Style,
        Title
    }
}
