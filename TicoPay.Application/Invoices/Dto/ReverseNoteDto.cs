using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Invoices.Dto
{
    public class ReverseNoteDto
    {
        [Required(ErrorMessage = "El campo InvoiceId es requerido")]
        public Guid InvoiceId { get; set; }

        [Required(ErrorMessage = "El campo NoteId es requerido")]
        public Guid OrigenNoteId { get; set; }

        [Display(Name = "Factura")]
        public int OrigenInvoiceNumber { get; set; }

        [Display(Name = "Número")]
        public string OrigenNoteNumber { get; set; }

        [Display(Name = "Tipo")]
        public NoteType OrigenNoteType { get; set; }

        [Display(Name = "Monto")]
        public decimal OrigenNoteAmount { get; set; }

        [Display(Name = "Impuesto")]
        public decimal OrigenNoteTax { get; set; }

        [Display(Name = "Total")]
        public decimal OrigenNoteTotal { get; set; }

        [Display(Name = "Código Moneda")]
        public NoteCodigoMoneda CodigoMoneda { get; set; }

        [Display(Name = "Monto")]
        [Required(ErrorMessage = "El campo Monto es requerido")]
        [Range(0.1, 99999999999, ErrorMessage = "El campo Monto debe ser un valor entre 0.1 y 99.999.999.999")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal Amount { get; set; }

        [Display(Name = "Impuesto")]
        [Range(0, 99999999999, ErrorMessage = "El campo Impuesto debe ser un valor entre 0.1 y 99.999.999.999")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal Tax { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal Total { get; set; }

        [Display(Name = "Razón")]
        [Required(ErrorMessage = "El campo Razón es requerido")]
        public NoteReason Reason { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "El campo Tipo es requerido")]
        public NoteType Type { get; set; }

        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public bool CanBeReversed { get; set; }
        public Guid ClientId { get; set; }
    }
}
