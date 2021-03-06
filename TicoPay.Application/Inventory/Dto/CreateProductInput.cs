using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using TicoPay.Common;
using TicoPay.Taxes;
using TicoPay.Invoices.XSD;
using TicoPay.Services.Dto;
using static TicoPay.Inventory.Product;
using Newtonsoft.Json;
using System.Linq;

namespace TicoPay.Inventory.Dto
{
    [AutoMapFrom(typeof(Product))]
    public class CreateProductInput :IDtoViewBaseFields
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the Name. 
        /// </summary>
        [Display(Name = "Nombre: ")]
        public string Name { get; set; }
        ///// <summary>
        ///// Gets or sets the Name. 
        ///// </summary>
        //public string Note { get; set; }
        /// <summary>
        /// Gets or sets the retail price of the product
        /// </summary>
        [Display(Name = "Precio Venta: ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [Range(1, 9999999999999.99, ErrorMessage = "El precio  debe estar entre 1 y 9,999,999,999,999.99")]
        public decimal RetailPrice { get; set; }
        ///// <summary>
        ///// Gets or sets the supply price of the product
        ///// </summary>
        ///// 
        //[Display(Name = "Precio Compra: ")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        //public decimal SupplyPrice { get; set; }
        ///// <summary>
        ///// Gets or sets the supply price of the product
        ///// </summary>
        //[Display(Name ="Margen: ")]
        //public decimal Markup { get; set; }
        ///// <summary>
        ///// Gets or sets the total in stock
        ///// </summary>
        //[Display(Name="Inventario: ")]
        //public int TotalInStock { get; set; }
        /// <summary>
        /// Gets or sets default tax id
        /// </summary>
        [Display(Name = "Impuesto: ")]
        public Guid TaxId { get; set; }
        /// <summary>
        /// Gets or sets default tax
        /// </summary>
        public Tax Tax { get; set; }
        public IList<Tax> Taxes { get; set; }
        ///// <summary>
        ///// Gets or sets brand Id
        ///// </summary>
        //[Display(Name ="Marca")]
        //public Guid? BrandId { get; set; }
        ///// <summary>
        ///// Gets or sets Brand
        ///// </summary>
        //public List<Brand> Brand { get; set; }
        ///// <summary>
        ///// Gets or sets brand Id
        ///// </summary>
        //[Display(Name ="Proveedor")]
        //public Guid? SupplierId { get; set; }
        ///// <summary>
        ///// Gets or sets Brand
        ///// </summary>
        //public List<Supplier> Supplier { get; set; }
        ///// <summary>
        ///// Gets or sets Supplier Code
        ///// </summary>
        //[Display(Name ="Cod. Proveedor")]
        //public string SupplierCode { get; set; }
        ///// <summary>
        ///// Gets or sets Sales Account Code
        ///// </summary>
        //public string SalesAccountCode { get; set; }

        ///// <summary>
        ///// Gets or sets Product Type Id
        ///// </summary>
        //[Display(Name ="Tipo Producto")]
        //public Guid? ProductTypeId { get; set; }
        /// <summary>
        /// Gets or sets Product Type
        /// </summary>
        //public List<ProductType> ProductType { get; set; }
        [Display(Name = "Unidad de Medida: ")]
        public UnidadMedidaType? UnitMeasurement { get; set; }

        [JsonIgnore]
        public List<System.Web.Mvc.SelectListItem> UnitMeasurements
        {
            get
            {
                var list = new List<System.Web.Mvc.SelectListItem>();
                Array values = Enum.GetValues(typeof(UnidadMedidaType)).Cast<UnidadMedidaType>().Where(v => (int)v != 86).ToArray();
                foreach (UnidadMedidaType value in values)
                {
                    list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)value).ToString(), Text = Enum.GetName(typeof(UnidadMedidaType), value).Replace("_", " ") });
                }

                return list;
            }
        }
        [Display(Name = "Estatus: ")]
        public Estatus Estado { get; set; }

        [JsonIgnore]
        public List<System.Web.Mvc.SelectListItem> Estatus
        {
            get
            {
                List<System.Web.Mvc.SelectListItem> lista = new List<System.Web.Mvc.SelectListItem>();
                lista.Add(new System.Web.Mvc.SelectListItem { Value = Product.Estatus.Activo.ToString(), Text = "Activo" });
                lista.Add(new System.Web.Mvc.SelectListItem { Value = Product.Estatus.Inactivo.ToString(), Text = "Inactivo" });
                return lista;
            }
        }
        ///// <summary>
        ///// Gets or sets can be sold
        ///// </summary>
        //[Display(Name ="Disp. Venta")]
        //public bool CanBeSold { get; set; }

        public int? ErrorCode { get ; set ; }
        public string ErrorDescription { get; set; }
        public string Action { get ; set; }
        public string Control { get ; set ; }
        public string Query { get ; set ; }


        public ProductDto result { get; set; }

    }
}
