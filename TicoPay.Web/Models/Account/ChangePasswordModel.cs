using Abp.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TicoPay.Common;
using TicoPay.Users;

namespace TicoPay.Web.Models.Account
{
    public class ChangePasswordModel : IDtoViewBaseFields
    {
        [Required]
        [DisableAuditing]
        public string Password { get; set; }

        public long IdUser { get; set; }

        public string UserName { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "La confirmación de la contraseña no coincide")]
        [DisableAuditing]
        public string CofirmedPassword { get; set; }

        public string CodeResetPassword { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}