using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using TicoPay.Common;
using TicoPay.Services.Dto;

namespace TicoPay.GroupConcept.Dto
{
    /// <summary>
    /// Clase que Almacena los Datos de un Grupo de Conceptos para Facturación Recurrente / Contains the information of a Concept Group
    /// </summary>
    public class CreateGroupConceptsInput : IDtoViewBaseFields
    {
        /// <summary>
        /// Obtiene o Almacena el Nombre del Grupo de Conceptos / Gets or Sets the Concept Group Name.
        /// </summary>
        /// <value>
        /// Nombre del Grupo de Conceptos / Concept group Name.
        /// </value>
        [Required(ErrorMessage = "El campo nombre es requerido.")]
        [MaxLength(60)]
        [Display(Name = "Nombre: ")]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción del Grupo de Conceptos / Gets or Sets the Concept Group Description.
        /// </summary>
        /// <value>
        /// Descripción del Grupo de Conceptos / Concept Group Description.
        /// </value>
        [MaxLength(500)]
        [Display(Name = "Descripción: ")]
        public string Description { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int TenantId { get; set; }
        
        /// <summary>
        /// Obtiene o Almacena la Expresión Cron utilizada para la Planificación de la tarea (Debe ser Valida) / Gets or Sets the Cron Expression to be used in scheduled invoicing.
        /// </summary>
        /// <value>
        /// Expresión Cron utilizada para la Planificación de la tarea (Debe ser Valida).
        /// </value>
        [Display(Name = "Frecuencia de Facturación:")]
        public string CronExpression { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int? ErrorCode { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string ErrorDescription { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string Action { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string Control { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string Query { get; set; }

        /// <exclude />
        [JsonIgnore]
        public List<ServiceDto> AvailableServicios { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Lista de ID de Servicios asociada al Grupo de Conceptos / Gets or Sets the associated Service Id list .
        /// </summary>
        /// <value>
        /// Lista de ID de Servicios asociada al Grupo de Conceptos.
        /// </value>
        [Display(Name = "Servicios: ")]
        [Required(ErrorMessage = "Debe seleccionar al menos un servicio.")]
        public List<string> Services { get; set; }



        /// <exclude />
        public CreateGroupConceptsInput()
        {
            AvailableServicios = new List<ServiceDto>();
        }
    }
}
