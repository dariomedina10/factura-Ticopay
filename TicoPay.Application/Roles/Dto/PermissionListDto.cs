using Abp.Authorization;
using Abp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Roles.Dto
{
    /// <summary>
    /// Clase que contiene los Permisos asignados a un usuario
    /// </summary>
    [AutoMapFrom(typeof(Permission))]
    public class PermissionListDto
    {
        /// <summary>
        /// Obtiene o Almacena la lista de Sub Permisos (Hojas) que forman parte del Permiso Raíz.
        /// </summary>
        /// <value>
        /// Sub-Permisos.
        /// </value>
        public ICollection<PermissionListDto> Children { get; set; }
        /// <summary>
        /// Obtiene o Asigna el nombre a mostrar del Permiso.
        /// </summary>
        /// <value>
        /// Nombre a mostrar del Permiso.
        /// </value>
        public string DisplayName { get; set; }
        /// <summary>
        /// Obtiene o Asigna el nombre real del Permiso.
        /// </summary>
        /// <value>
        /// Nombre real del Permiso.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena el Permiso Raíz del cual forma parte un Sub-Permiso.
        /// </summary>
        /// <value>
        /// Permiso Raíz.
        /// </value>
        public virtual PermissionListDto Parent { get; set; }
    }
}
