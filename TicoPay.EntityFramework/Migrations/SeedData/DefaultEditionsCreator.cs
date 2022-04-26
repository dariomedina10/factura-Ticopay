using System.Linq;
using Abp.Application.Editions;
using TicoPay.Editions;
using TicoPay.EntityFramework;

namespace TicoPay.Migrations.SeedData
{
    public class DefaultEditionsCreator
    {
        private readonly TicoPayDbContext _context;

        public DefaultEditionsCreator(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateEditions();
        }

        private void CreateEditions()
        {
            var free = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.FreeEditionName);
            if (free == null)
            {
                _context.Editions.Add(new Edition { Name = EditionManager.FreeEditionName, DisplayName = EditionManager.FreeEditionDisplayName });
            }
            var profesional = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.ProfesionalEditionName);
            if (profesional == null)
            {
                _context.Editions.Add(new Edition { Name = EditionManager.ProfesionalEditionName, DisplayName = EditionManager.ProfesionalEditionDisplayName });
            }
            var pyme1 = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.Pyme1EditionName);
            if (pyme1 == null)
            {
                _context.Editions.Add(new Edition { Name = EditionManager.Pyme1EditionName, DisplayName = EditionManager.Pyme1EditionDisplayName });
            }
            var pyme2 = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.Pyme2EditionName);
            if (pyme2 == null)
            {
                _context.Editions.Add(new Edition { Name = EditionManager.Pyme2EditionName, DisplayName = EditionManager.Pyme2EditionDisplayName });
            }
            var empresarial = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.BusinessEditionName);
            if (empresarial == null)
            {
                _context.Editions.Add(new Edition { Name = EditionManager.BusinessEditionName, DisplayName = EditionManager.BusinessEditionDisplayName });
            }

            _context.SaveChanges();
        }
    }
}