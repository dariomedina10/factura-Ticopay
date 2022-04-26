using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Clients
{
    public class ContapymeApiClient
    {
        public string init { get; set; }
        public string ntercero { get; set; }
        public string napellido { get; set; }

        public ContapymeApiClient()
        {
            init = "";
            ntercero = "";
            napellido = "";
        }
    }
}
