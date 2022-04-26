using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TicoPay.MultiTenancy.Dto;
using Abp.Application.Editions;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using TicoPay.Editions;

namespace TicoPay.MultiTenancy
{
    public interface ITenantAppService : IApplicationService
    {
        ListResultDto<TenantListDto> GetTenants();
        Task<CreateTenantOutput> CreateTenant(CreateTenantInput input);
        UpdateTenantInput GetEdit(int id);
        List<TicoPayEdition> GetAllEditions();
        Task Update(UpdateTenantInput input);
        Task Update(TenantDto input);
        List<AgreementConectivity> GetTenantsByPort(int? port);
        TipoLLaveAcceso GetTenantTipoAcceso(List<int> listTenant);
        CertifiedTenantOutput GetCertifiedTenant(int id);
        Certificate GetCertified(int id);
        void SendPayNotificationIfNeeded(Tenant tenant, int daysOfGrace);
        bool HasPayFirstInvoice(string IdentificationNumber);
        UpdateTenantInput GetById(int id);
        TenantDto GetBy(Expression<Func<Tenant, bool>> predicate);
        void UpdateCostoSms(int id, decimal costoSms);
        bool validTenantClientIdentification(CreateTenantInput input);
        bool IsValidTenancyName(string tenancyName);
        bool IsTenancyNameTaken(string tenancyName);
        List<TicoPayEdition> GetAllTicoPayEditions();
        TicoPayEdition GetTenantTicopayEdition(int editionId);
        bool PassFirstNotificationDay(Tenant tenant);

        //string  Encriptar(string texto);
        //string  Desencriptar(string textoEncriptado);

        Task<bool> CheckTenantAPIPermission(string tenancyName);
        Task<bool> CheckTenantValidConfig(string tenancyName);
        bool IsClientExonerate(string IdentificationNumber);

        bool isDrawerEnabled(int tenatId, bool isBranchOffices);
    }
}
