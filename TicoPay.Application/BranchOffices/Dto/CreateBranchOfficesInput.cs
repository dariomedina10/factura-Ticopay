using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Common;

namespace TicoPay.BranchOffices.Dto
{
    [AutoMapFrom(typeof(BranchOffice))]
    public class CreateBranchOfficesInput: IDtoViewBaseFields
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name ="Nombre")]
        public string Name { get; set; }
        /// <summary>
        /// Código de sucursal
        /// </summary>
        [Required]
        [StringLength(3, ErrorMessage = "Campo código debe tener una Longitud Maxima de 3" )]
        [Display(Name ="Código")]
        public string Code { get; set; }
        /// <summary>
        /// Ubicacion de la sucursal
        /// </summary>
        [Required]
        [Display(Name ="Ubicación")]
        public string Location { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
        public int TenantId { get; set; }

        public bool isEnabled { get; set; }

        public int? ErrorCode { get ; set; }
        public string ErrorDescription { get ; set ; }
        public string Action { get ; set ; }
        public string Control { get ; set ; }
        public string Query { get; set ; }

        /// <summary>
        /// cajas asocidas a la sucursal 
        /// </summary>
        public BranchOfficesDto result { get; set; }

    }
}
