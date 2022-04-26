using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Clients
{
    public class GetClients
    {
        public ContapymeApiClient[] datos { get; set; }
        // public PaginationInformation paginacion { get; set; }
        // public string seguridaddedatos { get; set; }
    }

    public class PaginationInformation
    {
        public string totalpaginas { get; set; }
        public string totalregistros { get; set; }
    }
}
