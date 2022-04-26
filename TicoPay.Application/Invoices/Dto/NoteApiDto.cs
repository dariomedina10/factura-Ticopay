using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Invoices.Dto
{
    [AutoMapFrom(typeof(Note))]
    public class NoteApiDto
    {
        public Guid Id { get; set; }

        public int TenantId { get; set; }

        public decimal Amount { get; protected set; }

        public decimal TaxAmount { get; protected set; }

        public DateTime CreationTime { get; protected set; }

        public decimal Total { get; protected set; }

        public virtual InvoiceApiDto Invoice { get; protected set; }

        public Guid InvoiceId { get; protected set; }

        public Guid? ExchangeRateId { get; protected set; }

        public virtual ExchangeRate ExchangeRate { get; protected set; }

        public string Reference { get; protected set; }

        public NoteCodigoMoneda CodigoMoneda { get; set; }

        public NoteReason NoteReasons { get; set; }

        public byte[] QRCode { get; set; }

        public string VoucherKey { get; set; }

        public string ConsecutiveNumber { get; set; }

        public decimal ChangeType { get; set; }

        public NoteType NoteType { get; set; }

        public bool SendInvoice { get; set; }

        public StatusTaxAdministration StatusTribunet { get; set; }

        public VoucherSituation StatusVoucher { get; set; }

        public string ConsecutiveNumberReference { get; set; }

        public FirmType? TipoFirma { get; set; }

        public StatusFirmaDigital? StatusFirmaDigital { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Numero de Referencia Externa (Usuarios del Api).
        /// </summary>
        /// <value>
        /// Numero de Referencia Externa (Usuarios del Api).
        /// </value>
        public string ExternalReferenceNumber { get; set; }
    }
}
