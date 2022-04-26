using Abp.WebApi.Controllers;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Web.Http;
using TicoPay.Clients.Dto;
using TicoPay.GroupConcept;
using TicoPay.Groups;
using TicoPay.Services.Dto;
using TicoPay.Taxes;
using TicoPay.Taxes.Dto;

namespace TicoPay.Api.Controllers
{
    /// <summary>
    /// Conjunto de Métodos que manejan la consulta y creación de Impuestos / Methods tha manage the Taxes
    /// </summary>
    /// <seealso cref="Abp.WebApi.Controllers.AbpApiController" />
    [Abp.Runtime.Validation.DisableValidation] //Deshabilita las validaciones que hace internamente boilerplate
    public class TaxController : AbpApiController
    {
        private readonly ITaxAppService _taxAppService;


        /// <exclude />
        public TaxController(ITaxAppService taxAppService, IGroupAppService groupAppService, IGroupConceptsAppService groupConceptsAppService)
        {
            _taxAppService = taxAppService;
            
        }

        /// <summary>
        /// Obtiene la lista de impuestos / Gets the Tax List.
        /// </summary>
        /// <remarks>
        /// Obtiene la lista de impuestos que posee el Tenant / Gets the Tenant Tax List
        /// </remarks>
        /// <param name="detallado">Si detallado <c>true</c> Trae todos los datos del impuesto , sino trae solo los campos indispensables.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la lista de impuestos / Returns the Tax List-> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<TaxDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de la factura", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen impuestos creados en para el Tenant "+
            "/ Returns this message if there are no Taxes Created", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetAll")]
        public IHttpActionResult GetAll(bool detallado)
        {          
            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
               
                var taxes = (detallado) ? _taxAppService.GetTaxes() : _taxAppService.GetTaxes();

                if (taxes == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));
                
                else               
                    return Ok(new TicoPayResponseAPI<TaxDto>(HttpStatusCode.OK,null,taxes));
                
            }
            catch (Exception ex)
            {
               
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Obtiene un Impuesto Especifico / Gets a Specific Tax.
        /// </summary>
        /// <remarks>
        /// Obtiene los datos de un Impuesto Especifico / Gets a Specific Tax
        /// </remarks>
        /// <param name="detallado">Si <c>true</c> obtiene todos los campos, sino solo los indispensables.</param>
        /// <param name="Id">Id de Impuesto / Tax Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el impuesto / Returns a Tax -> (objectResponse)", Type = typeof(TicoPayResponseAPI<TaxDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos de la factura", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el impuestos solicitado "+
            "/ Returns this message if the Requested Tax doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get(bool detallado, Guid Id)
        {
            try
            {
                if (AbpSession.TenantId == null)                
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
               
                var tax =  (detallado) ? _taxAppService.Get(Id) : _taxAppService.Get(Id);

                if (tax == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));
               
                else               
                    return Ok(new TicoPayResponseAPI<TaxDto>(HttpStatusCode.OK,tax));
               
            }
            catch (Exception ex)
            {
                var exceptionDetais = ex.GetBaseException().ToString();
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR,ex.GetBaseException().Message));

            }

        }


        /// <summary>
        /// Actualiza los Datos del Impuesto / Updates a Tax.
        /// </summary>
        /// <remarks>
        /// Actualiza los Datos del Impuesto / Updates a Tax
        /// </remarks>
        /// <param name="input">Datos del Impuesto / Tax information.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el impuesto / Returns the Updated Tax -> (objectResponse)", Type = typeof(TicoPayResponseAPI<TaxDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos del Impuesto o no existe "+
            "/ Returns this message if the Tax doesn't exist or there are problems with some of the fields", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el impuestos solicitado", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("Put")]
        [HttpPost]
        public IHttpActionResult Put(UpdateTaxInput input)
        {

            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            ModelState.Clear();
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                _taxAppService.Update(input);
                return Ok(new TicoPayResponseAPI<TaxDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Agrega un nuevo Impuesto / Creates a new Tax.
        /// </summary>
        /// <remarks>
        /// Agrega un nuevo impuesto al Tenant / Creates a new Tax
        /// </remarks>
        /// <param name="input">Datos del Impuesto / Tax information.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el impuesto / Returns the newly created Tax -> (objectResponse)", Type = typeof(TicoPayResponseAPI<TaxDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos del Impuesto "+
            "/ Returns this message if there are erros with some of the fields", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el impuestos solicitado", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("Post")]
        [HttpPost]
        public IHttpActionResult Post(CreateTaxInput input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            ModelState.Clear();
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
               var tax= _taxAppService.Create(input);
                return Ok(new TicoPayResponseAPI<TaxDto>(HttpStatusCode.OK, tax));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Elimina un Impuesto Especificado / Deletes a specific Tax.
        /// </summary>
        /// <remarks>
        /// Elimina un Impuesto especificado por parámetro / Deletes a specific Tax
        /// </remarks>
        /// <param name="id">Id de Impuesto / Tax Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el Null si el Impuesto fue Eliminado / Returns null if the Tax was successfully Deleted -> (objectResponse)", Type = typeof(TicoPayResponseAPI<TaxDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si el impuesto esta asociado a un servicio "+
            "/ Returns this message when the Tax is already associated to a Service or Product", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el impuesto a Eliminar "+
            "/ Returns this message when the Tax to delete doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("Delete")]
        [HttpPost]
        public IHttpActionResult Delete(Guid id)
        {
            if (AbpSession.TenantId == null)
              return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            
            try
            {
                var service = _taxAppService.Get(id);

                if (service == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));
                
                if (!_taxAppService.isAllowedDelete(id))                
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "El impuesto se encuentra asociado a servicio."));
                
                _taxAppService.Delete(id); ;
                return Ok(new TicoPayResponseAPI<UpdateClientInput>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

      
    }
}
