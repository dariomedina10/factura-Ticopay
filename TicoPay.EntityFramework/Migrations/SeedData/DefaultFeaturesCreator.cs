using Abp.Application.Features;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Editions;
using TicoPay.EntityFramework;

namespace TicoPay.Migrations.SeedData
{
    public class DefaultFeaturesCreator
    {
        public const string InvoicesMonthlyLimitFeatureName = "InvoicesMonthlyLimit";
        public const string UsersLimitFeatureName = "UsersLimit";

        public const string DrawerLimitFeatureName = "DrawerLimit";

        private readonly TicoPayDbContext _context;

        public DefaultFeaturesCreator(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateFeatures();
        }

        private void CreateFeatures()
        {
            var free = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.FreeEditionName);
            if (free != null)
            {
                UpdateFeatures(free, "10", "1");
            }
            var profesionalJr = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.ProfesionalJrEditionName);
            if (profesionalJr != null)
            {
                UpdateFeatures(profesionalJr, "10", "1");
            }
            var profesional = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.ProfesionalEditionName);
            if (profesional != null)
            {
                UpdateFeatures(profesional, "500", "2");
            }
            var pyme1 = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.Pyme1EditionName);
            if (pyme1 != null)
            {
                UpdateFeatures(pyme1, "1000", int.MaxValue.ToString());
            }
            var pyme2 = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.Pyme2EditionName);
            if (pyme2 != null)
            {
                UpdateFeatures(pyme2, "2000", int.MaxValue.ToString());
            }
            var empresarial = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.BusinessEditionName);
            if (empresarial != null)
            {
                UpdateFeatures(empresarial, int.MaxValue.ToString(), int.MaxValue.ToString());
            }

            _context.SaveChanges();
        }

        private void UpdateFeatures(Abp.Application.Editions.Edition edition, string invoicesMonthlyLimit, string usersLimit)
        {
            var features = _context.EditionFeatureSettings.Where(d => d.EditionId == edition.Id).ToList();
            if (features.Count > 2)
            {
                foreach (EditionFeatureSetting feature in features)
                {
                    _context.Entry(feature).State = EntityState.Deleted;
                    _context.SaveChanges();
                }

                _context.EditionFeatureSettings.Add(new EditionFeatureSetting(edition.Id, DefaultFeaturesCreator.InvoicesMonthlyLimitFeatureName, invoicesMonthlyLimit));
                _context.EditionFeatureSettings.Add(new EditionFeatureSetting(edition.Id, DefaultFeaturesCreator.UsersLimitFeatureName, usersLimit));
            }
            else
            {
                var featureInvoicesMonthly = _context.EditionFeatureSettings.Where(d => d.EditionId == edition.Id && d.Name == DefaultFeaturesCreator.InvoicesMonthlyLimitFeatureName).FirstOrDefault();
                featureInvoicesMonthly.Value = invoicesMonthlyLimit;
                _context.EditionFeatureSettings.Attach(featureInvoicesMonthly);
                var entry = _context.Entry(featureInvoicesMonthly);
                entry.Property(e => e.Value).IsModified = true;

                _context.SaveChanges();

                var featureUsersLimit = _context.EditionFeatureSettings.Where(d => d.EditionId == edition.Id && d.Name == DefaultFeaturesCreator.UsersLimitFeatureName).FirstOrDefault();
                featureUsersLimit.Value = usersLimit;
                _context.EditionFeatureSettings.Attach(featureUsersLimit);
                entry = _context.Entry(featureUsersLimit);
                entry.Property(e => e.Value).IsModified = true;

                _context.SaveChanges();
            }
        }
    }
}
