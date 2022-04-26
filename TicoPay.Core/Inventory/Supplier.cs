using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace TicoPay.Inventory
{
    public class Supplier: AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }
        /// <summary>
        /// Gets or sets the Name. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Description. 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Default Markup. 
        /// </summary>
        public decimal DefaultMarkup { get; set; }

        /// <summary>Gets or sets the Product List. </summary>
        public virtual ICollection<Product> Products { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }
    }
}
