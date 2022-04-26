using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TicoPay.Inventory
{
    public class Brand : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }

        /// <summary>
        /// Gets or sets the Name. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Name. 
        /// </summary>
        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        /// <summary>Gets or sets the Product List. </summary>
        public virtual ICollection<Product> Products { get; set; }
    }
}
