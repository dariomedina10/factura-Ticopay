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
    public class Exoneration : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public int TenantId { get; set; }

        [MaxLength(2)]
        public string DocumentType { get; set; }

        [MaxLength(17)]
        public string DocumentNumber { get; set; }

        [MaxLength(100)]
        public string InstitutionName { get; set; }

        public DateTime ExonerationDate { get; set; }
        public decimal TaxAmountExonerated { get; set; }
        public int PercentagePurchaseExonerated { get; set; }
        /// <summary>Gets or sets the User Id. </summary>
        public Guid? UserId { get; set; }

        /// <summary>Gets or sets the User Name. </summary>
        public string UserName { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public virtual ICollection<InvoiceLine> Notes { get; protected set; }
    }
}
