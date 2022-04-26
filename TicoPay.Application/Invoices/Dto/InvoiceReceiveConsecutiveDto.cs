using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Invoices.Dto
{
    public class InvoiceReceiveConsecutiveDto
    {
        /// <summary>
        /// Obtiene si la factura fue enviada a Hacienda.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si fue enviada a Hacienda; Sino, <c>false</c>.
        /// </value>
        public bool SendInvoice { get; set; }

        public DateTime DueDate { get; protected set; }

        public string XmlContent { get; set; }
    }
}
