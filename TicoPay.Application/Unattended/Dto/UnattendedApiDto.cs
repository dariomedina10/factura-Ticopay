using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Unattended.Dto
{
    public class UnattendedApiDto
    {
        public bool SendInvoice { get; set; }

        public DateTime DueDate { get; set; }

        public string XmlContent { get; set; }

        public string PDF { get; set; }
    }
}
