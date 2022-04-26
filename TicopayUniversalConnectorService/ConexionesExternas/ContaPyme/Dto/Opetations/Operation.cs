using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Opetations
{
    public class Operation
    {
        public string[] _parameters;

        public Operation(OperationJson credenciales)
        {
            _parameters = new string[4];
            _parameters[0] = JsonConvert.SerializeObject(credenciales.dataJSON);
            if (credenciales.controlkey == null)
            {
                _parameters[1] = "";
            }
            else
            {
                _parameters[1] = credenciales.controlkey;
            }
            _parameters[2] = credenciales.iapp.ToString();
            _parameters[3] = credenciales.random.ToString();

        }

        public class OperationJson
        {
            public object dataJSON { get; set; }
            public string controlkey { get; set; }
            public string iapp { get; set; }
            public string random { get; set; }
        }

        public class RequestParameters
        {
            public PageData datospagina { get; set; }
            public string[] camposderetorno { get; set; }
        }

        public class PageData
        {
            public string cantidadregistros { get; set; }
            public string pagina { get; set; }
        }
    }
}
