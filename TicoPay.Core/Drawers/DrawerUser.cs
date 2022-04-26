using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.BranchOffices;

namespace TicoPay.Drawers
{
    public class DrawerUser : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
        public int TenantId { get; set; }
        public Guid? DrawerId { get; set; }
        [ForeignKey("DrawerId")]
        public virtual Drawer Drawer { get; set; }
        public bool IsActive { get; set; }
    }
}
