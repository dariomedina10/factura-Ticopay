using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using AutoMapper;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TicoPay.Api.Common;
using TicoPay.BranchOffices;
using TicoPay.BranchOffices.Dto;
using TicoPay.Drawers;

namespace TicoPay.Api.Controllers
{
    /// <summary>
    /// Conjunto de Métodos que manejan la consulta de Sucursales / Methods to manage the Branch Offices
    /// </summary>
    /// <seealso cref="TicoPay.Api.Common.TicoPayApiController" />
    [AbpAuthorize]
    [Abp.Runtime.Validation.DisableValidation]
    public class BranchOfficeController : TicoPayApiController
    {
        private readonly IRepository<BranchOffice, Guid> _branchOfficesRepository;
        private readonly IDrawersAppService _drawerAppService;
        private readonly IBranchOfficesAppService _branchOfficeAppService;

        /// <exclude />
        public BranchOfficeController(
            IRepository<BranchOffice, Guid> branchOfficesRepository, IDrawersAppService drawerAppService, IBranchOfficesAppService branchOfficeAppService)
        {
            _drawerAppService = drawerAppService;
            _branchOfficesRepository = branchOfficesRepository;
            _branchOfficeAppService = branchOfficeAppService;
        }

        /// <summary>
        /// Obtiene todas las Sucursales del Sub Dominio / Gets all the Branch Offices of the Tenant.
        /// </summary>
        /// <remarks>
        /// Obtiene todas las Sucursales del Sub Dominio / Gets all the Branch Offices of the Tenant
        /// </remarks>
        /// <returns>Lista de Sucursales / List of Branch Offices</returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de Sucursales del usuario -> (listObjectResponse) / Returns the users Branch offices list in -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<BranchOfficesDto>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro / Return this message if an invalid security token wast sent or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Sucursales creadas / Returns this message when there are no Branch Offices assigned to the user", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay / Returns this message of an internal process failed in the Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetAll")]
        public IHttpActionResult GetAll()
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
                }
                var branches = _branchOfficeAppService.GetBranchOffices();

                if (branches == null || branches.Items.Count == 0)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                }
                else
                {
                    return Ok(new TicoPayResponseAPI<BranchOfficesDto>(HttpStatusCode.OK, null, branches));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }
    }
}
