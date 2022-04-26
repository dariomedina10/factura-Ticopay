using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.BranchOffices.Dto;
using TicoPay.Invoices.XSD;



namespace TicoPay.Imports.Dto
{
    [AutoMapTo(typeof(BranchOfficesDto))]
    public class ImportBranchOfficesDto
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Location { get; set; }

        public string Estado { get; set; }

    }
}
