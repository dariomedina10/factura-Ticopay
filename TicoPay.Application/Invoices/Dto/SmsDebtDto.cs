using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Invoices.Dto
{
    public class SmsDebtDto
    {
        public int TenantId { get; set; }
        public int SmsSenderId { get; set; }
        public decimal SmsCost { get; set; }
        public int SmsCount { get; set; }
    }
}
