using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Dto.Security
{
    public class Parameters
    {
        public string[] _parameters;

        public Parameters(LoginJson credenciales)
        {
            _parameters = new string[4];
            _parameters[0] = JsonConvert.SerializeObject(credenciales.dataJSON);
            if (credenciales.controlkey == null)
            {
                _parameters[1] = "";
            } else
            {
                _parameters[1] = credenciales.controlkey;
            }           
            _parameters[2] = credenciales.iapp.ToString();
            _parameters[3] = credenciales.random.ToString();

        }
    }
}
