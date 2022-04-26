using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Invoices.Dto
{
    public class TicopayPDFDto
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
    }
}
