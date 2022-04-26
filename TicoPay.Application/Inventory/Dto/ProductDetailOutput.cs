using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using TicoPay.Common;
using TicoPay.Taxes;
using TicoPay.Users;

namespace TicoPay.Inventory.Dto
{
    [AutoMapFrom(typeof(Product))]
    public class ProductDetailOutput : IDtoViewBaseFields
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the Name. 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the Name. 
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Gets or sets the retail price of the product
        /// </summary>
        [Display(Name = "Precio Venta: ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal RetailPrice { get; set; }
        /// <summary>
        /// Gets or sets the supply price of the product
        /// </summary>
        /// 
        [Display(Name = "Precio Compra: ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal SupplyPrice { get; set; }
        /// <summary>
        /// Gets or sets the supply price of the product
        /// </summary>
        [Display(Name = "Margen: ")]
        public decimal Markup { get; set; }
        /// <summary>
        /// Gets or sets the total in stock
        /// </summary>
        [Display(Name = "Inventario: ")]
        public int TotalInStock { get; set; }
        /// <summary>
        /// Gets or sets default tax id
        /// </summary>
        [Display(Name = "Impuesto: ")]
        public Guid TaxId { get; set; }
        /// <summary>
        /// Gets or sets default tax
        /// </summary>
        public Tax Tax { get; set; }
        /// <summary>
        /// Gets or sets brand Id
        /// </summary>
        [Display(Name = "Marca")]
        public Guid BrandId { get; set; }
        /// <summary>
        /// Gets or sets Brand
        /// </summary>
        public Brand Brand { get; set; }
        /// <summary>
        /// Gets or sets brand Id
        /// </summary>
        [Display(Name = "Proveedor")]
        public Guid SupplierId { get; set; }
        /// <summary>
        /// Gets or sets Brand
        /// </summary>
        public Supplier Supplier { get; set; }
        /// <summary>
        /// Gets or sets Supplier Code
        /// </summary>
        [Display(Name = "Cod. Proveedor")]
        public string SupplierCode { get; set; }
        /// <summary>
        /// Gets or sets Sales Account Code
        /// </summary>
        public string SalesAccountCode { get; set; }

        /// <summary>
        /// Gets or sets Product Type Id
        /// </summary>
        [Display(Name = "Tipo Producto")]
        public Guid ProductTypeId { get; set; }
        /// <summary>
        /// Gets or sets Product Type
        /// </summary>
        public ProductType ProductType { get; set; }
        /// <summary>
        /// Gets or sets can be sold
        /// </summary>
        [Display(Name = "Disp. Venta")]
        public bool CanBeSold { get; set; }


        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public long LastModifierUserId { get; set; }

        public long CreatorUserId { get; set; }

        public IList<User> Users { get; set; }

        public int? ErrorCode { get ; set ; }
        public string ErrorDescription { get ; set ; }
        public string Action { get ; set ; }
        public string Control { get ; set ; }
        public string Query { get ; set ; }
    }
}
