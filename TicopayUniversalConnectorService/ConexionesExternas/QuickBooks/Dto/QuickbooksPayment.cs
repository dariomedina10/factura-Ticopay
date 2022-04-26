using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.QuickBooks.Dto
{
    public class QuickbooksPayment
    {
        public string PaymentId { get; set; }

        public string AffectedInvoiceId { get; set; }

        public string NumeroControl { get; set; }

        public string ClientId { get; set; }

        public List<QuickbooksPaymentInvoce> ListPaymentType { get; set; }
    }
}
