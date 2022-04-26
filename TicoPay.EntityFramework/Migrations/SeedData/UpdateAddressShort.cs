using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.EntityFramework;

namespace TicoPay.Migrations.SeedData
{
    public class UpdateAddressShort
    {
        private readonly TicoPayDbContext _context;

        public UpdateAddressShort(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            //TODO: se actualiza la dirección corta del tenant de ticopay            
            var tenant = _context.Tenants.Where(r => r.Id == 2 && r.AddressShort == null && r.IsAddressShort == false).FirstOrDefault();
            if (tenant != null)
            {
                tenant.IsAddressShort = true;
                tenant.AddressShort = "San Carlos, Alajuela - Costa Rica";
                _context.SaveChanges();
            }
            //End TODO


        }
    }
}
