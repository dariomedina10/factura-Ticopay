using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Groups.Dto;

namespace TicoPay.Clients.Dto
{
    /// <summary>
    /// Clase que almacena los Grupos o Categorías de un Cliente / Contains the Client categories to be applied to a Client
    /// </summary>
    public class ClientGroupsDto
    {
        /// <summary>
        /// Obtiene o Almacena el Id de un Cliente / Gets or Sets a Client Id.
        /// </summary>
        /// <value>
        /// Id de un Cliente / Client Id.
        /// </value>
        [Required]
        public Guid ClientId { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Lista de Grupos de un cliente / Gets or Sets the Client Categories list.
        /// </summary>
        /// <value>
        /// Lista de Grupos de un cliente / Client Categories List.
        /// </value>
        [Required]
        public IList<GroupDto> Groups { get; set; }
    }
}
