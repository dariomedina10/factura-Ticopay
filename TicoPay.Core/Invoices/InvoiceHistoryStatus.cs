using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace TicoPay.Invoices
{
    public class InvoiceHistoryStatus: AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }

        public Status Status { get; protected set; }
        /// <summary>Gets or sets the InvoiceId. </summary>
        public Guid InvoiceId { get; protected set; }

        /// <summary>Gets or sets the Invoice. </summary>
        public Invoice Invoice { get; protected set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        protected InvoiceHistoryStatus()
        {

        }

        public static InvoiceHistoryStatus Create(int tenantId, Status status, Invoice invoice)
        {
            var entity = new InvoiceHistoryStatus
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Status = status,
                Invoice = invoice,
            };
            return entity;
        }
    }
}
