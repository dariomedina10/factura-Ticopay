using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TicoPayDll.Invoices;

namespace TicoPayDll.Notes
{
    public class Note
    {
        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public int TenantId { get; set; }

        [JsonProperty]
        public decimal Amount { get; protected set; }

        [JsonProperty]
        public decimal TaxAmount { get; protected set; }

        [JsonProperty]
        public decimal Total { get; protected set; }

        [JsonProperty]
        public string InvoiceId { get; protected set; }

        [JsonProperty]
        public DateTime CreationTime { get; protected set; }

        [JsonProperty]
        public virtual Invoice Invoice { get; protected set; }

        [JsonProperty]
        public string ExchangeRateId { get; protected set; }

        [JsonProperty]
        public virtual ExchangeRate ExchangeRate { get; protected set; }

        [JsonProperty]
        public string Reference { get; protected set; }

        [JsonProperty]
        public CodigoMoneda CodigoMoneda { get; set; }

        [JsonProperty]
        public NoteReason NoteReasons { get; set; }

        [JsonProperty]
        public byte[] QRCode { get; set; }

        [JsonProperty]
        public string VoucherKey { get; set; }

        [JsonProperty]
        public string ConsecutiveNumber { get; set; }

        [JsonProperty]
        public decimal ChangeType { get; set; }

        [JsonProperty]
        public NoteType NoteType { get; set; }

        [JsonProperty]
        public bool SendInvoice { get; set; }

        [JsonProperty]
        public StatusTaxAdministration StatusTribunet { get; set; }

        [JsonProperty]
        public VoucherSituation StatusVoucher { get; set; }

        [JsonProperty]
        public string ConsecutiveNumberReference { get; set; }

        [JsonProperty]
        public FirmType? TipoFirma { get; set; }

        [JsonProperty]
        public StatusFirmaDigital? StatusFirmaDigital { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Numero de Referencia Externa (Usuarios del Api).
        /// </summary>
        /// <value>
        /// Numero de Referencia Externa (Usuarios del Api).
        /// </value>
        public string ExternalReferenceNumber { get; set; }
    }

    public enum NoteReason
    {
        /// Reversa un Documento (Por el monto completo)
        [System.Xml.Serialization.XmlEnumAttribute("01")]
        Reversa_documento,
        /// Corregir Texto del Documento (Corrección de Errores)
        [System.Xml.Serialization.XmlEnumAttribute("02")]
        Corregir_Texto_Documento,
        /// Corregir Monto de Factura (Cambios a lineas de la factura)
        [System.Xml.Serialization.XmlEnumAttribute("03")]
        Corregir_Monto_Factura,
        /// Referencia de Documento
        [System.Xml.Serialization.XmlEnumAttribute("04")]
        Referencia_documento,
        /// Sustituye Comprobante Provisional
        [System.Xml.Serialization.XmlEnumAttribute("05")]
        Sustituye_comprobante,
        /// Otros
        [System.Xml.Serialization.XmlEnumAttribute("99")]
        Otros
    }

    public enum VoucherSituation
    {
        Normal = 1,
        Contigency,
        withoutInternet
    }
}
