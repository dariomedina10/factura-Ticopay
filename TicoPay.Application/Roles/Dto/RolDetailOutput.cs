using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicoPay.Authorization.Roles;
using TicoPay.Common;
using TicoPay.Users;

namespace TicoPay.Roles.Dto
{
    [AutoMapFrom(typeof(Role))]
    public class RolDetailOutput : IDtoViewBaseFields
    {
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastModificationTime { get; set; }

        public int LastModifierUserId { get; set; }

        public int CreatorUserId { get; set; }

        public IList<User> Users { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Query { get; set; }
    }
}
