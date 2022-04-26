using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices.Dto;
using TicoPay.Invoices.XSD;

namespace TicoPay.ReportStatusInvoices.Dto
{
    public class IntegracionZohoSVConta
    {
        [Display(Name = "Tipo de documento")]
        public TypeDocument Type { get; set; }

        [Display(Name = "No. Documento")]
        public string ConsecutiveNumber { get; set; }

        [Display(Name = "Nº Identificación")]
        public string ClientIdentification { get; set; }

        public Guid? ClientId { get; set; }

        [Display(Name = "Nombre Cliente")]
        public string ClientName { get; set; }

        [Display(Name = "Fecha Documento")]
        public DateTime DateDocument { get; set; }

        [Display(Name = "Fecha Vencimiento")]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "Moneda")]
        public string CodigoMoneda { get; set; }

        [Display(Name = "Precio")]
        public decimal ItemPrice { get; set; }

        [Display(Name = "Cantidad")]
        public decimal Cantidad { get; set; }

        [Display(Name = "Servicio")]
        public string Servicio { get; set; }

        [Display(Name = "Descuento Porcentaje")]
        public decimal DiscountPercentage { get; set; }

        [Display(Name = "Descuento Descripción")]
        public string DescriptionDiscount { get; set; }

        [Display(Name = "Descuento de Importe")]
        public decimal DiscountAmount { get; set; }

        [Display(Name = "Descripcion Impuesto")]
        public string ItemTax1 { get; set; }

        [Display(Name = "Impuesto %")]
        public decimal ItemTax { get; set; }

        [Display(Name = "Impuesto Importe")]
        public decimal ItemTaxAmount { get; set; }

        [Display(Name = "Sub Total")]
        public decimal ItemTotal { get; set; }

        [Display(Name = "Total")]
        public decimal Total { get; set; }

        public FacturaElectronicaCondicionVenta ConditionSaleType { get; set; }

        [Display(Name = "Monto Total Excento")]
        public decimal TotalGravado { get; set; }

        [Display(Name = "Monto Total Gravado")]
        public decimal TotalExento { get; set; }

        public IdentificacionTypeTipo IdentificationType { get; set; }

        public ICollection<InvoiceSVContaLines> InvoiceSVContaLines { get; set; }

        public string IdentificacionExtranjero { get; set; }
    }
    

    public class InvoiceSVContaLines
    {
        public decimal ItemPrice { get; set; }
        public decimal Cantidad { get; set; }
        public string Servicio { get; set; }
        public string DescriptionDiscount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public string ItemTax1 { get; set; }
        public decimal ItemTax { get; set; }
        public decimal ItemTaxAmount { get; set; }
        public decimal ItemTotal { get; set; }
        public decimal Total { get; set; }
    }

    public enum IntegrationZohoSVConta
    {
        [Description("SVConta")]
        SVConta = 0,
        [Description("Zoho")]
        Zoho
    }
}
