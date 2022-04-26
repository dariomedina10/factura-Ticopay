using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Invoices.XSD;
using TicoPay.Services.Dto;

namespace TicoPay.Imports.Dto
{
    [AutoMapTo(typeof(ServiceDto))]
    public class ImportServiceDto
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string TaxName { get; set; }

        public UnidadMedidaType UnitMeasurement { get; set; }

        public string UnitMeasurementOthers { get; set; }

        public bool IsRecurrent { get; set; }

        public decimal Quantity { get; set; }

        public decimal DiscountPercentage { get; set; }
    }
}
