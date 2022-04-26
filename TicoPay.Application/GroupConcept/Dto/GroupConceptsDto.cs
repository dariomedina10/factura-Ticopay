using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.Services.Dto;

namespace TicoPay.GroupConcept.Dto
{
    /// <summary>
    /// Clase que Almacena los Datos de un Grupo de Conceptos para Facturación Recurrente / Contains the Invoice Concept Groups information
    /// </summary>
    [AutoMapFrom(typeof(GroupConcepts))]
    public class GroupConceptsDto : EntityDto<Guid>
    {

        /// <summary>
        /// Obtiene o Almacena el ID del Grupo de Conceptos / Gets or Sets the Concept group Id.
        /// </summary>
        /// <value>
        /// ID del Grupo de Conceptos / Concept Group Id.
        /// </value>
        public Guid? IdDetails { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Nombre del Grupo de Conceptos / Gets or Sets the Concept Group Name.
        /// </summary>
        /// <value>
        /// Nombre del Grupo de Conceptos / Concept Group Name.
        /// </value>
        [MaxLength(60)]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción del Grupo de Conceptos / Gets or Sets the Concept Group Description.
        /// </summary>
        /// <value>
        /// Descripción del Grupo de Conceptos / Concept Group Description.
        /// </value>
        [MaxLength(500)]
        public string Description { get; set; }

        /// <exclude />
        [JsonIgnore]
        public long TenantId { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Lista de Servicios asociada al Grupo de Conceptos / Gets or Sets the list of services associated to the group.
        /// </summary>
        /// <value>
        /// Lista de Servicios asociada al Grupo de Conceptos / List of services associated to the group.
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ServiceDto> AvailableServicios { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Expresión Cron utilizada para la Planificación de la tarea (Debe ser Valida) / Gets or Sets the Cron expression to be used in the scheduled invoicing.
        /// </summary>
        /// <value>
        /// Expresión Cron utilizada para la Planificación de la tarea (Debe ser Valida) / Cron Expression to be used in the Scheduled invoicing.
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CronExpression { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string Type { get; set; } = "Group";

        /// <summary>
        /// Obtiene o Almacena la Cantidad del Grupo de Conceptos / Gets or Sets the Quantity of Concept groups to invoice.
        /// </summary>
        /// <value>
        /// Cantidad del Grupo de Conceptos / Quantity of concept groups.
        /// </value>
        [Range(0, 99999, ErrorMessage = "La cantidad debe estar entre 0 y 99999")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Porcentaje de Descuento del Grupo de Conceptos / Gets or Sets the Discount percentage of the concept group.
        /// </summary>
        /// <value>
        /// Porcentaje de Descuento del Grupo de Conceptos / Discount percentage of the concept group.
        /// </value>
        [Range(0, 100, ErrorMessage = "El porcentaje de descuento debe estar entre 0 y 100")]
        public decimal DiscountPercentage { get; set; } 
    }
}
