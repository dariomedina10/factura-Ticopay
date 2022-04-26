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
    [AutoMapFrom(typeof(Role))] // comentar 
    public class UpdateRoleInput 
    {
        public int Id { get; set; }

        [Display(Name = "Nombre: ")]
        public string Name { get; set; }

        [Display(Name = "Permisos: ")]
        public List<RecursiveObject> PermissionNames { get; set; }

        public object[] permisos { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        //bu
    }
}
