using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using TicoPay.Common;

namespace TicoPay.Groups.Dto
{
    /// <summary>
    /// Clase que contiene los datos a Actualizar de los Grupos (Categorías) / Contains the Category information to update
    /// </summary>
    public class UpdateGroupInput : IDtoViewBaseFields
    {
        /// <summary>
        /// Obtiene o Almacena el Nombre del Grupo o Categoría / Gets or Sets the Name of the Category.
        /// </summary>
        /// <value>
        /// Nombre del Grupo o Categoría / Name of the Category.
        /// </value>
        [Required]
        [MaxLength(60)]
        [Display(Name = "Nombre: ")]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción del Grupo o Categoría / Gets or Sets the Category Description.
        /// </summary>
        /// <value>
        /// Descripción del Grupo o Categoría / Category Description.
        /// </value>
        [MaxLength(1024)]
        [Display(Name = "Descripción: ")]
        public string Description { get; set; }

        /// <exclude />
        [JsonIgnore]
        public int TenantId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Id del Grupo o Categoría / Gets the Category ID.
        /// </summary>
        /// <value>
        /// Id del Grupo o Categoría / Category Id.
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
    }
}
