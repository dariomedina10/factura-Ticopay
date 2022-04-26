using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.ConexionesExternas.ContaPyme.Responses
{
    public class GetAuthResult
    {
        public GetAuthResponse[] result { get; set; }
    }

    public class GetAuthResponse
    {
        public GetAuthHeader encabezado { get; set; }
        public GetAuthData respuesta { get; set; }
    }

    public class GetAuthHeader
    {
        public string resultado { get; set; } // Retorna true siempre que la petición se ejecute satisfactoriamente.
        public string imensaje { get; set; } // Código del mensaje de eventualidad o error en caso de presentarse.
        public string mensaje { get; set; } // Mensaje de eventualidad o error en caso de presentarse.
        public string tiempo { get; set; } // Tiempo que se tardó el Agente en resolver la petición, este tiempo está dado en milisegundos.

        // Posibles Errores / imensaje / mensaje
        // 0: Error en la aplicación, errores no controlados.
        // 1: Mensaje que le indica al usuario que debe corregir errores (errores controlados).
        // 10: No se ingresó un Json como parámetro.
        // Especificas
        // 1000: El nombre de usuario y/o contraseña son incorrectos.
        // 1001: No se ingresó el nombre de usuario y/o contraseña.
        // 1007: Ingrese el id de la aplicación "IAPP".
        // 1008: El código de la aplicación es incorrecto, informar de este error.
    }

    public class GetAuthData
    {
        public GetAuthBody datos;
    }

    public class GetAuthBody
    {
        public string keyagente { get; set; } // Identificador único que se asigna a cada usuario cuando se loguea en el Agente. Este identificador se debe utilizar en cada petición que se haga al agente después del logueo, es lo que corresponde al “controlkey”
        public string versión { get; set; } // Número de versión en la que se encuentra el sistema ContaPyme / AgroWin.
        public string release { get; set; } // Número de release en el que se encuentra el sistema ContaPyme / AgroWin.
        public string actualizacion { get; set; } // Número de la actualización en la que se encuentra el sistema ContaPyme / AgroWin.
        public string versionapiagente { get; set; } // Version del Agente del Api
    }
}
