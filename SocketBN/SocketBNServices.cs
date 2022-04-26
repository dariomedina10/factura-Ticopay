using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TicoPay.MultiTenancy;

namespace TicoPay
{

    public class SocketBNServices : ITransientDependency
    {
        private readonly TenantManager _tenantManager;
        public SocketBNServices(TenantManager tenantManager)
        {
            _tenantManager = tenantManager;
        }

        public Tenant GetTenant()
        {
            var tenants = _tenantManager.GetActiveTenants();
            return tenants.FirstOrDefault();
        }
    }
}
