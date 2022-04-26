using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.BranchOffices;
using TicoPay.Common;

namespace TicoPay.Drawers.Dto
{
    public class CreateDrawerInput : IDtoViewBaseFields
    {
        public Guid Id { get; set; }

        [Display(Name ="Código")]
        [StringLength(5,MinimumLength =5, ErrorMessage ="Código Tiene que ser longitud 5")]
        public string Code { get; set; }
        /// <summary>
        /// descripción
        /// </summary>
        [Display(Name = "Descripción")]
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

        [Display(Name = "Sucursal")]
        public List<BranchOffice> BranchOffice { get; set; }
     
        //SelectListItem
       public IList<SelectListItem> DrawerUser { get; set; }

        public bool IsOpen { get; set; }
        public long? UserIdOpen { get; set; }
        public long? LastUserIdOpen { get; set; }
        public DateTime? OpenUserDate { get; set; }
        public DateTime? CloseUserDate { get; set; }


        public int? ErrorCode { get ; set ; }
        public string ErrorDescription { get ; set ; }
        public string Action { get; set ; }
        public string Control { get ; set ; }
        public string Query { get ; set ; }

        // public virtual ICollection<DrawerUser> DrawerUse


        public bool isEnabled { get; set; }
        public DrawerDto result { get; set; }
    }
}
