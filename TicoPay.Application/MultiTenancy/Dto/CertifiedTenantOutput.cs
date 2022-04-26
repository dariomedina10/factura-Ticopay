using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.MultiTenancy.Dto
{
    public class CertifiedTenantOutput
    {
        public int  TenantID { get; set; }
        public byte[] CertifiedTenant { get; set; }
        public string Password { get; set; }
        public string FileName { get; set; }
    }
}
