using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace TicoPay.Inventory
{
    public class ProductTag : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Guid ProductId { get; set; }

        public virtual Tag Tag { get; set; }
        public virtual Guid TagId { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }
    }
}
