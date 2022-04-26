using Abp.Domain.Entities;
using System;
using Abp.Domain.Entities.Auditing;
using TicoPay.Taxes;

namespace TicoPay.Inventory
{
    public class Outlet : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets default tax id
        /// </summary>
        public Guid? DefaultTaxId { get; set; }

        /// <summary>
        /// Gets or sets default tax
        /// </summary>
        public Tax DefaultTax { get; set; }

        /// <summary>
        /// Gets or sets Order Number Prefix
        /// </summary>
        public string OrderNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets Supplier Return Number Prefix
        /// </summary>
        public string SupplierReturnNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets the supplier number
        /// </summary>
        public Guid SupplierReturnNumberId { get; set; }

        /// <summary>
        /// Gets or sets the supplier number
        /// </summary>
        public Guid SupplierReturnNumber { get; set; }

        /// <summary>Gets or sets the Phone Number. </summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the Address
        /// </summary>
        public string Address { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }
    }
}
