using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace TicoPay.Users.Dto
{
    /// <summary>
    /// Clase que contiene los usuarios asignados a un tenant
    /// </summary>
    [AutoMapFrom(typeof(User))]
    public class UserListDto : EntityDto<long>
    {
        /// <summary>
        /// Obtiene o Almacena el nombre del Usuario.
        /// </summary>
        /// <value>
        /// Nombre del Usuario.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o Almacena el apellido del Usuario.
        /// </summary>
        /// <value>
        /// Apellido del Usuario.
        /// </value>
        public string Surname { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre del usuario a utilizar en el inicio de sesión.
        /// </summary>
        /// <value>
        /// Nombre de usuario para sesiones.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Obtiene o Almacena el nombre completo del Usuario.
        /// </summary>
        /// <value>
        /// Nombre completo del usuario.
        /// </value>
        public string FullName { get; set; }

        /// <summary>
        /// Obtiene o Almacena el correo electrónico del Usuario.
        /// </summary>
        /// <value>
        /// Correo electrónico del Usuario.
        /// </value>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Obtiene o Almacena la confirmación de que el correo de usuario es valido.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si el correo de usuario es valido; sino es, <c>false</c>.
        /// </value>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha y tiempo del ultimo inicio de sesión del usuario.
        /// </summary>
        /// <value>
        /// Ultimo inicio de sesión del usuario.
        /// </value>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// Obtiene o Almacena si el usuario esta Activo o no.
        /// </summary>
        /// <value>
        ///   <c>true</c> Si el usuario esta activo; sino, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Obtiene o Almacena la fecha y tiempo en la cual fue creado el usuario.
        /// </summary>
        /// <value>
        /// Fecha de creación del Usuario.
        /// </value>
        public DateTime CreationTime { get; set; }
    }
}