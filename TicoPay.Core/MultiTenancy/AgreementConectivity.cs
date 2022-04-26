using Abp.Domain.Entities;
using Abp.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Users;

namespace TicoPay.MultiTenancy
{
    public class AgreementConectivity : Entity<int>
    {
        public int TenantID { get; set; }

        //public string AgreementNumber { get; set; }

        public int AgreementNumbers { get; set; }

        public int Port { get; set; }

        public TipoLLaveAcceso KeyType { get; set; }
    }

    public enum TipoLLaveAcceso
    {
        Numero_Identificacion,
        Codigo_Cliente
    }
}
