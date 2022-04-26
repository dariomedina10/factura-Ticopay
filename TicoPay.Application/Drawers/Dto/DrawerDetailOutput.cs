using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.BranchOffices;
using TicoPay.Common;
using TicoPay.Users;

namespace TicoPay.Drawers.Dto
{
    [AutoMapFrom(typeof(Drawer))]
    public class DrawerDetailOutput : IDtoViewBaseFields
    {
        public Guid Id { get; set; }

        [Display(Name = "Codigo")]
        public string Code { get; set; }
        /// <summary>
        /// descripción
        /// </summary>
        [Display(Name = "Descripcion")]
        public string Description { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
        public int TenantId { get; set; }
        /// <summary>
        /// Id Sucursal
        /// </summary>
        [Display(Name = "Sucursal")]
        public Guid BranchOfficeId { get; set; }
        /// <summary>
        /// Sucursal
        /// </summary>
        /// 
        [Display(Name = "Sucursal")]
        public BranchOffice BranchOffice { get; set; }

        [Display(Name = "Sucursal")]
        public List<BranchOffice> BranchOffices { get; set; }
        public bool IsOpen { get; set; }
        public long? UserIdOpen { get; set; }
        public long? LastUserIdOpen { get; set; }
        public DateTime? OpenUserDate { get; set; }
        public DateTime? CloseUserDate { get; set; }
        // public virtual ICollection<DrawerUser> DrawerUsers { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

        public IList<User> Users { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public long LastModifierUserId { get; set; }

        public long CreatorUserId { get; set; }
    }
}
