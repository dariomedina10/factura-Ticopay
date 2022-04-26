using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Inventory;
using TicoPay.Invoices.XSD;
using TicoPay.Services;
using TicoPay.Taxes;

namespace TicoPay.Invoices
{
    public class NoteLine : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {

        public int TenantId { get; set; }
        /// <summary>
        /// Gets or sets the Price per unit. 
        /// </summary>        
        public decimal PricePerUnit { get; protected set; }
        /// <summary>
        /// Gets or sets the TaxAmount. 
        /// </summary>
        public decimal TaxAmount { get; protected set; }

        /// <summary>
        /// Gets or sets the Total. 
        /// </summary>
        public decimal Total { get; protected set; }

        /// <summary>
        /// Gets or sets the Discount Percentage
        /// </summary>
        public decimal DiscountPercentage { get; protected set; }

        /// <summary>
        /// Gets or sets the Note. 
        /// </summary>
        [MaxLength(400)]
        public string Notes { get; protected set; }

        /// <summary>
        /// Gets or sets the Title. 
        /// </summary>
        [MaxLength(160)]
        public string Title { get; protected set; }

        /// <summary>
        /// Gets or sets the Quantity. 
        /// </summary>
        public decimal Quantity { get; protected set; }

        /// <summary>Gets or sets the InvoiceId. </summary>
        public Guid NoteId { get; protected set; }

        /// <summary>Gets or sets the Invoice. </summary>
        public Note Note { get; protected set; }

        /// <summary>
        /// Gets or sets the LineType. 
        /// </summary>
        public LineType LineType { get; protected set; }

        /// <summary>
        /// Gets or sets the service
        /// </summary>
        public virtual Service Service { get; set; }

        /// <summary>
        /// Foreign key to service id, it can be null 
        /// </summary>
        public Guid? ServiceId { get; protected set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public Product Product { get; protected set; }

        /// <summary>
        /// Foreign key to product id, it can be null 
        /// </summary>
        public Guid? ProductId { get; protected set; }

        public virtual Tax Tax { get; set; }

        public Guid? TaxId { get; set; }

        public UnidadMedidaType UnitMeasurement { get; set; }

        public string UnitMeasurementOthers { get; set; }
             
        /// <summary>
        /// Numero de linea de la factura
        /// </summary>
        public int LineNumber { get; protected set; }
        /// <summary>
        /// Tipo de codigo de Producto
        /// </summary>
        //[MaxLength(2)]
        //public string CodeType { get; protected set; }

        public CodigoTypeTipo CodeTypes { get; protected set; }
        /// <summary>
        /// Si tiene descuento se debe colocar una descripcion de la naturaleza
        /// </summary>
        [MaxLength(80, ErrorMessage = "La descripción del descuento no puede tener más de 80 caracteres.")]
        public string DescriptionDiscount { get; protected set; }
        /// <summary>
        /// se obtiene de restar monto total menos el monto descuento
        /// </summary>
        public decimal SubTotal { get; protected set; }
        /// <summary>
        /// se obtiene de la suma de subtotal mas monto impuesto
        /// </summary>
        public decimal LineTotal { get; protected set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public virtual Exoneration Exonerations { get; set; }

        /// <summary>
        /// Foreign key Exoneration
        /// </summary>
        public Guid? ExonerationId { get; protected set; }

        protected NoteLine()
        {
        }

        public static NoteLine Create(int tenantId, decimal pricePerUnit, string notes, string title, decimal qty, LineType ltype, Service service,
            Product product, Note note, int numberline, decimal discountAmount, string descriptionDiscount, CodigoTypeTipo _codeType, Tax tax, Guid? taxId,
            UnidadMedidaType unitMeasurement, string unitMeasurementOthers)
        {
            decimal precioUnit = Invoice.GetValor(pricePerUnit);
            decimal cantidad = Invoice.GetValor(qty);
            decimal descuento = Invoice.GetValor(discountAmount);
            decimal total = Invoice.GetValor(precioUnit * cantidad);
            var entity = new NoteLine
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                PricePerUnit = pricePerUnit,
                Total = total,
                Notes = notes,
                Title = title.Length > 160 ? title.Substring(0, 160) : title,
                Quantity = cantidad,
                LineType = ltype,
                Service = service,
                Product = product,
                Note = note,
                CodeTypes = _codeType,
                LineNumber = numberline,
                DiscountPercentage = descuento,
                DescriptionDiscount = descriptionDiscount,
                Tax = tax,
                TaxId = taxId,
                UnitMeasurement = unitMeasurement,
                UnitMeasurementOthers = unitMeasurementOthers
            };
            return entity;
        }

        public void SetLineTax(decimal taxAmount)
        {
            if (taxAmount > 0)
            {
                //decimal rate = taxAmount/100;
                //TaxAmount = rate * Total;
                //Total = Total + TaxAmount;
                TaxAmount = taxAmount;
            }
        }

        public void SetLineSubtotal(decimal dAmount, decimal total)
        {
            if (total > 0)
            {
                SubTotal = Invoice.GetValor(total - dAmount);
            }

        }

        public void SetLineLinetotal(decimal subTotal, decimal taxAmount)
        {
            if (subTotal > 0)
            {
                LineTotal = subTotal + taxAmount;
            }
        }
    }
}
