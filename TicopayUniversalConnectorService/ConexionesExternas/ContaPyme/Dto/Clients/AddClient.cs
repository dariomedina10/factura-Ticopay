using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Clients
{
    public class AddClient
    {
        public string init { get; set; }
        public ClientBasicInformation infobasica { get; set; }
        public string[] tipotercero { get; set; }

        public AddClient(TipoPersona Tipo)
        {
            infobasica = new ClientBasicInformation();
            init = "";
            tipotercero = new string[1] { "{\"codigo\": \"2\",\"base\": \"2\"}" };
            if (Tipo == TipoPersona.Natural)
            {
                infobasica.sclasiflegal = "1;";
            }
            if(Tipo == TipoPersona.Juridica)
            {
                infobasica.sclasiflegal = "2;";
            }
            if (Tipo == TipoPersona.Extranjero)
            {
                infobasica.sclasiflegal = "3;";
            }
            infobasica.bvisible = "T";
        }
    }

    public class ClientBasicInformation
    {
        public string ntercero { get; set; }
        public string napellido { get; set; }
        public string bempresa { get; set; } // F o T
        public string itddocum { get; set; } // 13 
        public string semail { get; set; }
        public string bvisible { get; set; }
        public string sclasiflegal { get; set; }

        public ClientBasicInformation()
        {
            ntercero = "";
            napellido = "";
            bempresa = "";
            itddocum = "";
            semail = "";
            bvisible = "";
            sclasiflegal = "";
        }
    }

    public enum TipoPersona
    {
        Natural,
        Juridica,
        Extranjero
    }
}
