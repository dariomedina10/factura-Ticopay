using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Inventory.Dto;
using TicoPay.Invoices.XSD;
using TicoPay.Services.Dto;


namespace TicoPay.Imports.Dto
{
    [AutoMapTo(typeof(ProductDto))]
    public class ImportProductDto
    {
        public string Name { get; set; }

        public decimal RetailPrice { get; set; }

        public string TaxName { get; set; }

        public UnidadMedidaType UnitMeasurement { get; set; }

        public string Estado { get; set; }

    }
}
