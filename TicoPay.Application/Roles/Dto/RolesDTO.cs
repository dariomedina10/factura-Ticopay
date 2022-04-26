using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Authorization.Roles;

namespace TicoPay.Roles.Dto
{
    /// <summary>
    /// Clase que Define contenido de los Roles de seguridad / Class that contains the Role information
    /// </summary>
    [AutoMapFrom(typeof(Role))]
    public class RolesDTO : EntityDto<int>
    {
        /// <summary>
        /// Obtiene o Almacena el Nombre del Role / Gets the Role Name.
        /// </summary>
        /// <value>
        /// Nombre del Role / Role Name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha de creación del Role / Gets the Role creation date.
        /// </summary>
        /// <value>
        /// Fecha de creación del Role / Role creation date.
        /// </value>
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Obtiene o Almacena la Fecha de ultima modificación del Role / Gets the date of the last time the Role was edited.
        /// </summary>
        /// <value>
        /// Fecha de ultima modificación del Role / Date of the last time the Role was edited.
        /// </value>
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LastModificationTime { get; set; }
    }
}
