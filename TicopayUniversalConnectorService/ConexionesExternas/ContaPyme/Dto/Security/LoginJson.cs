using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Security
{
    public class LoginJson
    {
        public LoginInformation dataJSON { get; set; }
        public string controlkey { get; set; }
        public string iapp { get; set; }
        public string random { get; set; }
    }
}
