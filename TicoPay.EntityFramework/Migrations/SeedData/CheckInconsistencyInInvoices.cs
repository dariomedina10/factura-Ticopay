using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.EntityFramework;
using TicoPay.Invoices;

namespace TicoPay.Migrations.SeedData
{
    public class CheckInconsistencyInInvoices
    {
        private readonly TicoPayDbContext _context;

        public CheckInconsistencyInInvoices(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Check()
        {
            InvoicesWithoutInvoicesLines();
        }

        private void InvoicesWithoutInvoicesLines()
        {
            var tenants = _context.Tenants.Where(d => d.IsActive).ToList();

            foreach (var tenant in tenants)
            {
                #region Repara Facturas Sin Detalle Creado

                var invoices = _context.Invoices.Where(d => d.TenantId == tenant.Id && d.InvoiceLines.Count == 0);

                foreach (var invoice in invoices)
                {
                    if (invoice.TotalTax == 0)
                    {
                        var service = _context.Services.Where(d => d.TenantId == tenant.Id && d.Tax.Rate == 0).FirstOrDefault();
                        invoice.AssignInvoiceLine(tenant.Id, invoice.Total, 0, 0, "", "REPARADO POR CheckInconsistencyInInvoices", ((service.Name.Length > 50) ? service.Name.Substring(0, 49) : service.Name), 1, LineType.Service, service, null, invoice, 1, service.Tax, service.TaxId, service.UnitMeasurement,"");
                    }
                    else
                    {
                        int line = 0;
                        if (invoice.TotalGravado > 0)
                        {
                            var rate = Math.Round((invoice.TotalTax * 100) / invoice.TotalGravado, 2);
                            var service = _context.Services.Where(d => d.TenantId == tenant.Id && d.Tax.Rate == rate).FirstOrDefault();
                            invoice.AssignInvoiceLine(tenant.Id, invoice.TotalGravado, (invoice.TotalGravado * rate) / 100, 0, "", "Reparado por CheckInconsistencyInInvoices", ((service.Name.Length > 50) ? service.Name.Substring(0, 49) : service.Name), 1, LineType.Service, service, null, invoice, line++, service.Tax, service.TaxId, service.UnitMeasurement, "");
                        }

                        if (invoice.TotalExento > 0)
                        {
                            var service = _context.Services.Where(d => d.TenantId == tenant.Id && d.Tax.Rate == 0).FirstOrDefault();
                            invoice.AssignInvoiceLine(tenant.Id, invoice.TotalExento, 0, 0, "", "Reparado por CheckInconsistencyInInvoices", ((service.Name.Length > 50) ? service.Name.Substring(0, 49) : service.Name), 1, LineType.Service, service, null, invoice, line++, service.Tax, service.TaxId, service.UnitMeasurement, "");
                        }
                    }
                }

                _context.SaveChanges();

                #endregion
            }
        }
    }
}
