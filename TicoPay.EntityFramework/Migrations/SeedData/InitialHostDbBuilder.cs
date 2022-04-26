using TicoPay.EntityFramework;
using EntityFramework.DynamicFilters;

namespace TicoPay.Migrations.SeedData
{
    public class InitialHostDbBuilder
    {
        private readonly TicoPayDbContext _context;

        public InitialHostDbBuilder(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            _context.DisableAllFilters();

            new DefaultEditionsCreator(_context).Create();            
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
        }
    }
}
