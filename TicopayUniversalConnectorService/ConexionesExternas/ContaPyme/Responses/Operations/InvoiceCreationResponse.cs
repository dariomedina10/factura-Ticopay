using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Responses.Operations
{
    public class InvoiceCreationResponse
    {
        public InvoiceData datos { get; set; }
    }

    public class InvoiceData
    {
        public string iemp { get; set; }
        public string inumoper { get; set; }
        public string itdsop { get; set; }
        public string inumsop { get; set; }
        public string snumsop { get; set; }
        public string fsoport { get; set; }
        public string iclasifop { get; set; }
        public string imoneda { get; set; }
        public string banulada { get; set; }
        public string qoprsok { get; set; }
    }
}
