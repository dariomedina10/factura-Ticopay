using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Home
{
    public class ContactDTO
    {
        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Debe Ingresar un nombre valido")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Campo requerido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Debe Ingresar un asunto valido")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Debe Ingresar un mensaje valido")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "Debe Ingresar un teléfono valido")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Debe Completar el captcha.")]
        public bool ReCaptcha { get; set; } = false;
    }
}
