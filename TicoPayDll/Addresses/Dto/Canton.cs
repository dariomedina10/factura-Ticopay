using System;
using System.Collections.Generic;
using System.Text;

namespace TicoPayDll.Addresses.Dto
{
    public class Canton
    {
        /// <summary>
        /// Obtiene o Almacena el Id del Canton.
        /// </summary>
        /// <value>
        /// Id de Canton.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre del Canton.
        /// </summary>
        /// <value>
        /// Nombre del Canton.
        /// </value>
        public string NombreCanton { get; set; }

        /// <summary>
        /// Obtiene o Almacena el código según Hacienda del Canton.
        /// </summary>
        /// <value>
        /// Código del Canton.
        /// </value>
        public string codigocanton { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id de la Provincia a la que pertenece el Canton.
        /// </summary>
        /// <value>
        /// Id de Provincia.
        /// </value>
        public int ProvinciaID { get; set; }
    }
}
