using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Drawers;

namespace TicoPay.BranchOffices
{
    public class BranchOffice : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        /// <summary>
        /// Nombre de sucursal
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Código de sucursal
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Ubicacion de la sucursal
        /// </summary>
        public string Location { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
        public int TenantId { get; set; }
        /// <summary>
        /// cajas asocidas a la sucursal 
        /// </summary>
        public virtual ICollection<Drawer> Drawers { get; protected set; }

        public BranchOffice() { }

        public static BranchOffice Create(int tenantId, string name, string code, string location)
        {
            BranchOffice branchOffice = new BranchOffice()
            {
                TenantId = tenantId,
                Id = Guid.NewGuid(),
                Name = name,
                Code =code,
                Location = location
            };

            return branchOffice;
        }
    }
}
