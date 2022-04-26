using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using TicoPay.Common;
using TicoPay.Users;
using TicoPay.General;
using System.Collections.Generic;
using TicoPay.Invoices.XSD;
using Newtonsoft.Json;

namespace TicoPay.Taxes.Dto
{
    /// <summary>
    /// Clase que contiene Datos para Actualización del Impuesto / Contains the Tax information to be updated
    /// </summary>
    [AutoMapFrom(typeof(Tax))]
    public class UpdateTaxInput : IDtoViewBaseFields
    {
        /// <summary>
        /// Obtiene o Almacena el Id del Impuesto / Gets or Sets the Tax Id.
        /// </summary>
        /// <value>
        /// Id del Impuesto / Tax Id.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Tenant al que pertenece el Impuesto / Gets or Sets Tenant id.
        /// </summary>
        /// <value>
        /// Tenant al que pertenece el Impuesto.
        /// </value>
        [JsonIgnore]
        public int TenantId { get; set; }

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
        /// Obtiene o Almacena la Tasa del Impuesto (Porcentaje) / Gets or Sets the Tax Rate (Percentage).
        /// </summary>
        /// <value>
        /// Tasa del Impuesto (Porcentaje) / Tax Rate.
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
        public ImpuestoTypeCodigo TaxTypes { get;  set; }

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
        public DateTime CreationTime { get; set; }

        /// <exclude />
        [JsonIgnore]
        public Guid CreatorUserId { get; set; }

        /// <exclude />
        [JsonIgnore]
        public User CreatorUser { get; set; }

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
