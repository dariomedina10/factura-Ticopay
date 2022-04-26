using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;


namespace TicoPay.Invoices
{
    public class PaymentNote : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }
        /// <summary>
        /// Gets or sets the Amount. 
        /// </summary>
        public decimal Amount { get; protected set; }

        public PaymetnMethodType PaymetnMethodType { get; set; }

        /// <summary>Gets or sets the InvoiceId. </summary>
        public Guid NoteId { get; protected set; }

        /// <summary>Gets or sets the Invoice. </summary>
        public virtual Note Note { get; protected set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public bool? IsPaymentReversed { get; set; }

        public DateTime? DeletionTime { get; set; }

        public Guid PaymentId { get; protected set; }

        public virtual Payment Payment { get; protected set; }

        protected PaymentNote()
        {

        }

        public static PaymentNote Create(int tenantId, decimal amount, Note note,PaymetnMethodType paymetnMethodType)
        {
            var entity = new PaymentNote
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Amount = amount,
                PaymetnMethodType=paymetnMethodType,
                Note = note
            };
            return entity;
        }
    }
}
