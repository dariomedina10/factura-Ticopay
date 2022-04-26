using System;
using System.Collections.Generic;
using System.Text;

namespace TicoPayDll.Addresses.Dto
{
    public class Barrio
    {
        /// <summary>
        /// Obtiene o Almacena el Id del Barrio.
        /// </summary>
        /// <value>
        /// Id del Barrio.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre del Barrio.
        /// </summary>
        /// <value>
        /// Nombre del Barrio.
        /// </value>
        public string NombreBarrio { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Código del Barrio según Hacienda.
        /// </summary>
        /// <value>
        /// Código del Barrio.
        /// </value>
        public string codigobarrio { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Distrito al cual pertenece el Barrio.
        /// </summary>
        /// <value>
        /// Id de Distrito.
        /// </value>
        public int DistritoID { get; set; }
    }
}
