using Abp.AutoMapper;

using TicoPay.General;

namespace TicoPay.Address.Dto
{
    /// <summary>
    /// Clase que almacena la información de los Cantones / Class that contains the Canton information
    /// </summary>
    [AutoMapFrom(typeof(Canton))]
    public class CantonDto
    {
        /// <summary>
        /// Obtiene o Almacena el Id del Canton / Gets the Canton Id.
        /// </summary>
        /// <value>
        /// Id de Canton / Canton Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre del Canton / Gets the Canton name.
        /// </summary>
        /// <value>
        /// Nombre del Canton / Canton name.
        /// </value>
        public string NombreCanton { get; set; }

        /// <summary>
        /// Obtiene o Almacena el código según Hacienda del Canton / Gets the Hacienda Canton Code.
        /// </summary>
        /// <value>
        /// Código del Canton / Hacienda Canton Code.
        /// </value>
        public string codigocanton { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id de la Provincia a la que pertenece el Canton / Gets the Province Id to which the Canton belongs.
        /// </summary>
        /// <value>
        /// Id de Provincia / Province Id to which the Canton belongs.
        /// </value>
        public int ProvinciaID { get; set; }
    }
}
