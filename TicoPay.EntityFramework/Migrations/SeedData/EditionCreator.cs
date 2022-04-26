using Abp.Application.Editions;
using Abp.Application.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Editions;
using TicoPay.EntityFramework;
using TicoPay.Services;

namespace TicoPay.Migrations.SeedData
{
    public class EditionCreator
    {
        private readonly TicoPayDbContext _context;
        private readonly TicoPayEdition _e;
        public const string CRON_MONTHLY = "0 0 0 1 * ?";
        public const string CRON_ANNUAL = "0 0 12 1 {0}/12 ? *";

        public EditionCreator(TicoPayDbContext context, string name, string displayName, int price, TicopayEditionType editionType, bool closeForSale = false)
        {
            _context = context;
            _context.Editions.Add(new TicoPayEdition { Name = name, DisplayName = displayName, Price = price, EditionType = editionType, CloseForSale = closeForSale });
            _context.SaveChanges();
            _e = (TicoPayEdition)context.Editions.Where(e => e.Name == name).OrderByDescending(e => e.Id).FirstOrDefault();
        }

        public void Create(string InvoicesMonthlyLimitFeatureName, string UsersLimitFeatureName, string cronExpr)
        {
            CreatePlan(InvoicesMonthlyLimitFeatureName, UsersLimitFeatureName, cronExpr);
        }

        private void CreatePlan(string InvoicesMonthlyLimitFeatureName, string UsersLimitFeatureName, string cronExpr)
        {
            _context.EditionFeatureSettings.Add(new EditionFeatureSetting(_e.Id, DefaultFeaturesCreator.InvoicesMonthlyLimitFeatureName, InvoicesMonthlyLimitFeatureName));
            _context.EditionFeatureSettings.Add(new EditionFeatureSetting(_e.Id, DefaultFeaturesCreator.UsersLimitFeatureName, UsersLimitFeatureName));

            var ticoPayTenant = _context.Tenants.FirstOrDefault(t => t.TenancyName == TicoPayTenantCreator.TicoPayTenantName);

            var tax = _context.Taxes.FirstOrDefault(t => t.TenantId == ticoPayTenant.Id && t.Name == "exento");

            var service = Service.Create(ticoPayTenant.Id, _e.DisplayName, _e.Price, cronExpr, Invoices.XSD.UnidadMedidaType.Otros, "Uso de Software", true, 1, 0);
            service.ChangeTax(tax);
            _context.Services.Add(service);

        }
    }
}
