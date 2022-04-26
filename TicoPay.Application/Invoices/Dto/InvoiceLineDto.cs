using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using TicoPay.Invoices.XSD;
using TicoPay.Services.Dto;
using TicoPay.Taxes;

namespace TicoPay.Invoices.Dto
{
    [AutoMapFrom(typeof(InvoiceLine))]
    public class InvoiceLineDto : EntityDto<Guid>
    {
        public int TenantId { get; set; }
        public decimal PricePerUnit { get; protected set; }
        public decimal TaxAmount { get; protected set; }
        public decimal Total { get; protected set; }
        public decimal DiscountPercentage { get; protected set; }
        [MaxLength(200)]
        public string Note { get; protected set; }
        [MaxLength(160)]
        public string Title { get; protected set; }
        public decimal Quantity { get; protected set; }
        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; protected set; }
        public LineType LineType { get; protected set; }
        public Guid? ServiceId { get; protected set; }
        public Guid? ProductId { get; protected set; }
        public int LineNumber { get; protected set; }
        public CodigoTypeTipo CodeTypes { get; protected set; }
        [MaxLength(80, ErrorMessage = "La descripción del descuento no puede tener más de 80 caracteres.")]
        public string DescriptionDiscount { get; protected set; }
        public decimal SubTotal { get; protected set; }
        public decimal LineTotal { get; protected set; }
        public bool IsDeleted { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public Guid? ExonerationId { get; protected set; }
        public ServiceDto Service { get; set; }
        public virtual Tax Tax { get; set; }
        public Guid? TaxId { get; set; }
        public UnidadMedidaType UnitMeasurement { get; set; }
        public string UnitMeasurementOthers { get; set; }

    }
}