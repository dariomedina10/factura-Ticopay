using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using System.Collections.Generic;
using TicoPay.Authorization.Roles;

namespace TicoPay.Users.Dto
{
    [AutoMap(typeof(User))]
    public class CreateUserInput 
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(User.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(User.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(User.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "La confirmación de la contraseña no coincide")]
        [DisableAuditing]
        public string ConfirmedPassword { get; set; }

        public IList<Role> Roles { get; set; }

        public int IdRol { get; set; }

        public bool IsActive { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }
    }
}