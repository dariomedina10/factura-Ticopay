using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Responses
{
    public class ComplexOperationResponse
    {
        public ComplexOperationResult[] result { get; set; }
    }

    public class ComplexOperationResult
    {
        public ComplexOperationHeader encabezado { get; set; }
        public ComplexOperationData respuesta { get; set; }
    }

    public class ComplexOperationHeader
    {
        public string resultado { get; set; } // Retorna true siempre que la petición se ejecute satisfactoriamente.
        public string imensaje { get; set; } // Código del mensaje de eventualidad o error en caso de presentarse.
        public string mensaje { get; set; } // Mensaje de eventualidad o error en caso de presentarse.
        public string tiempo { get; set; } // Tiempo que se tardó el Agente en resolver la petición, este tiempo está dado en milisegundos.

        // Posibles Errores / imensaje / mensaje
        //0: Error en la aplicación, errores no controlados.
        //1: Mensaje que le indica al usuario que debe corregir errores (errores controlados).
        //40: Usuario no logueado.
    }

    public class ComplexOperationData
    {
        public object datos { get; set; }
    }
}
