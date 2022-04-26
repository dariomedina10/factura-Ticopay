using Abp.Application.Services.Dto;
using Abp.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Web.Http;
using TicoPay.Address;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.GroupConcept.Dto;
using TicoPay.Groups;
using TicoPay.Groups.Dto;
using TicoPay.Address.Dto;
using TicoPay.General;
using Abp.AutoMapper;
using Swashbuckle.Swagger.Annotations;

namespace TicoPay.Api.Controllers
{

    /// <summary>
    /// Conjunto de Métodos que manejan las direcciones / Methods that manage addresses 
    /// </summary>
    public class AddressController : AbpApiController
    {
        private readonly IAddressService _addressAppService;

        /// <exclude />
        public AddressController(IAddressService addressAppService)
        {
            _addressAppService = addressAppService;
          
        }

        /// <summary>
        /// Obtiene la lista de países / Gets the Country list.
        /// </summary>
        /// <remarks>Obtiene la lista de países validado según hacienda / Gets the country list</remarks>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de Países en / Returns the Country list -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<CountryDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro / Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Países en la lista / Returns this message if there are no Countries", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetAllCountries")]
        public IHttpActionResult GetAllCountries()
        {          
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
                }
                var countries =  _addressAppService.GetAllCountries().MapTo<List<CountryDto>>();
                if (countries == null)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                }
                else
                {
                    return Ok(new TicoPayResponseAPI<CountryDto>(HttpStatusCode.OK, null, new ListResultDto<CountryDto>(countries)));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Obtiene todas las provincias de Costa Rica / Gets the Provinces in Costa Rica.
        /// </summary>
        /// <remarks>Obtiene todas las provincias validas según Hacienda en Costa Rica / Gets all the valid Provinces in Costa Rica</remarks>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de Países / Return the Country list -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<ProvinciaDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Provincias en la lista / Returns this message when there are no Provinces", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetAllProvincias")]
        public IHttpActionResult GetAllProvincias()
        {
            try
            {
                if (AbpSession.TenantId == null)                
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
               
                var result =   _addressAppService.GetAllProvincias().MapTo<List<ProvinciaDto>>();                
                
                if (result == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));
               
                else               
                    return Ok(new TicoPayResponseAPI<ProvinciaDto>(HttpStatusCode.OK,null, new ListResultDto<ProvinciaDto>(result)));
               
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }

        }

        /// <summary>
        /// Obtiene los Cantones de una Provincia / Gets the Cantons of a Province.
        /// </summary>
        /// <param name="IdProvincia">Id de la Provincia / Province Id.</param>
        /// <remarks>Obtiene la lista de Catones pertenecientes a una Provincia especifica / Gets a list of the Cantons that belongs to a Province</remarks>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de Cantones de una Provincia / Returns a list of Cantons of a Province-> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<CantonDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Catones en la provincia solicitada o si la misma no existe "+
            "/ Returns this message when there are no Cantons in the Province or the Province doesn't exist ", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetCanton")]
        public  IHttpActionResult GetCanton(int IdProvincia)
        {

            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var result = _addressAppService.GetCantonesByProvinciaId(IdProvincia).MapTo<List<CantonDto>>();

                if (result == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else
                    return Ok(new TicoPayResponseAPI<CantonDto>(HttpStatusCode.OK, null, new ListResultDto<CantonDto>(result)));

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR,  ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Obtener los Distritos de un Canton / Gets the District list of a Canton.
        /// </summary>
        /// <param name="IdCanton">Id de Canton / Canton Id.</param>
        /// <remarks>Obtiene todos los distritos de un canton especifico / Gets all the Districts of a Canton</remarks>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de los Distritos de un Canton / Returns the District list of a Canton-> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<DistritoDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Distritos en el Canton solicitado o si el mismo no existe "+
            "/ Returns this message when the Canton doesn't exist or doesn't have any Districts", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetDistrito")]
        public IHttpActionResult GetDistrito(int IdCanton)
        {
            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var result = _addressAppService.GetDistritosByCantonId(IdCanton).MapTo<List<DistritoDto>>();

                if (result == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else
                    return Ok(new TicoPayResponseAPI<DistritoDto>(HttpStatusCode.OK, null, new ListResultDto<DistritoDto>(result)));

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Obtiene los Barrios de un Distrito / Gets the Hoods of a District.
        /// </summary>
        /// <param name="IdDistrito">Id del Distrito / District Id.</param>
        /// <remarks>Obtiene todos los Barrios que pertenecen a un Distrito / Gets all the Hoods of a District</remarks>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de los Barrios de un Distrito / Returns a list of the Hoods of a District-> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<BarrioDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "
            + "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Barrios en el Distrito solicitado o si el mismo no existe "+ 
            "/ Returns this message when the are no Hood in a District or the District doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays " +
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetBarrio")]
        public IHttpActionResult GetBarrio(int IdDistrito)
        {
            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var result = _addressAppService.GetBarriosByDistritoId(IdDistrito).MapTo<List<BarrioDto>>();

                if (result == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else
                    return Ok(new TicoPayResponseAPI<BarrioDto>(HttpStatusCode.OK, null, new ListResultDto<BarrioDto>(result)));

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

       
    }
}
