using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TicoPayDll.Invoices;

namespace TicoPayDll.Vouchers
{
    public class Voucher
    {   
        public Guid Id { get; set; }
                
        [Display(Name = "No. Identificación")]
        public string IdentificationSender { get; set; }

        [Display(Name = "Nombre Emisor")]
        public string NameSender { get; set; }

        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Display(Name = "Nombre Receptor")]
        public string NameReceiver { get; set; }

        [Display(Name = "No. Identificación Receptor")]
        public string IdentificationReceiver { get; set; }
                
        [Display(Name = "No. Factura")]
        public string ConsecutiveNumberInvoice { get; set; }

        [Display(Name = "Fecha Factura")]
        public DateTime DateInvoice { get; set; }

        [Display(Name = "Moneda")]
        public CodigoMoneda Coin { get; set; }

        [Display(Name = "Total Factura")]
        public decimal Totalinvoice { get; set; }

        [Display(Name = "Total Impuesto")]
        public decimal TotalTax { get; set; }
                
        [Display(Name = "Clave")]
        public string VoucherKey { get; set; }

        [Display(Name = "Clave Referido")]
        public string VoucherKeyRef { get; set; }
        
        [Display(Name = "No. Comprobante")]
        public string ConsecutiveNumber { get; set; }
                
        [Display(Name = "Mensaje")]
        public MessageVoucher Message { get; set; }
                
        [Display(Name = "Detalle del Mensaje")]
        public string DetailsMessage { get; set; }
        
        public string ElectronicBill { get; set; }

        public bool SendVoucher { get; set; }

        [Display(Name = "Estado Hacienda")]
        public StatusTaxAdministration StatusTribunet { get; set; }

        public FirmType TipoFirma { get; set; }

        public StatusFirmaDigital StatusFirmaDigital { get; set; }
                
        public DateTime creationTime { get; set; }

    }

    public enum MessageVoucher
    {
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        // "Aceptado"
        Aceptado = 0,
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        // "Aceptado Parcialmente"
        AceptadoParcial,
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        // "Rechazado"
        Rechazado,
    }
}
