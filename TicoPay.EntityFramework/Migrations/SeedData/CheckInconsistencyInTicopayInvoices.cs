using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.EntityFramework;
using TicoPay.Invoices;

namespace TicoPay.Migrations.SeedData
{
    public class CheckInconsistencyInTicopayInvoices
    {
        private readonly TicoPayDbContext _context;

        public CheckInconsistencyInTicopayInvoices(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Check()
        {
            InvoicesWithDifferentTenant();
        }

        private void InvoicesWithDifferentTenant()
        {
            var tenant = _context.Tenants.Where(d => d.Id == 2 && d.TenancyName == TicoPayTenantCreator.TicoPayTenantName).FirstOrDefault();

            #region Repara Facturas Con Clientes con misma Identificacion pero de otro tenant

            var invoices = _context.Invoices.Where(d => d.TenantId == 2 && d.Tenant.TenancyName == TicoPayTenantCreator.TicoPayTenantName && d.Client.TenantId != 2);

            foreach (var invoice in invoices)
            {
                var nroFactura = invoice.Number;

                var client = _context.Clients.Where(d => d.TenantId == tenant.Id && d.Identification == invoice.Client.Identification).FirstOrDefault();
                var clientService = _context.ClientServices.Where(d => d.TenantId == tenant.Id && d.ClientId == invoice.Client.Id && !d.IsDeleted).FirstOrDefault();

                clientService.SetClientId(client.Id);
                invoice.Client = client;

            }
            #endregion
        }
    }
}
