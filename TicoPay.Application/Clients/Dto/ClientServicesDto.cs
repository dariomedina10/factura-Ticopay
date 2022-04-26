using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Services.Dto;

namespace TicoPay.Clients.Dto
{
    /// <summary>
    /// Almacena los Servicios recurrentes Asignados a un Cliente / Class that contains the Scheduled services of a Client
    /// </summary>
    public class ClientServicesDto
    {
        /// <summary>
        /// Obtiene o Almacena el Id de Cliente / Gets or Sets the Client ID.
        /// </summary>
        /// <value>
        /// Id de Cliente / Client ID.
        /// </value>
        [Required]
        public Guid ClientId { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Lista de Servicios Recurrentes / Gets or Sets the Scheduled Services List.
        /// </summary>
        /// <value>
        /// Lista de Servicios Recurrentes / Scheduled Services List.
        /// </value>
        [Required]
        public IList<ServiceDto> Services { get; set; }
    }
}
