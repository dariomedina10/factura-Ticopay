using System;
using System.Collections.Generic;
using System.Text;

namespace TicoPayDll.Addresses.Dto
{
    public class Distrito
    {
        /// <summary>
        /// Obtiene o Almacena el Id del Distrito.
        /// </summary>
        /// <value>
        /// Id de Distrito.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre del Distrito.
        /// </summary>
        /// <value>
        /// Nombre Distrito.
        /// </value>
        public string NombreDistrito { get; set; }

        /// <summary>
        /// Obtiene o Almacena el código del Distrito según Hacienda.
        /// </summary>
        /// <value>
        /// Código del Distrito.
        /// </value>
        public string codigodistrito { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Canton al cual pertenece el Distrito.
        /// </summary>
        /// <value>
        /// Id de Canton al cual pertenece.
        /// </value>
        public int CantonID { get; set; }
    }
}
