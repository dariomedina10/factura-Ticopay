using System;
using System.Collections.Generic;
using System.Text;

namespace TicoPayDll.Addresses.Dto
{
    public class Provincia
    {
        /// <summary>
        /// Obtiene o Almacena el código de identificación de la provincia.
        /// </summary>
        /// <value>
        /// Código de identificación de Provincia.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre de la Provincia.
        /// </summary>
        /// <value>
        /// Nombre de Provincia.
        /// </value>
        public string NombreProvincia { get; set; }
    }
}
