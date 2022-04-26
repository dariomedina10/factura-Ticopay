using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.BranchOffices;

namespace TicoPay.Drawers
{
    public class Drawer : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        /// <summary>
        /// código de la caja (debe ser un formato de 5 dígitos, y permite ceros adelante)
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// descripción
        /// </summary>
        public string Description { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
        public int TenantId { get; set; }
        /// <summary>
        /// Id Sucursal
        /// </summary>
        public Guid BranchOfficeId { get; set; }
        /// <summary>
        /// Sucursal
        /// </summary>
        public virtual BranchOffice BranchOffice { get; set; }
        public bool IsOpen { get; set; }
        public long? UserIdOpen { get; set; }
        public long? LastUserIdOpen { get; set; }
        public DateTime? OpenUserDate { get; set; }
        public DateTime? CloseUserDate { get; set; }
        public virtual ICollection<DrawerUser> DrawerUsers { get; set; }

        protected Drawer() { }

        public static Drawer Create(int TenantId, string  code, string description, Guid branchOfficeId)
        {
            var drawer = new Drawer
            {
                Id = Guid.NewGuid(),
                TenantId = TenantId,
                Code = code,
                Description = description,
                BranchOfficeId = branchOfficeId
            };

            return drawer;
        }

    }
}
