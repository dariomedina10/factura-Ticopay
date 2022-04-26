using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.MultiTenancy
{
    public class Certificate : Entity<int>
    {
        public virtual Tenant Tenant { get; protected set; }
        public int TenantID { get; set; }
        [Required]
        public byte[] CertifiedRoute { get; set; }
        [Required]
        public string Password { get; set; }
        public string FileName { get; set; }
        public bool Installed { get; set; }
    }
}
