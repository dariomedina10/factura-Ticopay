using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TicoPay.Clients
{
    public class Group : AuditedEntity<Guid>, IMustHaveTenant, IFullAudited
    {
        /// <summary>
        /// Obtiene el Id del Tenant.
        /// </summary>
        [JsonIgnore]
        public int TenantId { get; set; }

        /// <summary>
        /// Obtiene el Nombre del Categoría / Gets the Category Name.
        /// </summary>
        /// <value>
        /// Nombre del Categoría / Category Name.
        /// </value>
        [MaxLength(60)]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene la Descripción de la Categoría / Gets the Category Description.
        /// </summary>
        /// <value>
        /// Descripción de la Categoría / Category Description.
        /// </value>
        [MaxLength(1024)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Clients List
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<ClientGroup> Groups { get; protected set; }

        /// <exclude />
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        /// <exclude />
        [JsonIgnore]
        public long? DeleterUserId { get; set; }

        /// <exclude />
        [JsonIgnore]
        public DateTime? DeletionTime { get; set; }

        /// <exclude />
        public static Group Create(int tenantId, string name, string description)
        {
            var @group = new Group
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = name,
                Description = description
            };
            return @group;
        }
    }
}
