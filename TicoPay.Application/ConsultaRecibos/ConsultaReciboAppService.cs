using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;

namespace TicoPay.ConsultaRecibos
{
    public class ConsultaReciboAppService: ApplicationService, IConsultaReciboAppService
    {

        private readonly IInvoiceManager _invoiceManager;
        private readonly TenantManager _tenantManager;

        public ConsultaReciboAppService(IInvoiceManager invoiceManager, TenantManager tenantManager)
        {
            _invoiceManager = invoiceManager;
            _tenantManager = tenantManager;
        }

        public IList<Tenant> GetAllTenants()
        {
            var tenants = _tenantManager.Tenants;
            return tenants.ToList();
        }
    }
}
