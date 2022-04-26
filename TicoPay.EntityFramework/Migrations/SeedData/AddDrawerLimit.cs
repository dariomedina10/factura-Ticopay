using Abp.Application.Features;
using System.Linq;
using TicoPay.Editions;
using TicoPay.EntityFramework;

namespace TicoPay.Migrations.SeedData
{
    class AddDrawerLimit
    {
        private readonly TicoPayDbContext _context;

        public AddDrawerLimit(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            var editions = _context.Editions.Where(a=> a.IsDeleted == false).ToList();

            foreach (var item in editions)
            {
                var featureDrawerLimit = _context.EditionFeatureSettings.Where(a => a.EditionId == item.Id && a.Name == DefaultFeaturesCreator.DrawerLimitFeatureName).ToList();
                if (featureDrawerLimit.Count == 0)
                {
                    var UserLimint = _context.EditionFeatureSettings.Where(a => a.EditionId == item.Id && a.Name == DefaultFeaturesCreator.UsersLimitFeatureName).Select(a => a.Value).FirstOrDefault();
                    _context.EditionFeatureSettings.Add(new EditionFeatureSetting(item.Id, DefaultFeaturesCreator.DrawerLimitFeatureName, UserLimint));
                }
            }

        }
    }
}
