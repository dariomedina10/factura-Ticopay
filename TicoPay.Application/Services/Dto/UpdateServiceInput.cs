using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using TicoPay.Common;
using TicoPay.Taxes;
using TicoPay.Invoices.XSD;
using Newtonsoft.Json;

namespace TicoPay.Services.Dto
{
    [AutoMapFrom(typeof(Service))]
    public class UpdateServiceInput : IDtoViewBaseFields
    {
        public Guid Id { get; set; }

        [MaxLength(Service.MaxNameLength)]
        [Display(Name = "Nombre: ")]
        public string Name { get; set; }

        [Display(Name = "Precio: ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [Range(1, 9999999999999.99, ErrorMessage = "El precio  debe estar entre 1 y 9999999999999.99")]
        public decimal Price { get; set; }

        [Display(Name = "Impuesto: ")]
        public Guid? TaxId { get; set; }

        public string CronExpression { get; set; }

        public Tax Tax { get; set; }

        public IList<Tax> Taxes { get; set; }

        [Display(Name = "Unidad de Medida: ")]
        public UnidadMedidaType UnitMeasurement { get; set; }

        [JsonIgnore]
        public List<System.Web.Mvc.SelectListItem> UnitMeasurements
        {
            get
            {
                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.Servicios_Profesionales).ToString(), Text = "Servicios Profesionales" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.m2).ToString(), Text = "Metro Cuadrado" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.m3).ToString(), Text = "Metro Cubico" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.min).ToString(), Text = "Minuto" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.Segundo).ToString(), Text = "Segundo" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.h).ToString(), Text = "Hora" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.d).ToString(), Text = "Dia" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.Otros).ToString(), Text = "Otros" });
                return list;
            }
        }

        [Display(Name = "Indique la descripción de la medida: ")]
        public string UnitMeasurementOthers { get; set; }

        [Display(Name = "Recurrente: ")]
        public bool IsRecurrent { get; set; }

        [Display(Name = "Cantidad: ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [Range(1, 99999, ErrorMessage = "La cantidad debe estar entre 1 y 99999")]
        public decimal Quantity { get; set; }

        [Display(Name = "Porcentaje de descuento: ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [Range(0, 100, ErrorMessage = "El porcentaje de descuento debe estar entre 0 y 100")]
        public decimal DiscountPercentage { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
