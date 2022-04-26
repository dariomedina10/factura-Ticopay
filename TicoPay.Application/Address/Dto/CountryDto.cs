using Abp.AutoMapper;
using System;
using TicoPay.General;

namespace TicoPay.Address.Dto
{
    /// <summary>
    /// Clase que contiene la información de los Países Validos / Class that contains the Country information
    /// </summary>
    [AutoMapFrom(typeof(Country))]
    public class CountryDto
    {
        /// <summary>
        /// Obtiene o Almacena el nombre del País / Gets the Country name.
        /// </summary>
        /// <value>
        /// Nombre del País / Country name.
        /// </value>
        public string CountryName { get; set; }


        /// <summary>
        /// Obtiene o Almacena el código de País / Gets the country code.
        /// </summary>
        /// <value>
        /// Código de País / Country Code.
        /// </value>
        public string CountryCode { get; set; }

        /// <summary>
        /// Obtiene o Almacena el número de resolución donde aparece el País / Gets the Resolution number where the country was published.
        /// </summary>
        /// <value>
        /// Número de Resolución / Resolution number where the country was published.
        /// </value>
        public string ResolutionNumber { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha de la resolución donde aparece el País / Gets Resolution date where the country was published.
        /// </summary>
        /// <value>
        /// Fecha de la resolución / Resolution date where the country was published.
        /// </value>
        public DateTime ResolutionDate { get; set; }
    }
}
