using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;

namespace TicoPay.General
{
    public class Address : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        /// <summary>
        /// Gets or sets the City. 
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the City. 
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the Country. 
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the PostCode. 
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>Gets or sets the Suburb. </summary>
        public string Suburb { get; set; }

        /// <summary>
        /// Gets or sets the StreetNumber. 
        /// </summary>
        public string StreetNumber { get; set; }

        /// <summary>Fisrt street address line. </summary>
        public string Address1 { get; set; }

        /// <summary>Second street address line. </summary>
        public string Address2 { get; set; }

        /// <summary>Gets or sets the Primary. </summary>
        public bool Primary { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public int TenantId { get; set; }
    }
}
