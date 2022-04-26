using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Editions;
using TicoPay.EntityFramework;

namespace TicoPay.Migrations.SeedData
{
    public class TicopayTypeEditionEditor
    {
        private readonly TicoPayDbContext _context;
        public TicopayTypeEditionEditor(TicoPayDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CheckEditionType();
        }

        private void CheckEditionType()
        {
            var editions = _context.Editions.Where(d => !d.IsDeleted).ToList();

            foreach (var edition in editions)
            {
                TicoPayEdition ticoPayEdition = (TicoPayEdition)edition;
                if (ticoPayEdition.EditionType == null)
                {
                    if (ticoPayEdition.Name == EditionManager.ProfesionalJrEditionName)
                    {
                        ticoPayEdition.EditionType = TicopayEditionType.Monthly;
                    }
                    else if (ticoPayEdition.Name == EditionManager.PymeJrEditionName)
                    {
                        ticoPayEdition.EditionType = TicopayEditionType.Monthly;
                    }
                    else if (ticoPayEdition.Name == EditionManager.Pyme1EditionName)
                    {
                        ticoPayEdition.EditionType = TicopayEditionType.Monthly;
                    }
                    else if (ticoPayEdition.Name == EditionManager.Pyme2EditionName)
                    {
                        ticoPayEdition.EditionType = TicopayEditionType.Monthly;
                    }
                    else if (ticoPayEdition.Name == EditionManager.BusinessEditionName)
                    {
                        ticoPayEdition.EditionType = TicopayEditionType.Monthly;
                    }
                    else if (ticoPayEdition.Name == EditionManager.ProfesionalEditionName)
                    {
                        ticoPayEdition.EditionType = TicopayEditionType.Monthly;
                    }else if (ticoPayEdition.Name == EditionManager.ProfesionalJrAnnualEditionName)
                    {
                        ticoPayEdition.EditionType = TicopayEditionType.Annual;
                    }
                    else if (ticoPayEdition.Name == EditionManager.PymeJrAnnualEditionName)
                    {
                        ticoPayEdition.EditionType = TicopayEditionType.Annual;
                    }
                    else if (ticoPayEdition.Name == EditionManager.Pyme1AnnualEditionName)
                    {
                        ticoPayEdition.EditionType = TicopayEditionType.Annual;
                    }
                    else if (ticoPayEdition.Name == EditionManager.Pyme2AnnualEditionName)
                    {
                        ticoPayEdition.EditionType = TicopayEditionType.Annual;
                    }
                    else if (ticoPayEdition.Name == EditionManager.BusinessAnnualEditionName)
                    {
                        ticoPayEdition.EditionType = TicopayEditionType.Annual;
                    }
                    else if (ticoPayEdition.Name == EditionManager.ProfesionalAnnualEditionName)
                    {
                        ticoPayEdition.EditionType = TicopayEditionType.Annual;
                    }


                    _context.SaveChanges();
                }
            }
        }
    }
}
