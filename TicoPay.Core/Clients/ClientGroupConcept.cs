using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicoPay.Clients
{

    public class ClientGroupConcept : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        [ForeignKey("GroupId")]
        public virtual GroupConcepts Group { get; protected set; }
        public Guid GroupId { get; protected set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; protected set; }       
        public Guid ClientId { get; protected set; }

        public int TenantId { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public decimal Quantity { get; set; } = 1;

        public decimal DiscountPercentage { get; set; } = 0;

        public static ClientGroupConcept Create(GroupConcepts group, Client client, decimal quantity, decimal discountPercentage)
        {
            var @cg = new ClientGroupConcept
            {
                Client = client,
                Group = group,
                Quantity=quantity,
                DiscountPercentage=discountPercentage
            };
            return @cg;
        }

        protected ClientGroupConcept()
        {

        }
    }
}
