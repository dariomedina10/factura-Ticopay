using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TicoPayDll.Invoices;

namespace TicoPayDll.Reports
{
    public class InvoiceSendTribunetReport
    {
        [JsonProperty]
        public string ReportName;
        [JsonProperty]
        public string PrintDate;
        [JsonProperty]
        public InvoiceSendTribunet[] Invoices;
    }

    public class InvoiceSendTribunet
    {
        [JsonProperty]
        public string NumeroFactura;

        [JsonProperty]
        public string DueDate;

        [JsonProperty]
        public string PaymentDate;

        [JsonProperty]
        public PaymetnMethodType PaymetnMethodType;

        [JsonProperty]
        public string NombreCliente;

        [JsonProperty]
        public Status Status;

        [JsonProperty]
        public StatusTaxAdministration StatusTribunet;

        [JsonProperty]
        public bool IsInvoiceReceptionConfirmed;

        [JsonProperty]
        public decimal TotalInvoiceLines;

        [JsonProperty]
        public decimal TotalNotasDebito;

        [JsonProperty]
        public decimal TotalNotasCredito;

        [JsonProperty]
        public decimal TotalTaxes;
    }

    public enum StatusTaxAdministration
    {
        // No Enviado
        NoEnviada = 0,
        // Recibido
        Recibido,
        // Procesando
        Procesando,
        // Aceptado
        Aceptado,
        // Rechazado
        Rechazado,
        // Error
        Error
    }    

    public enum FacturaElectronicaCondicionVenta
    {
        Contado,
        Credito,
        Consignacion,
        Apartado,
        Arrendamiento_Opcion_de_Compra,
        Arrendamiento_Funcion_Financiera,
        Otros,
    }
}
