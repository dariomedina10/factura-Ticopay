using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;

namespace TicoPay.Users.Dto
{
    [AutoMap(typeof(User))]
    public class UpdateProfileInput
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LastLogin { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "La confirmación de la contraseña no coincide")]
        [DisableAuditing]
        public string ConfirmedPassword { get; set; }

        [Required]
        [DisableAuditing]
        public string Password { get; set; }

        public int? TenantID { get; set; }

        public string TenancyName { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public string EditionName { get; set; }

        public string EditionInvoicesMonthlyLimit { get; set; }

        public string EditionUsersLimit { get; set; }

        public int InvoicesInMonth { get; set; }

        public int UsersCount { get; set; }
    }
}
