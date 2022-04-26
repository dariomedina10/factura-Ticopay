using System;
using System.Collections.Generic;
using System.Text;

namespace TicoPayDll.Addresses.Dto
{
    public class Country
    {
        /// <summary>
        /// Obtiene o Almacena el nombre del País.
        /// </summary>
        /// <value>
        /// Nombre del País.
        /// </value>
        public string CountryName { get; set; }

        /// <summary>
        /// Obtiene o Almacena el código de País.
        /// </summary>
        /// <value>
        /// Código de País.
        /// </value>
        public string CountryCode { get; set; }

        /// <summary>
        /// Obtiene o Almacena el número de resolución donde aparece el País.
        /// </summary>
        /// <value>
        /// Número de Resolución.
        /// </value>
        public string ResolutionNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha de la resolución donde aparece el País.
        /// </summary>
        /// <value>
        /// Fecha de la resolución.
        /// </value>
        public DateTime ResolutionDate { get; set; }
    }
}
