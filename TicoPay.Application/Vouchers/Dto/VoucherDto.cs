using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.Drawers;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;
using TicoPay.MultiTenancy;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Vouchers.Dto
{
    public class VoucherDto
    {
        public const int MaxNameLength = 80;
        public const int MaxIdentificationLength = 12;
        public const int MaxIdentificationExtranjeroLength = 20;
        public const int MaxConsecutiveNumberLength = 20;
        public const int MaxKeyLength = 50;

        public Guid Id { get; set; }

        //[MaxLength(MaxKeyLength)]
        //[Display(Name = "Clave")]
        //public string VoucherKey { get; set; }

        public string Type { get; set; }

        [Required]
        [StringLength(MaxIdentificationLength)]
        [Display(Name = "No. Identificación")]
        public string IdentificationSender { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        [Display(Name = "Nombre Emisor")]
        public string NameSender { get; set; }

        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Display(Name = "Nombre Receptor")]
        public string NameReceiver  { get; set; }

        [Display(Name = "No. Identificación Receptor")]
        public string IdentificationReceiver { get; set; }

        [MaxLength(MaxConsecutiveNumberLength)]
        [Display(Name = "No. Factura")]
        public string ConsecutiveNumberInvoice { get; set; }

  
        [Display(Name = "Fecha Factura")]
        public DateTime DateInvoice { get; set; }


        /// <summary>
        /// Clave foranea de moneda
        /// </summary>
        [Display(Name = "Moneda")]
        public FacturaElectronicaResumenFacturaCodigoMoneda Coin { get; set; }

        [Required]
        [Display(Name = "Total Factura")]
        public decimal Totalinvoice { get; set; }

        [Display(Name = "Total Impuesto")]
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Clave del comprobante
        /// </summary>
        [MaxLength(MaxKeyLength)]
        [Display(Name = "Clave")]
        public string VoucherKey { get; set; }

        [Display(Name = "Clave Referido")]
        public string VoucherKeyRef { get; set; }
        /// <summary>
        /// Consecutivo de mensases de confirmación
        /// </summary>
        [MaxLength(MaxConsecutiveNumberLength)]
        [Display(Name = "No. Comprobante")]
        public string ConsecutiveNumber { get; set; }

        /// <summary>
        /// Código del mensaje de respuesta
        /// </summary>
       [Display(Name = "Mensaje")]
        public MessageVoucher Message { get; set; }

        /// <summary>
        /// Detalle del mensaje
        /// </summary>
        [StringLength(MaxNameLength)]
        [Display(Name = "Detalle del Mensaje")]
        public string DetailsMessage { get; set; }
        /// <summary>
        /// XML del comprobante
        /// </summary>
        public string ElectronicBill { get; set; }
        /// <summary>
        /// Indica si el comprobante fue enviado a Hacienda
        /// </summary>
        public bool SendVoucher { get; set; }
        /// <summary>
        /// Estatus del comprobate en hacienda
        /// </summary>
        [Display(Name = "Estado Hacienda")]
        public StatusTaxAdministration StatusTribunet { get; set; }

        [Display(Name = "Tipo de Firma")]
        public FirmType? TipoFirma { get; set; }

        public StatusFirmaDigital? StatusFirmaDigital { get; set; }

        public DateTime CreationTime { get; set; }

        public string XLM { get; set; }
                
        public System.Web.HttpPostedFileBase File { get; set; }

        public bool isFile { get; set; }

        public Drawer Drawer { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }

        public IEnumerable<SelectListItem> ListType { get; set; }

        public int MinimumAmount { get; set; }
        public int MaxAmount { get; set; }

        [Display(Name = "Tipo de Documento")]
        public bool IsTypeDocument { get; set; }

        public TypeVoucher TypeVoucher { get; set; } = TypeVoucher.Purchases;

        public string MessageTaxAdministration { get; set; }

        public MessageSupplier? MessageSupplier { get; set; }
        public string MessageTaxAdministrationSupplier { get; set; }

    }
}
