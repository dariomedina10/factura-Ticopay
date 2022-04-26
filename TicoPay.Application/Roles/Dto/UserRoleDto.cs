using Abp.Authorization.Users;
using Abp.AutoMapper;
using Newtonsoft.Json;

namespace TicoPay.Roles.Dto
{
    /// <summary>
    /// Clase que contiene los datos de los Roles asignados a un usuario / Class that contains the Role information
    /// </summary>
    [AutoMapFrom(typeof(UserRole))]
    public class UserRoleDto
    {
        /// <summary>
        /// Obtiene o Almacena el código de identificación del tenant al que el role pertenece / Gets the Tenant Id to which the Role belongs.
        /// </summary>
        /// <value>
        /// Código de identificación del Tenant / Tenant Id to which the Role belongs.
        /// </value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el código del usuario / Gets the User Id.
        /// </summary>
        /// <value>
        /// Código del Usuario / User Id.
        /// </value>
        public virtual long UserId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el código del Role / Gets the Role Id.
        /// </summary>
        /// <value>
        /// Código del Role / Role Id.
        /// </value>
        public virtual int RoleId { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Role asignado / Gets the Role.
        /// </summary>
        /// <value>
        /// Role.
        /// </value>
        public RolesDTO Role { get; set; }
    }
}
