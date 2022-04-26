using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Services;

namespace TicoPay.Clients
{
    //public class GroupServices : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    //{
    //    public virtual GroupConcepts Group { get; protected set; }
    //    public Guid GroupId { get; protected set; }

    //    public virtual Service Services { get; protected set; }
    //    public Guid ServiceId { get; protected set; }

    //    public int TenantId { get; set; }

    //    public bool IsDeleted { get; set; }

    //    public long? DeleterUserId { get; set; }

    //    public DateTime? DeletionTime { get; set; }

    //    public static GroupServices Create(GroupConcepts group, Service service)
    //    {
    //        var @cg = new GroupServices
    //        {
    //            Services = service,
    //            Group = group
    //        };
    //        return @cg;
    //    }

    //    protected GroupServices()
    //    {

    //    }
    //}
}
