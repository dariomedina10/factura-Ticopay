using System.Linq;
using TicoPay.EntityFramework;
using TicoPay.MultiTenancy;

namespace TicoPay.Migrations.SeedData
{
    public class DefaultTenantCreator
    {
        private readonly TicoPayDbContext _context;

        public DefaultTenantCreator(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateUserAndRoles();
        }

        private void CreateUserAndRoles()
        {
            //Default tenant

            var defaultTenant = _context.Tenants.FirstOrDefault(t => t.TenancyName == Tenant.DefaultTenantName);
            if (defaultTenant == null)
            {
                _context.Tenants.Add(new Tenant {TenancyName = Tenant.DefaultTenantName, Name = Tenant.DefaultTenantName});
                _context.SaveChanges();
            }
        }
    }
}
