using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TicoPay.Common;
using TicoPay.General;
using TicoPay.Invoices.XSD;

namespace TicoPay.Taxes.Dto
{
    /// <summary>
    /// Contiene los Datos para la Creación del Impuesto / Contains the Tax information
    /// </summary>
    /// <seealso cref="TicoPay.Common.IDtoViewBaseFields" />
    public class CreateTaxInput : IDtoViewBaseFields
    {
        /// <summary>
        /// Obtiene o Almacena el Nombre del Impuesto / Gets or Sets the Tax Name.
        /// </summary>
        /// <value>
        /// Nombre del Impuesto / Tax Name.
        /// </value>
        [Required]
        [Display(Name = "Nombre: ")]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Tasa del Impuesto / Gets or Sets the Tax Rate (Percentage).
        /// </summary>
        /// <value>
        /// Tasa del Impuesto / Tax Rate.
        /// </value>
        [Required]
        [Display(Name = "Porcentaje del impuesto: ")]
        public decimal Rate { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tipo de Impuesto / Gets or Sets the Tax Type.
        /// </summary>
        /// <value>
        /// Tipo de Impuesto / Tax Type.
        /// </value>
        [Required]
        [Display(Name = "Tipo de impuesto: ")]
        //[Range(0, 12, ErrorMessage = "El tipo de impuesto debe estar entre 0 y 12")]
        public ImpuestoTypeCodigo TaxTypes { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int? ErrorCode { get; set; }
        
        /// <exclude />
        [JsonIgnore]
        public string ErrorDescription { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string Action { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string Control { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string Query { get; set; }

        /// <exclude />
        [JsonIgnore]
        public List<System.Web.Mvc.SelectListItem> ListTaxTypes
        {
            get
            {
                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new System.Web.Mvc.SelectListItem { Value = ImpuestoTypeCodigo.Impuesto_Bebidas_Alcoholicas.ToString(), Text = "Impuesto Bebidas Alcoholicas" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ImpuestoTypeCodigo.Exento.ToString(), Text = "Exento" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ImpuestoTypeCodigo.Impuesto_Bebidas_Envasadas_sin_Alcohol_y_Jabones_de_tocador.ToString(), Text = "Impuesto Bebidas Envasadas sin Alcohol y Jabones de tocador" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ImpuestoTypeCodigo.Impuesto_General_Sobre_Ventas.ToString(), Text = "Impuesto General Sobre Ventas" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ImpuestoTypeCodigo.Impuesto_General_sobre_Ventas_Compras_Autorizadas.ToString(), Text = "Impuesto General sobre Ventas Compras Autorizadas" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ImpuestoTypeCodigo.Impuesto_General_sobre_Ventas_Diplomaticos.ToString(), Text = "Impuesto General sobre Ventas Diplomaticos" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ImpuestoTypeCodigo.Impuesto_General_sobre_Ventas_Instituciones_Publicas_y_Organismos.ToString(), Text = "Impuesto General sobre Ventas Instituciones Publicas y Organismos" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ImpuestoTypeCodigo.Impuesto_Productos_Tabaco.ToString(), Text = "Impuesto Productos Tabaco" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ImpuestoTypeCodigo.Impuesto_Selectivo_Consumo.ToString(), Text = "Impuesto Selectivo Consumo" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ImpuestoTypeCodigo.Impuesto_Selectivo_Consumo_Compras_Autorizadas.ToString(), Text = "Impuesto Selectivo Consumo Compras Autorizadas" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ImpuestoTypeCodigo.Impuesto_Unico_Combustibles.ToString(), Text = "Impuesto Unico Combustibles" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ImpuestoTypeCodigo.Otros.ToString(), Text = "Otros" });
                return list;
            }
        }
    }
}
