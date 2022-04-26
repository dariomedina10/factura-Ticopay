using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using System.Collections.Generic;
using TicoPay.Authorization.Roles;
using TicoPay.Drawers;
using TicoPay.BranchOffices;
using System;

namespace TicoPay.Users.Dto
{
    [AutoMap(typeof(UserStore))]
    public class UpdateUserInput 
    {
        public long Id { get; set; }

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

        public ICollection<DrawerUser> DrawerUsers { get; set; }

        public List<BranchOffice> BranchOffice { get; set; }

        public IList<Role> Roles { get; set; }

        public int IdRol { get; set; }

        public IList<Role> UserRoles { get; set; }       

        public bool IsActive { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }
    }

    public class UpdateUserDrawers
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<BranchOffice> BranchOffice { get; set; }

        public ICollection<DrawerUser> DrawerUsers { get; set; }

        public IList<listDrawer> ListDrawers { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }
    }

    public class listDrawer
    {
        public Guid Id { get; set; }
        public Guid IdDrawerUser { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }       
        public string Description { get; set; }
    }
}