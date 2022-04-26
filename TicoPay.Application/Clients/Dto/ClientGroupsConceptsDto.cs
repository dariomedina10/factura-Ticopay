using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.GroupConcept.Dto;

namespace TicoPay.Clients.Dto
{
    /// <summary>
    /// Clase que Almacena el ID de Cliente y la lista de Grupos de Conceptos a los que Pertenece / Contains the information of the Client and the Concept groups to assign
    /// </summary>
    public class ClientGroupsConceptsDto
    {
        /// <summary>
        /// Obtiene o Almacena el Id de Cliente / Gets or Sets the Client Id.
        /// </summary>
        /// <value>
        /// Id de Cliente / Client Id.
        /// </value>
        [Required]
        public Guid ClientId { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Lista de Grupos de Concepto a los que pertenece el Cliente / Gets or Sets the Invoice Concept Group list.
        /// </summary>
        /// <value>
        /// Lista de Grupos de Concepto / Invoice Concept Group List.
        /// </value>
        [Required]
        public IList<GroupConceptsDto> Groups { get; set; }
    }
}
