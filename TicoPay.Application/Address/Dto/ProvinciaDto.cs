using Abp.AutoMapper;
using TicoPay.General;

namespace TicoPay.Address.Dto
{
    /// <summary>
    /// Clase que contiene la información de las provincias de Costa Rica / Class that contains the Province information
    /// </summary>
    [AutoMapFrom(typeof(Provincia))]
    public class ProvinciaDto
    {
        /// <summary>
        /// Obtiene o Almacena el código de identificación de la provincia / Gets the Province Id Code.
        /// </summary>
        /// <value>
        /// Código de identificación de Provincia / Province Id Code.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre de la Provincia / Gets the Province name.
        /// </summary>
        /// <value>
        /// Nombre de Provincia / Province name.
        /// </value>
        public string NombreProvincia { get; set; }
    }
}
