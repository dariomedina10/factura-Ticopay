using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices;
using TicoPay.ReportInvoicesSentToTribunet.Dto;

namespace TicoPay.Api.Models
{
    /// <summary>
    /// Clase que contiene la Información de las facturas enviadas a Hacienda
    /// </summary>
    public class InvoiceSentToTribunetModel
    {
        /// <summary>
        /// Obtiene el Número de Factura.
        /// </summary>
        /// <value>
        /// Número de Factura.
        /// </value>
        [Description("Número")]
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Obtiene la Fecha de Emisión de la factura.
        /// </summary>
        /// <value>
        /// Fecha de Emisión.
        /// </value>
        [Description("Fecha Emisión")]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Obtiene la Fecha de Pago de la factura.
        /// </summary>
        /// <value>
        /// Fecha de Pago.
        /// </value>
        [Description("Fecha Pago")]
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Obtiene la Forma de Pago.
        /// </summary>
        /// <value>
        /// Forma de Pago.
        /// </value>
        [Description("Forma Pago")]
        public PaymetnMethodType PaymetnMethodType { get; set; }

        /// <summary>
        /// Obtiene el Nombre de Cliente de la factura.
        /// </summary>
        /// <value>
        /// Nombre de Cliente.
        /// </value>
        [Description("Nombre Cliente")]
        public string NombreCliente { get; set; }

        /// <summary>
        /// Obtiene el Estado de la Factura según Ticopay.
        /// </summary>
        /// <value>
        /// Estado de la Factura.
        /// </value>
        [Description("Estado")]
        public Status Status { get; set; }

        /// <summary>
        /// Obtiene el Estado de la Factura según Hacienda.
        /// </summary>
        /// <value>
        /// Estado de la Factura.
        /// </value>
        [Description("Estado Hacienda")]
        public StatusTaxAdministration StatusTribunet { get; set; }

        /// <summary>
        /// Obtiene si la Factura fue confirmada como recibida.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si la Factura fue confirmada recibida; sino, <c>false</c>.
        /// </value>
        [Description("Acuse Recibo")]
        public bool IsInvoiceReceptionConfirmed { get; set; }

        /// <summary>
        /// Obtiene el Total de la Factura.
        /// </summary>
        /// <value>
        /// Total de la Factura.
        /// </value>
        [Description("Monto")]
        public decimal TotalInvoiceLines { get; set; }

        /// <summary>
        /// Obtiene el Total de las Notas de Débito.
        /// </summary>
        /// <value>
        /// Total de las Notas de Débito.
        /// </value>
        [Description("Débitos")]
        public decimal TotalNotasDebito { get; set; }

        /// <summary>
        /// Obtiene el Total de las Notas de Crédito.
        /// </summary>
        /// <value>
        /// Total de las Notas de Crédito.
        /// </value>
        [Description("Créditos")]
        public decimal TotalNotasCredito { get; set; }

        /// <summary>
        /// Obtiene el Total de Impuesto.
        /// </summary>
        /// <value>
        /// Total de Impuesto.
        /// </value>
        [Description("Impuestos")]
        public decimal TotalTaxes { get; set; }

        /// <exclude />
        public InvoiceSentToTribunetModel(ReportInvoicesSentToTribunetDto dto)
        {
            NumeroFactura = dto.ConsecutiveNumber;
            DueDate = dto.DueDate;
            PaymentDate = dto.PaymentDate.GetValueOrDefault();
            PaymetnMethodType = dto.PaymetnMethodType.GetValueOrDefault();
            NombreCliente = (dto.Client == null) ? dto.NombreCliente : (dto.Client.Name + (dto.Client == null ? "" : " " + dto.Client.LastName));
            Status = dto.Status;
            StatusTribunet = dto.StatusTribunet;
            IsInvoiceReceptionConfirmed = dto.IsInvoiceReceptionConfirmed;
            TotalInvoiceLines = dto.TotalInvoiceLines;
            TotalNotasDebito = dto.TotalNotasDebito;
            TotalNotasCredito = dto.TotalNotasCredito;
            TotalTaxes = dto.TotalTaxes;
        }
    }
}
