using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TicoPay.Invoices.XSD;
using TicoPay.Services.Dto;
using TicoPay.Taxes;
using TicoPay.Taxes.Dto;

namespace TicoPay.Web.Models.Invoice
{
    public class LineDetailsViewModel
    {
        public IList<Tax> Taxes { get; set; }
        public IList<line> Lines { get; set; }
        public List<System.Web.Mvc.SelectListItem> UnitMeasurements
        {
            get
            {
                var list = new List<System.Web.Mvc.SelectListItem>();
                Array values = Enum.GetValues(typeof(UnidadMedidaType)).Cast<UnidadMedidaType>().ToArray();
                foreach (UnidadMedidaType value in values)
                {
                    list.Add(new System.Web.Mvc.SelectListItem { Value = ((int)value).ToString(), Text = Enum.GetName(typeof(UnidadMedidaType), value).Replace("_", " ") });
                }
                
                return list;
            }
        }
    }

    public class line {
               
        public decimal PricePerUnit { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public decimal DiscountPercentage { get; set; }     
        public string Note { get; set; }     
        public string Title { get; set; }
        public decimal Quantity { get; set; }     
        public int LineNumber { get; set; }     
        public decimal SubTotal { get; set; }
        public decimal LineTotal { get; set; }     
        public virtual TaxDto Tax { get; set; }
        public Guid? TaxId { get; set; }
        public UnidadMedidaType UnitMeasurement { get; set; }
        public string UnitMeasurementOthers { get; set; }
        public string DescriptionDiscount { get; set; }
    }
}