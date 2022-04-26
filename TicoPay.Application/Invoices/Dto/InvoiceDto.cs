using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using TicoPay.Clients;
using TicoPay.Services;
using TicoPay.General;
using TicoPay.Invoices.XSD;
using TicoPay.MultiTenancy;
using static TicoPay.MultiTenancy.Tenant;

namespace TicoPay.Invoices.Dto
{
    [AutoMapFrom(typeof(Invoice))]
    public class InvoiceDto : EntityDto<Guid>
    {
        public virtual Tenant Tenant { get; protected set; }
        public int TenantId { get; set; }

        public int Number { get; protected set; }

        [MaxLength(50)]
        public string Alphanumeric { get; protected set; }

        [MaxLength(500)]
        public string Note { get; protected set; }

        public string ConsecutiveNumber { get; set; }

        public byte[] QRCode { get; set; }

        public virtual Register Register { get; protected set; }

        public Guid? RegisterId { get; protected set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal SubTotal {
            get
            {
                return NetaSale;
            }
            protected set { }
        }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal DiscountPercentaje { get; protected set; }

        public DateTime? PaymentDate { get; protected set; }

        public string Transaction { get; protected set; }

        public string TenantName { get; set; }

        public string ComercialName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal DiscountAmount { get; protected set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalTax { get; protected set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Total { get; protected set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Balance { get; protected set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DueDate { get; protected set; }

        public Status Status { get; protected set; }

        public Guid? ClientServiceId { get; protected set; }

        public virtual ClientService ClientService { get; protected set; }

        public Guid? ClientId { get; protected set; }
        public virtual Client Client { get; set; }
        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }

        public Guid? UserId { get; protected set; }
        public string UserName { get; protected set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }


        public virtual IList<InvoiceLineDto> InvoiceLines { get;  set; }

        public virtual IList<PaymentInvoice> PaymentInvoices { get; protected set; }

        public IList<ListItems> ListItems { get; set; }

        public virtual IList<Note> Notes { get; protected set; }

        public virtual IList<InvoiceHistoryStatus> InvoiceHistoryStatuses { get; protected set; }

        public Guid ServiceId { get; set; }

        public string ServiceName { get; set; }

        public decimal SaleTotal { get; set; }

        public FirmType? TipoFirma { get; set; }

        public StatusFirmaDigital? StatusFirmaDigital { get; set; }

        public bool SendInvoice { get; set; }

        public TypeDocumentInvoice TypeDocument { get; set; } = TypeDocumentInvoice.Invoice;


        public string ClientName { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Numero de Referencia Externa (Usuarios del Api).
        /// </summary>
        /// <value>
        /// Numero de Referencia Externa (Usuarios del Api).
        /// </value>
        public string ExternalReferenceNumber { get; set; }

        public XSD.IdentificacionTypeTipo ClientIdentificationType { get; set; }
        /// <summary>
        /// Gets or sets the identification of your client. 
        /// </summary>

        [StringLength(20)]
        public string ClientIdentification { get; set; }

        public string ClientAddress { get; set; }

        public string ClientPhoneNumber { get; set; }

        public string ClientMobilNumber { get; set; }

        public string ClientEmail { get; set; }

        public string VoucherKey { get; set; }

        /// <summary>
        /// Obtiene o Almacena si la factura o tiquete es realizado en sustitución de un comprobante provisional.
        /// </summary>
        /// <value>Indica si la factura o tiquete es realizado en sustitución de un comprobante provisional</value>
        public bool IsContingency { get; set; } = false;

        /// <summary>
        /// Obtiene o Almacena el número consecutivo del comprobante provisional.
        /// </summary>
        /// <value>Número consecutivo del comprobante provisional</value>        
        [MaxLength(50)]
        public string ConsecutiveNumberContingency { get; set; }

        /// <summary>
        /// Obtiene o Almacena el motivo del comprobante provisional.
        /// </summary>
        /// <value>Motivo del comprobante provisional</value>
        [MaxLength(180)]
        public string ReasonContingency { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha del comprobante provisional.
        /// </summary>
        /// <value>Fecha del comprobante provisional</value>
        public DateTime? DateContingency { get; set; }

        public decimal TotalServGravados { get; set; }
        public decimal TotalServExento { get; set; }
        public decimal TotalProductExento { get; set; }
        public decimal TotalProductGravado { get; set; }
        public decimal TotalGravado { get; set; }
        public decimal TotalExento { get; set; }
        public decimal NetaSale { get; set; }
    }

    public class DetailItems
    {
        public DetailItems(IList<TicoPay.Invoices.Dto.ListItems> _Items, decimal _DiscountAmount)
        {
            Items = _Items;
            DiscountAmount = _DiscountAmount;
        }

        public IList<TicoPay.Invoices.Dto.ListItems> Items { get; set; }

        public decimal DiscountAmount { get; set; }
    }

    public class ListItems
    {
        public virtual IList<Note> Notes { get; set; }

        public virtual IList<InvoiceLineDto> InvoiceLines { get; set; }

        public virtual IList<Invoice> Invoices { get; set; }

        public virtual IList<Payment> PaymentInvoicesReverse { get; set; }

        public FacturaElectronicaResumenFacturaCodigoMoneda CodigoMoneda { get; set; }

        public Status Status { get; set; }

        public TypeDocumentInvoice? TypeDocument { get; set; }

        public decimal Balance { get; set; }

        public decimal Rate { get; set; }

        public Guid? ClientId { get; set; }
        public decimal SaleTotal { get; set; }

        public string VoucherKey { get; set; }

        public ListItems()
        {
            Notes = new List<Note>();
            InvoiceLines = new List<InvoiceLineDto>();
            Invoices = new List<Invoice>();
            PaymentInvoicesReverse = new List<Payment>();
        }
    }
}
