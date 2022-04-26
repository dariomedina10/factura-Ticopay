using Abp.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.Common;

namespace TicoPay.Roles.Dto
{
    public class CreateRoleInput : IDtoViewBaseFields
    {
        [Required]
        [Display(Name = "Nombre: ")]
        public string Name { get; set; }

        [Display(Name = "Permisos: ")]
        public List<RecursiveObject> PermissionNames { get; set; }

        public object[] permisos { get; set; }

        public string DisplayName { get; set; }

        public bool IsStatic { get; set; }
        public bool IsDefault { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
