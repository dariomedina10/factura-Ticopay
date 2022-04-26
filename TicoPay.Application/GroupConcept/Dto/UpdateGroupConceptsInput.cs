using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Common;
using TicoPay.Services.Dto;

namespace TicoPay.GroupConcept.Dto
{
    /// <summary>
    /// Clase que Almacena los Datos de un Grupo de Conceptos para Facturación Recurrente
    /// </summary>
    public class UpdateGroupConceptsInput : IDtoViewBaseFields, IAuditInfo
    {
        /// <summary>
        /// Obtiene o Almacena el Nombre del Grupo de Conceptos.
        /// </summary>
        /// <value>
        /// Nombre del Grupo de Conceptos.
        /// </value>
        [Required(ErrorMessage = "El campo nombre es requerido.")]
        [MaxLength(60)]
        [Display(Name = "Nombre: ")]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción del Grupo de Conceptos.
        /// </summary>
        /// <value>
        /// Descripción del Grupo de Conceptos.
        /// </value>
        [MaxLength(500)]
        [Display(Name = "Descripción: ")]
        public string Description { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int TenantId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el ID del Grupo de Conceptos.
        /// </summary>
        /// <value>
        /// ID del Grupo de Conceptos.
        /// </value>
        public Guid Id { get; set; }

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
        public string CreatorUserUserName { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string LastModifierUserName { get; set; }

        /// <exclude />
        [JsonIgnore]
        public string DeleterUserName { get; set; }

        /// <exclude />
        [JsonIgnore]
        public List<ServiceDto> AvailableServicios { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Lista de ID de Servicios asociada al Grupo de Conceptos.
        /// </summary>
        /// <value>
        /// Lista de ID de Servicios asociada al Grupo de Conceptos.
        /// </value>
        [Display(Name = "Servicios: ")]
        [Required(ErrorMessage = "Debe seleccionar al menos un servicio.")]
        public List<string> Services { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Expresión Cron utilizada para la Planificación de la tarea (Debe ser Valida).
        /// </summary>
        /// <value>
        /// Expresión Cron utilizada para la Planificación de la tarea (Debe ser Valida).
        /// </value>
        [Display(Name = "Frecuencia de Facturación:")]
        public string CronExpression { get; set; }

        /// <exclude />
        public UpdateGroupConceptsInput()
        {
            AvailableServicios = new List<ServiceDto>();
        }
    }
}
