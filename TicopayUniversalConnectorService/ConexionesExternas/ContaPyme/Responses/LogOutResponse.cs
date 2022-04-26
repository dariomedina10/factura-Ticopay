using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Responses
{
    public class LogOutResponse
    {
        public LogOutResult[] result { get; set; }
    }

    public class LogOutResult
    {
        public LogOutHeader encabezado { get; set; }
        public LogOutData respuesta { get; set; }
    }

    public class LogOutHeader
    {
        public string resultado { get; set; } // Retorna true siempre que la petición se ejecute satisfactoriamente.
        public string imensaje { get; set; } // Código del mensaje de eventualidad o error en caso de presentarse.
        public string mensaje { get; set; } // Mensaje de eventualidad o error en caso de presentarse.

        // Posibles Errores / imensaje / mensaje
        //0: Error en la aplicación, errores no controlados.
        //1: Mensaje que le indica al usuario que debe corregir errores (errores controlados).
        //40: Usuario no logueado.
    }

    public class LogOutData
    {
        public LogOutBody datos;
    }

    public class LogOutBody
    {
        public string cerro { get; set; } // Retorna True cuando Cerro sesion , False si no cerro
    }
}
