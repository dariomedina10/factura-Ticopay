using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Invoices
{
    public class Bank : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        /// <summary>
        /// Igets or set Bank Name
        /// </summary>
        [Required]
        [StringLength(60)]
        public string Name { get; set; }
        /// <summary>
        /// Igets or set Bank Name
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// bank Is Active
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// gets o sets TennantId
        /// </summary>
        public int TenantId { get; set; }
        /// <summary>
        /// InvoicePaymentMethod
        /// </summary>
        public virtual ICollection<PaymentInvoice> PaymentsInvoices { get; protected set; }
        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// DeletedUserId
        /// </summary>
        public long? DeleterUserId { get; set; }
        /// <summary>
        /// DeletedTime
        /// </summary>
        public DateTime? DeletionTime { get; set; }

        public static Bank Create(int tenantId, string name, string shortName)
        {
            var @bank = new Bank
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = name,
                ShortName = shortName,
                IsActive = true,
                IsDeleted = false
            };
        return @bank;
        }
    }
}
