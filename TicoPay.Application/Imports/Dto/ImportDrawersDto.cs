using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Drawers.Dto;
using TicoPay.Invoices.XSD;



namespace TicoPay.Imports.Dto
{
    [AutoMapTo(typeof(DrawerDto))]
    public class ImportDrawersDto
    {
        public string Description { get; set; }

        public string Code { get; set; }

        public string CodeBranchOffice { get; set; }

    }
}
