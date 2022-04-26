using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using TicoPay.Clients;
using Newtonsoft.Json;

namespace TicoPay.Groups.Dto
{
    /// <summary>
    /// Contiene la información de los Grupos o Categorías de Clientes / Contains the information about the Client Categories
    /// </summary>
    [AutoMapFrom(typeof(Group))]
    public class GroupDto : EntityDto<Guid>
    {
        /// <summary>
        /// Obtiene o Almacena el Nombre de la Categoría / Gets or Sets the Category Name.
        /// </summary>
        /// <value>
        /// Nombre de la Categoría / Category Name.
        /// </value>
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Descripción de la Categoría / Gets or Sets the Category Description.
        /// </summary>
        /// <value>
        /// Descripción de la Categoría / Category Description.
        /// </value>
        [MaxLength(1024)]
        public string Description { get; set; }

        /// <exclude />
        [JsonIgnore]
        public long TenantId { get; set; }

    }
}
