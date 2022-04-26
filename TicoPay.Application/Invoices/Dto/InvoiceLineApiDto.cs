using Abp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using TicoPay.Invoices.XSD;
using TicoPay.Services.Dto;

namespace TicoPay.Invoices.Dto
{
    [AutoMapFrom(typeof(InvoiceLine))]
    public class InvoiceLineApiDto
    {
        /// <summary>
        /// Obtiene el Id de Sub Dominio o Tenant que hizo la factura / Gets or sets the tenant Id that created the invoice.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public int TenantId { get; set; }

        /// <summary>
        /// Obtiene El Precio por Unidad / Gets the price per unit.
        /// </summary>
        /// <value>
        /// Precio por Unidad / The price per unit.
        /// </value>
        public decimal PricePerUnit { get; protected set; }

        /// <summary>
        /// Obtiene el Monto de Impuesto / Gets the tax amount.
        /// </summary>
        /// <value>
        /// Monto del Impuesto / The tax amount.
        /// </value>
        public decimal TaxAmount { get; protected set; }

        /// <summary>
        /// Obtiene el Total de la Linea / Gets the Line total.
        /// </summary>
        /// <value>
        /// Total / Total.
        /// </value>
        public decimal Total { get; protected set; }

        /// <summary>
        /// Obtiene el Porcentaje de Descuento de la Linea / Gets the discount percentage.
        /// </summary>
        /// <value>
        /// Porcentaje de Descuento / The discount percentage.
        /// </value>
        public decimal DiscountPercentage { get; protected set; }

        /// <summary>
        /// Obtiene La Nota de la Linea / Gets the Line Note.
        /// </summary>
        /// <value>
        /// Nota de la Linea / Line Note.
        /// </value>
        [MaxLength(200)]
        public string Note { get; protected set; }

        /// <summary>
        /// Obtiene la Descripción de la Linea / Gets the Line Description.
        /// </summary>
        /// <value>
        /// Descripción de la Linea / Line Description.
        /// </value>
        [MaxLength(50)]
        public string Title { get; protected set; }

        /// <summary>
        /// Obtiene la Cantidad de Items de la linea / Gets the quantity of items in the Line.
        /// </summary>
        /// <value>
        /// Cantidad / Quantity.
        /// </value>
        public decimal Quantity { get; protected set; }

        /// <summary>
        /// Obtiene el Id de la factura a la que pertenece la Linea / Gets the Invoice Id to which the Line belongs.
        /// </summary>
        /// <value>
        /// Id de la Factura / Invoice Id.
        /// </value>
        public Guid InvoiceId { get; protected set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Linea / Gets or sets the Line Type.
        /// </summary>
        /// <value>
        /// Tipo de Linea / Line Type.
        /// </value>
        public LineType LineType { get; protected set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Servicio a Facturar (Si esta Creado en Ticopays, Sino enviar en Null) / 
        /// Gets or Sets the Service Id (If you created the service in Ticopays , otherwise send it null).
        /// </summary>
        /// <value>
        /// Id del Servicio a Facturar / Service Id.
        /// </value>
        public Guid? ServiceId { get; protected set; }

        /// Obtiene o Almacena el Id del Producto a Facturar (Si esta Creado en Ticopays, Sino enviar en Null) /
        /// Gets or Sets the Product Id (If you created the Product in Ticopays, otherwise send it null).
        /// </summary>
        /// <value>
        /// Id del Producto a Facturar / Product Id.
        /// </value>
        public Guid? ProductId { get; protected set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Impuesto a Aplicar a la Linea / Gets or Sets the Tax Id of the item.
        /// </summary>
        /// <value>
        /// Id del Impuesto a Aplicar / Tax Id.
        /// </value>
        public Guid? TaxId { get; protected set; }

        /// <summary>
        /// Obtiene o almacena el numero del Linea del Item / Gets or sets the Line number.
        /// </summary>
        /// <value>
        /// Numero de Linea / The line number.
        /// </value>
        public int LineNumber { get; protected set; }

        [JsonIgnore]
        public CodigoTypeTipo CodeTypes { get; protected set; }

        /// <summary>
        /// Obtiene o Almacena una Descripción del descuento sobre la Linea / Gets or Sets the Discount Description.
        /// </summary>
        /// <value>
        /// Nota adicional sobre Descuento de la Linea / Line Discount Description.
        /// </value>
        [MaxLength(20)]
        public string DescriptionDiscount { get; protected set; }

        /// <summary>
        /// Obtiene o Almacena el Sub Total de la Linea / Gets or Sets the Line Sub Total.
        /// </summary>
        /// <value>
        /// /Sub Total de la Linea / Line Sub Total.
        /// </value>
        public decimal SubTotal { get; protected set; }

        /// <summary>
        /// Obtiene o Almacena el Total de la Linea / Gets or Sets the Line Total.
        /// </summary>
        /// <value>
        /// Total de la Linea / Line Total.
        /// </value>
        public decimal LineTotal { get; protected set; }

        /// <exclude />
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        /// <exclude />
        [JsonIgnore]
        public long? DeleterUserId { get; set; }

        /// <exclude />
        [JsonIgnore]
        public DateTime? DeletionTime { get; set; }

        /// <exclude />
        [JsonIgnore]
        public Guid? ExonerationId { get; protected set; }

        /// <summary>
        /// Obtiene o Almacena el Servicio a Facturar (Si esta Creado en Ticopays, Sino regresa en Null) / 
        /// Gets or Sets the Service (If you created the service in Ticopays , otherwise comes null).
        /// </summary>
        /// <value>
        /// Id del Servicio a Facturar / Service Id.
        /// </value>
        public ServiceDto Service { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Unidad de Medida de la Linea / Gets or Sets the Line Measurement Unit.
        /// </summary>
        /// <value>
        /// Unidad de Medida de la Linea / Line Measurement Unit.
        /// </value>
        public UnidadMedidaType UnitMeasurement { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción de la Unidad de Medida en caso de ser Otra / Gets or Sets the Description of the Other Line Measurement Unit.
        /// </summary>
        /// <value>
        /// Descripción de la Unidad de Medida / Description of the Other Line Measurement Unit.
        /// </value>
        public string UnitMeasurementOthers { get; set; }
    }
}
