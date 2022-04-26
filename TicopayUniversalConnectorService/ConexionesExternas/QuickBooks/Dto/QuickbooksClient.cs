using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPayDll.Clients;

namespace TicopayUniversalConnectorService.ConexionesExternas.QuickBooks.Dto
{
    public class QuickbooksClient
    {
        public string Name { get; set; }
                
        public string LastName { get; set; }

        public IdentificacionTypeTipo IdentificationType { get; set; }

        public string Identification { get; set; }

        public string IdentificacionExtranjero { get; set; }

        public string NameComercial { get; set; }

        public string PhoneNumber { get; set; }

        public string MobilNumber { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public string WebSite { get; set; }

        public string PostalCode { get; set; }

        public string Address { get; set; }

        public string ContactName { get; set; }

        public string ContactMobilNumber { get; set; }

        public string ContactPhoneNumber { get; set; }

        public string ContactEmail { get; set; }

        public int CreditDays { get; set; }
    }
}
