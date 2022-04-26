using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Editions;
using TicoPay.EntityFramework;

namespace TicoPay.Migrations.SeedData
{
    public class PriceEditionCreator
    {
        private readonly TicoPayDbContext _context;
        public PriceEditionCreator(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create(bool force = false)
        {
            CheckEditionPrice(force);
        }

        private void CheckEditionPrice(bool force = false)
        {
            var legacyEdition = _context.Editions.Where(d => !d.IsDeleted).ToList();

            foreach (var edition in legacyEdition)
            {
                TicoPayEdition ticoPayEdition = (TicoPayEdition)edition;
                if (ticoPayEdition.Price == 0 || force)
                {
                    if (ticoPayEdition.Name == EditionManager.ProfesionalJrEditionName)
                    {
                        ticoPayEdition.Price = 4;
                    }
                    else if (ticoPayEdition.Name == EditionManager.ProfesionalEditionName)
                    {
                        ticoPayEdition.Price = 9;
                    }
                    else if (ticoPayEdition.Name == EditionManager.PymeJrEditionName)
                    {
                        ticoPayEdition.Price = 15;
                    }
                    else if (ticoPayEdition.Name == EditionManager.Pyme1EditionName)
                    {
                        ticoPayEdition.Price = 30;
                    }
                    else if (ticoPayEdition.Name == EditionManager.Pyme2EditionName)
                    {
                        ticoPayEdition.Price = 45;
                    }
                    else if (ticoPayEdition.Name == EditionManager.ProfesionalJrAnnualEditionName)
                    {
                        ticoPayEdition.Price = 48;
                    }
                    else if (ticoPayEdition.Name == EditionManager.ProfesionalAnnualEditionName)
                    {
                        ticoPayEdition.Price = 108;
                    }
                    else if (ticoPayEdition.Name == EditionManager.PymeJrAnnualEditionName)
                    {
                        ticoPayEdition.Price = 180;
                    }
                    else if (ticoPayEdition.Name == EditionManager.Pyme1AnnualEditionName)
                    {
                        ticoPayEdition.Price = 360;
                    }
                    else if (ticoPayEdition.Name == EditionManager.Pyme2AnnualEditionName)
                    {
                        ticoPayEdition.Price = 540;
                    }

                    _context.SaveChanges();
                }
            }
        }
    }
}
