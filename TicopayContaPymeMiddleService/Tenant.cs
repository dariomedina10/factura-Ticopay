using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayContaPymeMiddleService
{
    public class Tenant
    {
        public Tenant()
        {
            Id = Guid.NewGuid();
            Documentos = new List<Operaciones>();
        }

        public Tenant(string tenantTicopay, string userTicopay,string passwordTicopay,string companyContaPyme)
        {
            Id = Guid.NewGuid();
            TenantTicopay = tenantTicopay;
            UserTicopay = userTicopay;
            PasswordTicopay = passwordTicopay;
            CompanyContaPyme = companyContaPyme;
            Documentos = new List<Operaciones>();
        }

        [Key]
        public Guid Id { get; set; }
        public string TenantTicopay { get; set; }
        public string UserTicopay { get; set; }
        public string PasswordTicopay { get; set; }
        public string CompanyContaPyme { get; set; }
        public virtual ICollection<Operaciones> Documentos { get; set; }
    }   
}
