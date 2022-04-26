using Abp.AutoMapper;
using TicoPay.General;

namespace TicoPay.Address.Dto
{
    /// <summary>
    /// Clase que contiene la información de los Distritos / Class that contains the District information
    /// </summary>
    [AutoMapFrom(typeof(Distrito))]
    public class DistritoDto
    {
        /// <summary>
        /// Obtiene o Almacena el Id del Distrito / Gets the District Id.
        /// </summary>
        /// <value>
        /// Id de Distrito / District Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre del Distrito / Gets the District Name.
        /// </summary>
        /// <value>
        /// Nombre Distrito / District Name.
        /// </value>
        public string NombreDistrito { get; set; }

        /// <summary>
        /// Obtiene o Almacena el código del Distrito según Hacienda / Gets the Hacienda District Code .
        /// </summary>
        /// <value>
        /// Código del Distrito / District Code.
        /// </value>
        public string codigodistrito { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Canton al cual pertenece el Distrito / Gets the Canton Id to which the District belongs.
        /// </summary>
        /// <value>
        /// Id de Canton al cual pertenece / Canton Id to which the District belongs.
        /// </value>
        public int CantonID { get; set; }
    }
}
