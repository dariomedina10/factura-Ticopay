using System;
using System.Collections.Generic;
using System.Linq;
using Abp.AutoMapper;
using TicoPay.General;

namespace TicoPay.Address.Dto
{
    /// <summary>
    /// Clase que contiene la información del Barrio / Contains the Hood information
    /// </summary>
    [AutoMapFrom(typeof(Barrio))]
    public class BarrioDto
    {
        /// <summary>
        /// Obtiene o Almacena el Id del Barrio / Gets the Hood Id.
        /// </summary>
        /// <value>
        /// Id del Barrio / Hood Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre del Barrio / Gets the Hood Name.
        /// </summary>
        /// <value>
        /// Nombre del Barrio / Hood name.
        /// </value>
        public string NombreBarrio { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Código del Barrio según Hacienda / Hacienda Hood code.
        /// </summary>
        /// <value>
        /// Código del Barrio / Hood Code.
        /// </value>
        public string codigobarrio { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Distrito al cual pertenece el Barrio / Gets the District Id to which the Hood belongs.
        /// </summary>
        /// <value>
        /// Id de Distrito / District Id.
        /// </value>
        public int DistritoID { get; set; }
    }
}
