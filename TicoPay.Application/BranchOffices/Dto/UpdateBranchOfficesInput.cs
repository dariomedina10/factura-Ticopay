using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Common;
using TicoPay.Drawers;

namespace TicoPay.BranchOffices.Dto
{
    [AutoMapFrom(typeof(BranchOffice))]
    public class UpdateBranchOfficesInput : IDtoViewBaseFields
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Name { get; set; }
        /// <summary>
        /// Código de sucursal
        /// </summary>
        [Required]
        [StringLength(3, ErrorMessage = "Campo con Longitud Maxima 3")]
        [Display(Name = "Código")]
        public string Code { get; set; }
        /// <summary>
        /// Ubicacion de la sucursal
        /// </summary>
        [Required]
        [Display(Name = "Ubicación")]
        public string Location { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
        public int TenantId { get; set; }
        /// <summary>
        /// cajas asocidas a la sucursal 
        /// </summary>
        public virtual ICollection<Drawer> Drawers { get; protected set; }





        public int? ErrorCode { get ; set ; }
        public string ErrorDescription { get ; set ; }
        public string Action { get ; set ; }
        public string Control { get ; set ; }
        public string Query { get ; set ; }
    }
}
