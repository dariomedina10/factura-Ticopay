using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Editions;
using TicoPay.EntityFramework;

namespace TicoPay.Migrations.SeedData
{
    public class UpdateFirmType
    {
        private readonly TicoPayDbContext _context;

        public UpdateFirmType(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            var tenants = _context.Tenants.Where(r => r.IsActive && !r.IsDeleted && r.Edition.Name != EditionManager.FreeEditionName && r.ValidateHacienda && r.TipoFirma == null).ToList();
            foreach (var tenant in tenants)
            {
                tenant.TipoFirma = MultiTenancy.Tenant.FirmType.Llave;
                tenant.FirmaRecurrente = MultiTenancy.Tenant.FirmType.Llave;
            }
            _context.SaveChanges();
        }
    }
}
