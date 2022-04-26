using System;
using System.ComponentModel.DataAnnotations;
using TicoPay.Common;

namespace TicoPay.Groups.Dto
{
    public class CreateGroupInput: IDtoViewBaseFields
    {
        [Required]
        [MaxLength(60)]
        [Display(Name = "Nombre: ")]
        public string Name { get; set; }

        [MaxLength(1024)]
        [Display(Name = "Descripción: ")]
        public string Description { get; set; }

        public int TenantId { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
