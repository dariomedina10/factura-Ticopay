using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Invoices.Dto
{
    public class ApplyReverseInput
    {
        [Required(ErrorMessage = "El campo InvoiceId es requerido")]
        public Guid InvoiceId { get; set; }

        [Required(ErrorMessage = "El campo Amount es requerido")]
        [Range(0.1, 99999999999, ErrorMessage = "El campo Amount debe ser un valor entre 0.1 y 99.999.999.999")]
        public decimal Amount { get; set; }

        [Range(0, 99999999999, ErrorMessage = "El campo Tax debe ser un valor entre 0.1 y 99.999.999.999")]
        public decimal Tax { get; set; }

        //[Required(ErrorMessage = "El campo Reason es requerido")]
        //public NoteReason Reason { get; set; }

        //[Required(ErrorMessage = "El campo Type es requerido")]
        //public NoteType Type { get; set; }

    }
}
