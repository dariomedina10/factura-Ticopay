using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using TicoPay.Invoices;
using TicoPay.Services;

namespace TicoPay.Clients
{
    public class ClientGroup : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public virtual Group Group { get; protected set; }
        public Guid GroupId { get; protected set; }

        public virtual Client Client { get; protected set; }
        public Guid ClientId { get; protected set; }

        public int TenantId { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public static ClientGroup Create(Group group, Client client)
        {
            var @cg = new ClientGroup
            {
                Client = client,
                Group = group
            };
            return @cg;
        }

        protected ClientGroup()
        {

        }
    }
}
