using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Users;

namespace TicoPay.MultiTenancy.Dto
{
    public class CreateTenantOutput
    {
        public Tenant Tenant { get; set; }
        public User AdminUser { get; set; }
    }
}
