using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using TicoPay.Inventory;
using Abp.AutoMapper;
using System.Collections.Generic;
using TicoPay.Taxes;
using TicoPay.Taxes.Dto;
using Newtonsoft.Json;
using TicoPay.Invoices.XSD;
using TicoPay.Invoices;
using Newtonsoft.Json;
using TicoPay.Invoices;
using static TicoPay.Inventory.Product;

namespace TicoPay.Inventory.Dto
{
    /// <summary>
    /// Contiene los Datos de los productos a facturar / Contains the Product Information
    /// </summary>
    [AutoMapFrom(typeof(Product))]
    public class ProductDto: EntityDto<Guid>
    {
        /// <exclude />
        [JsonIgnore]
        public int TenantId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre del Producto / Gets or Sets the Product Name.
        /// </summary>
        /// <value>
        /// Nombre del Producto / Product Name.
        /// </value>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena una Nota sobre el Producto / Gets or Sets Details about the Product.
        /// </summary>
        /// <value>
        /// Nota sobre el Producto / Product Details.
        /// </value>
        public string Note { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Precio de Venta del Producto / Gets or Sets the Retail Price.
        /// </summary>
        /// <value>
        /// Precio de Venta del Producto / Retail Price.
        /// </value>
        [Required]
        public decimal RetailPrice { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Precio de Compra del Producto / Gets or Sets the Supply Price.
        /// </summary>
        /// <value>
        /// Precio de Compra del Producto / Supply Price.
        /// </value>
        public decimal SupplyPrice { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Estado del Producto (Activo o Inactivo) / Gets or Sets the Product State (Active or Inactive).
        /// </summary>
        /// <value>
        /// Estado del Producto / Product State.
        /// </value>
        public Estatus Estado { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Porcentaje de Ganancia sobre el Producto / Gets or Sets the Markup Percentage on the Product.
        /// </summary>
        /// <value>
        /// Porcentaje de Ganancia sobre el Producto / Markup Percentage on the Product.
        /// </value>
        public decimal Markup { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Existencia del Producto / Gets or Sets the Total of Units in stock.
        /// </summary>
        /// <value>
        /// Existencia del Producto / Total of Units in stock.
        /// </value>
        public int TotalInStock { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID del Impuesto a aplicar al Producto / Gets or Sets Tax Id of the Product.
        /// </summary>
        /// <value>
        /// ID del Impuesto a aplicar al Producto / Tax Id.
        /// </value>
        [Required]
        public Guid TaxId { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Unidad de Medida del Producto / Gets or Sets the Measurement unit.
        /// </summary>
        /// <value>
        /// Unidad de Medida del Producto / Measurement unit.
        /// </value>
        [Required]
        public UnidadMedidaType UnitMeasurement { get; set; }

        /// <summary>
        /// Obtiene el Impuesto del Producto / Gets or Sets the Product Tax.
        /// </summary>
        /// <value>
        /// Impuesto del Producto / Product Tax.
        /// </value>
        public virtual TaxDto Tax { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id de la Marca del Producto / Gets or Sets the Branch Id of the Product.
        /// </summary>
        /// <value>
        /// Id de la Marca del Producto.
        /// </value>
        [JsonIgnore]
        public Guid BrandId { get; set; }

        /// <summary>
        /// Obtiene la Marca del Producto / Product Branch.
        /// </summary>
        /// <value>
        /// Marca del Producto.
        /// </value>
        [JsonIgnore]
        public Brand Brand { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Proveedor del Producto / Gets or Sets the Product Supplier Id.
        /// </summary>
        /// <value>
        /// Id del Proveedor del Producto.
        /// </value>
        [JsonIgnore]
        public Guid SupplierId { get; set; }

        /// <summary>
        /// Obtiene el Proveedor del Producto / Gets or Sets the Supplier.
        /// </summary>
        /// <value>
        /// Proveedor del Producto.
        /// </value>
        [JsonIgnore]
        public Supplier Supplier { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Código de Producto para el Proveedor / Gets or Sets the Supplier Reference Code.
        /// </summary>
        /// <value>
        /// Código de Producto para el Proveedor / Supplier Reference Code.
        /// </value>
        public string SupplierCode { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Código de Cuenta Contable del Producto / Gets or Sets the Sales Account Code .
        /// </summary>
        /// <value>
        /// Código de Cuenta Contable del Producto / Sales Account Code.
        /// </value>
        public string SalesAccountCode { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Tipo de Producto / Gets or Sets the Product Type Id.
        /// </summary>
        /// <value>
        /// Id del Tipo de Producto.
        /// </value>
        [JsonIgnore]
        public Guid ProductTypeId { get; set; }

        /// <summary>
        /// Obtiene el Tipo de Producto / Gets the Product Type.
        /// </summary>
        /// <value>
        /// Tipo de Producto.
        /// </value>
        [JsonIgnore]
        public ProductType ProductType { get; set; }

        /// <summary>
        /// Obtiene o Almacena si el Producto puede ser Vendido / Gets or Sets if the Product can be Sold.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si el Producto puede ser Vendido.; Sino, <c>false</c> / <c>true</c> if the Product can be Sold , otherwise <c>false</c>.
        /// </value>
        public bool CanBeSold { get; set; }

        /// <summary>
        /// Obtiene o Almacena si el Producto tiene Variantes / Gets or Sets if the product has Variants.
        /// </summary>
        /// <value>
        ///   <c>true</c> si el Producto tiene Variantes; Sino, <c>false</c> / <c>true</c> if the product has variants; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool HasVariants { get; set; }

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
        public DateTime? LastModificationTime { get; set; }
        
        /// <exclude />
        [JsonIgnore]
        public long? LastModifierUserId { get; set; }

        /// <exclude />
        [JsonIgnore]
        public DateTime CreationTime { get; set; }

        /// <exclude />
        [JsonIgnore]
        public long? CreatorUserId { get; set; }

        /// <exclude />
        [JsonIgnore]
        public virtual ICollection<ProductTag> Tags { get; set; }

        /// <exclude />
        [JsonIgnore]
        public virtual ICollection<ProductVariant> Variants { get; set; }

        /// <exclude />
        [JsonIgnore]
        public LineType Tipo { get; set; } = LineType.Product;

        /// <exclude />
        [JsonIgnore]
        public int CanBeDeleted { get; set; }

    }
}
