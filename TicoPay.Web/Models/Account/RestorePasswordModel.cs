using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TicoPay.Common;

namespace TicoPay.Web.Models.Account
{
    public class RestorePasswordModel : IDtoViewBaseFields
    {
        public const string subject = "Restablecer la contraseña";
        public const string emailbody = "<p>No tiene que preocuparse si ha olvidado la contraseña. Haga clic en el siguiente enlace para crear una contraseña nueva.</p>";
        public const string emailfooter = "<p> Si Ud. no solicitó un restablecimiento de contraseña, solo descarte este correo.</p> <p>Para cualquier ayuda contáctenos a, soporte@ticopays.com</p> <p>Gracias</p> <p><b>Equipo TicoPay</b></p>";

        [Required]
        public string Email { get; set; }

        public bool ReCaptcha { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }

    }
}