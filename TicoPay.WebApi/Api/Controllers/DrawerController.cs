using Abp.Application.Services.Dto;
using Abp.Authorization;
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
using TicoPay.Drawers;
using TicoPay.Drawers.Dto;

namespace TicoPay.Api.Controllers
{
    /// <summary>
    /// Conjunto de Métodos que manejan la consulta de Cajas y Sucursales en Ticopays / Methods that manage the drawers and Branches in Ticopays 
    /// </summary>
    /// <seealso cref="TicoPay.Api.Common.TicoPayApiController" />
    [AbpAuthorize]
    [Abp.Runtime.Validation.DisableValidation]
    public class DrawerController : TicoPayApiController
    {
        private readonly IRepository<BranchOffice, Guid> _branchOfficesRepository;
        private readonly IRepository<Drawer, Guid> _drawerRepository;
        private readonly IDrawersAppService _drawerAppService;

        /// <exclude />
        public DrawerController(
            IRepository<BranchOffice, Guid> branchOfficesRepository, IRepository<Drawer, Guid> drawerRepository, IDrawersAppService drawerAppService)
        {
            _drawerAppService = drawerAppService;
            _branchOfficesRepository = branchOfficesRepository;
            _drawerRepository = drawerRepository;
        }

        /// <summary>
        /// Obtiene una Caja del Sub Dominio / Gets a Drawer from the Tenant.
        /// </summary>
        /// <remarks>
        /// Obtiene una Caja del Sub Dominio / Gets a Drawer from the Tenant.
        /// </remarks>
        /// <returns>Caja / Drawer</returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna una Caja -> (listObjectResponse) / Returns a Drawer from the Tenant in -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<DrawerDto>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro / Return this message if an invalid security token wast sent or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe la Caja en el Sub Dominio/ Returns this message when the Drawer doesn't exist in that Tenant", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay / Returns this message of an internal process failed in the Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get(Guid drawerId)
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
                }
                var drawer = _drawerAppService.Get(drawerId);
                if (drawer == null)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                }
                else
                {
                    return Ok(new TicoPayResponseAPI<DrawerDto>(HttpStatusCode.OK, drawer, null));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Obtiene las Cajas de una Sucursal / Gets the Drawers from a Branch Office.
        /// </summary>
        /// <remarks>
        /// Obtiene las Cajas de una Sucursal / Gets the Drawers from a Branch Office
        /// </remarks>
        /// <param name="branchId">Id de la Sucursal / The Branch Office identifier.</param>
        /// <returns>Lista de Cajas / List of Drawers</returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de Cajas de una Sucursal -> (listObjectResponse) / Returns the List of Drawers from a Branch offices in -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<DrawerDto>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro / Return this message if an invalid security token wast sent or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Cajas para esa Sucursal/ Returns this message when there are no Drawers assigned to the Branch Office", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay / Returns this message of an internal process failed in the Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetDrawersFromBranch")]
        public IHttpActionResult GetDrawersFromBranch(Guid branchId)
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
                }
                var drawers = _drawerAppService.SearchDrawersApi(new SearchDrawerInput { BranchOfficeFilter = branchId, TenantId = (int) AbpSession.TenantId});
                if (drawers == null || drawers.Items.Count == 0)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));
                }
                else
                {                    
                    return Ok(new TicoPayResponseAPI<DrawerDto>(HttpStatusCode.OK, null, drawers));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }



        /// <summary>
        /// Abre una Caja / Opens a Drawer.
        /// </summary>
        /// <remarks>
        /// Abre una Caja / Opens a Drawer.
        /// </remarks>
        /// <param name="drawerId">Id de la Caja / The Drawer identifier.</param>
        /// <returns>Verdadero si Abrio la Caja , Error si no pudo ser abierta / True if the Drawer was Opened, Error if it couldn't be Opened </returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : La Caja fue Abierta -> (objectResponse) / The Drawer has been Opened in -> (objectResponse)", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si la caja ya esta abierta / Returns this message if the Drawer is already Open", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro / Return this message if an invalid security token wast sent or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que la Caja no Existe / Returns this message when the Drawer doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay / Returns this message of an internal process failed in the Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpPost]
        [Route("OpenDrawer")]
        public IHttpActionResult OpenDrawer(Guid drawerId)
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
                }
                var drawer = _drawerAppService.Get(drawerId);
                if (drawer == null)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                }
                else
                {
                    if (drawer.IsOpen)
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "La caja ya esta abierta"));
                    }
                    else
                    {
                        try
                        {
                            _drawerAppService.OpenDrawer(drawerId);
                            return Ok(new TicoPayResponseAPI<bool>(HttpStatusCode.OK, true, null));
                        }
                        catch(Exception ex)
                        {
                            return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Cierra una Caja / Closes a Drawer.
        /// </summary>
        /// <remarks>
        /// Cierra una Caja / Closes a Drawer.
        /// </remarks>
        /// <param name="drawerId">Id de la Caja / The Drawer identifier.</param>
        /// <returns>Verdadero si Cerro la Caja , Error si no pudo ser cerrada / True if the Drawer was Closed, Error if it couldn't be Closed </returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : La Caja fue Cerrada -> (objectResponse) / The Drawer has been Closed in -> (objectResponse)", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si la caja ya esta Cerrada / Returns this message if the Drawer is already Close", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro / Return this message if an invalid security token wast sent or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que la Caja no Existe / Returns this message when the Drawer doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay / Returns this message of an internal process failed in the Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpPost]
        [Route("CloseDrawer")]
        public IHttpActionResult CloseDrawer(Guid drawerId)
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
                }
                var drawer = _drawerAppService.Get(drawerId);
                if (drawer == null)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                }
                else
                {
                    if (drawer.IsOpen == false)
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "La caja ya esta cerrada"));
                    }
                    else
                    {
                        try
                        {
                            _drawerAppService.CloseDrawer(drawerId);
                            return Ok(new TicoPayResponseAPI<bool>(HttpStatusCode.OK, true, null));
                        }
                        catch (Exception ex)
                        {
                            return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }
    }
}
