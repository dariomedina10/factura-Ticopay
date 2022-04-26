using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.WebApi.Controllers;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TicoPay.Application.Helpers;
using TicoPay.Editions;
using TicoPay.MultiTenancy;
using TicoPay.MultiTenancy.Dto;

namespace TicoPay.Api.Controllers
{
    //[RoutePrefix("api/tenant")]
    /// <summary>
    /// Conjunto de Métodos que manejan los Datos del Tenant o Sub Dominio
    /// </summary>
    /// <seealso cref="Abp.WebApi.Controllers.AbpApiController" />
    [Abp.Runtime.Validation.DisableValidation] //Deshabilita las validaciones que hace internamente boilerplate
    public class TenantController : AbpApiController
    {
        private readonly TenantManager _tenantManager;
        private readonly ITenantAppService _tenantAppService;

        //private readonly IRepository<AgreementConectivity, int> _agreementRepository;
        //private readonly IUnitOfWorkManager _unitOfWorkManager;
        //IRepository<Tenant> tenantRepository;
        //IRepository<TenantFeatureSetting, long> tenantFeatureRepository;
        //EditionManager editionManager;
        //IAbpZeroFeatureValueStore featureValueStore;

        /// <exclude />
        public TenantController(TenantManager tenantManager, ITenantAppService tenantAppService)
        {
            _tenantManager = tenantManager;
            _tenantAppService = tenantAppService;
        }

        //public TenantController()
        //{
        //    TenantController();
        //    //_tenantManager = new TenantManager(tenantRepository, tenantFeatureRepository, editionManager, featureValueStore, _agreementRepository, _unitOfWorkManager);
        //}

        /// <exclude />
        [ApiExplorerSettings(IgnoreApi = true)]
        [ResponseType(typeof(AgreementConectivity))]
        public IHttpActionResult GetTenantByPort(int port)
        {

            var convenio = _tenantManager.GetTenantsAgreementByPort(port);

            return Ok(new { results = convenio.FirstOrDefault() });
           // return convenio.FirstOrDefault();
        }

        /// <exclude />
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet]
        [Route("GetAll")]
        public IHttpActionResult GetAll(bool detallado)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            try {

                var tenants = (detallado) ? _tenantAppService.GetTenants() : _tenantAppService.GetTenants();

                return Ok(new TicoPayResponseAPI<TenantListDto>(HttpStatusCode.OK, null, tenants));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }

           
        }

        /// <summary>
        /// Obtiene los Datos del Tenant o Sub Dominio.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="detallado">if set to <c>true</c> [detallado].</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna los datos del Tenant, en -> (objectResponse)", Type = typeof(TicoPayResponseAPI<UpdateTenantInput>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los Datos del Tenant enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro)", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Datos del Tenant", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get(bool detallado)
        {
            int id = 0;
            try
            {
                if (AbpSession.TenantId != null)
                    id = (int)AbpSession.TenantId;
                else
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var tenant = (detallado) ? _tenantAppService.GetById(id) : _tenantAppService.GetById(id);
                tenant.Edition = _tenantAppService.GetTenantTicopayEdition(tenant.EditionId);

                if (tenant==null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else              
                  
                    return Ok(new TicoPayResponseAPI<UpdateTenantInput>(HttpStatusCode.OK, tenant));

               
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
           
        }

        /// <summary>
        /// Actualiza los datos del Tenant o Sub Dominio.
        /// </summary>
        /// <remarks>
        /// Actualiza los datos del Tenant o Sub Dominio.
        /// </remarks>
        /// <param name="input">Datos del Tenant Actualizados.</param>
        /// <returns></returns>
        //[SwaggerResponse(HttpStatusCode.OK, "OK : Retorna Null si los datos fueron Actualizados, en -> (objectResponse)", Type = typeof(TicoPayResponseAPI<UpdateTenantInput>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los Datos del Tenant enviados", Type = typeof(TicoPayResponseErrorAPI))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro)", Type = typeof(TicoPayResponseAPI<bool>))]        
        //[SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay", Type = typeof(TicoPayResponseErrorAPI))]
        //[Authorize]
        //[HttpPost]
        //[Route("Put")]
        //public async Task<IHttpActionResult> Put(TenantDto input)
        //{
        //    int id = 0;
        //    if (AbpSession.TenantId != null)
        //        id = (int)AbpSession.TenantId;
        //    else
        //        return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

        //    ModelState.Clear();
        //    if (!ModelState.IsValid)
        //        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST,ModelState.ToErrorMessage()));

        //    if(input.Id != (int)AbpSession.TenantId)
        //        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));
        //    try
        //    {
        //        input.Id = id;
        //        await  _tenantAppService.Update(input);
        //        return Ok(new TicoPayResponseAPI<UpdateTenantInput>(HttpStatusCode.OK,null));
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Content(HttpStatusCode.InternalServerError, new  TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

        //    }
        //}

       

        //public TicoPayResponseAPI Tenants(bool detallado)
        //{
        //    var tenants = (detallado) ? null : _tenantAppService.GetTenants();

        //    return ApiResponse(HttpStatusCode.OK, tenants: tenants);
        //}

        //private TicoPayResponseAPI ApiResponse(HttpStatusCode statusCode, string message = null, UpdateTenantInput tenant = null, ListResultDto<TenantListDto> tenants = null)
        //{
        //    if (string.IsNullOrWhiteSpace(message) && statusCode != HttpStatusCode.Accepted && statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.Created)
        //    {
        //        message = statusCode.ToString();
        //    }
        //    return new TicoPayResponseAPI
        //    {
        //        Success = statusCode == HttpStatusCode.Accepted || statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created,
        //        Message = message,
        //        StatusCode = (int)statusCode,
        //        Tenant = tenant,
        //        Tenants = tenants,
        //    };
        //}
    }
}
