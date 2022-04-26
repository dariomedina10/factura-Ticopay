using Abp.Application.Services.Dto;
using TicoPay.MultiTenancy.Dto;

namespace TicoPay.Api.Controllers
{
    public class TicoPayResponseAPI
    {
        public bool Success { get; internal set; }
        public string Message { get; internal set; }
        public int StatusCode { get; internal set; }
        public UpdateTenantInput Tenant { get; internal set; }
        public ListResultDto<TenantListDto> Tenants { get; internal set; }
    }
}