using Abp.Domain.Entities;
using System;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TicoPay.Invoices
{
    public class PaymentMethod : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }
        /// <summary>Gets or sets the Name. </summary>
         [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>Gets or sets the Gateway Url. </summary>
        public string GatewayUrl { get; protected set; }

        /// <summary>Gets or sets the Code. </summary>
         [MaxLength(2)]
        public string Code { get; set; }

        //public virtual ICollection<Invoice> Invoices { get; protected set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        /// <summary>
        /// We don't make constructor public
        /// But constructor can not be private since it's used by EntityFramework.
        /// Thats why we did it protected.
        /// </summary>
        //protected PaymentMethod()
        //{

        //}
    }
}
