using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices.XSD;

namespace TicoPay.Services.Dto
{
    public class BaseServiceDTO
    {
        public List<System.Web.Mvc.SelectListItem> UnitMeasurements
        {
            get
            {
                var list = new List<System.Web.Mvc.SelectListItem>();
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.Servicios_Profesionales).ToString(), Text = "Servicios Profesionales" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.Unid).ToString(), Text = "Unidad" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.Metro).ToString(), Text = "Metro" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.m2).ToString(), Text = "Metro Cuadrado" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.m3).ToString(), Text = "Metro Cubico" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.Kilogramo).ToString(), Text = "Kilogramo" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.g).ToString(), Text = "Gramo" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.L).ToString(), Text = "Litro" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.min).ToString(), Text = "Minuto" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.Segundo).ToString(), Text = "Segundo" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.h).ToString(), Text = "Hora" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.d).ToString(), Text = "Dia" });
                list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)UnidadMedidaType.Otros).ToString(), Text = "Otros" });
                return list;
            }
        }
    }
}
