using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicoPay.Editions
{
    public class EditionManager : AbpEditionManager
    {
        public const string FreeEditionName = "Free";
        public const string ProfesionalEditionName = "Profesional";
        public const string PymeJrEditionName = "Pyme Jr";
        public const string Pyme1EditionName = "Pyme1";
        public const string Pyme2EditionName = "Pyme2";
        public const string BusinessEditionName = "Business";
        public const string ProfesionalJrEditionName = "Profesional Jr";

        public const string FreeEditionDisplayName = "Free";
        public const string ProfesionalEditionDisplayName = "Servicio Facturación Electrónica en Línea (Plan Profesional)";
        public const string PymeJrEditionDisplayName = "Servicio Facturación Electrónica en Línea (Plan Pyme Jr)";
        public const string Pyme1EditionDisplayName = "Servicio Facturación Electrónica en Línea (Plan Pyme1)";
        public const string Pyme2EditionDisplayName = "Servicio Facturación Electrónica en Línea (Plan Pyme2)";
        public const string BusinessEditionDisplayName = "Servicio Facturación Electrónica en Línea (Plan Bussines)";
        public const string ProfesionalJrEditionDisplayName = "Servicio Facturación Electrónica en Línea (Plan Profesional Jr)";

        public const string ProfesionalAnnualEditionName = "Profesional Anual";
        public const string PymeJrAnnualEditionName = "Pyme Jr Anual";
        public const string Pyme1AnnualEditionName = "Pyme1 Anual";
        public const string Pyme2AnnualEditionName = "Pyme2 Anual";
        public const string BusinessAnnualEditionName = "Business Anual";
        public const string ProfesionalJrAnnualEditionName = "Profesional Jr Anual";

        public const string ProfesionalAnnualEditionDisplayName = "Servicio Facturación Electrónica en Línea (Plan Profesional Anual)";
        public const string PymeJrAnnualEditionDisplayName = "Servicio Facturación Electrónica en Línea (Plan Pyme Jr Anual)";
        public const string Pyme1AnnualEditionDisplayName = "Servicio Facturación Electrónica en Línea (Plan Pyme1 Anual)";
        public const string Pyme2AnnualEditionDisplayName = "Servicio Facturación Electrónica en Línea (Plan Pyme2 Anual)";
        public const string BusinessAnnualEditionDisplayName = "Servicio Facturación Electrónica en Línea (Plan Bussines Anual)";
        public const string ProfesionalJrAnualEditionDisplayName = "Servicio Facturación Electrónica en Línea (Plan Profesional Jr Anual)";

        public EditionManager(IRepository<Edition> editionRepository, IAbpZeroFeatureValueStore featureValueStore) : base(editionRepository, featureValueStore)
        {
        }

        public List<TicoPayEdition> GetActiveEditions()
        {
            var query = GetActiveTicoPayEditions().Where(t => !t.CloseForSale);

            return query.ToList();
        }

        public List<TicoPayEdition> GetActiveTicoPayEditions()
        {
            var query = EditionRepository.GetAll().Where(t => t.IsDeleted == false);
            List<TicoPayEdition> result = new List<TicoPayEdition>();

            foreach (var item in query)
            {
                result.Add((TicoPayEdition)item);
            }

            return result;
        }

        public Edition GetEdition(int editionId)
        {
            return EditionRepository.Get(editionId);
        }

        public List<TicoPayEdition> GetEditionApi(int editionId)
        {
            var query = GetActiveTicoPayEditions().Where(t => t.Id == editionId);

            return query.ToList();
        }

        
        public async Task<TicoPayEdition> GetTicoPayEditionForSale(string planType)
        {
            List<Edition> query = await EditionRepository.GetAllListAsync(t => t.IsDeleted == false && t.Name == planType);
            List<TicoPayEdition> result = new List<TicoPayEdition>();

            foreach (var item in query)
            {
                result.Add((TicoPayEdition)item);
            }

            return result.Where(t => !t.CloseForSale).FirstOrDefault();
        }
    }
}
