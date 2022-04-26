using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.ReportInvoicesSentToTribunet.Dto
{
    public class ReportInvoicesSentToTribunetClienteDto
    {
        public Guid Id { get; set; }
        public int Code { get; set; }
        public string Identification { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LastName { get; internal set; }
    }
}
