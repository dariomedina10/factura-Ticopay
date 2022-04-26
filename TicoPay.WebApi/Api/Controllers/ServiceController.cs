using Abp.WebApi.Controllers;
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
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.GroupConcept;
using TicoPay.GroupConcept.Dto;
using TicoPay.Groups;
using TicoPay.Services;
using TicoPay.Services.Dto;

namespace TicoPay.Api.Controllers
{
    /// <summary>
    /// Conjunto de Métodos que manejan la Consulta, Creación, Actualización y Eliminación de Servicios / Methods that manages the Services
    /// </summary>
    [Abp.Runtime.Validation.DisableValidation]
    public class ServiceController : AbpApiController
    {
        private readonly IServiceAppService _serviceAppService;
        private readonly IGroupConceptsAppService _groupConceptsAppService;
        private readonly IGroupAppService _groupAppService;

        /// <exclude />
        public ServiceController(IServiceAppService serviceAppService, IGroupAppService groupAppService, IGroupConceptsAppService groupConceptsAppService)
        {
            _serviceAppService = serviceAppService;
            _groupAppService = groupAppService;
            _groupConceptsAppService = groupConceptsAppService;
        }

        /// <summary>
        /// Obtiene Todos los Servicios / Gets all Services.
        /// </summary>
        /// <remarks>
        /// Obtiene Todos los Servicios del Tenant / Gets all Services.
        /// </remarks>
        /// <param name="detallado">Si es <c>true</c> Trae todos los Datos del Servicio, sino solo trae los Indispensables /
        /// If <c>true</c> Gets all Service information, Otherwise only gets the Main fields.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de Servicios / Returns a List of Services -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<ServiceDto>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Servicios creados "+
            "/ Returns this message if there are no Services Created", Type = typeof(TicoPayResponseErrorAPI))]
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
               
                var services = (detallado) ? _serviceAppService.GetServices() : _serviceAppService.GetServices();

                if (services == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));
                
                else               
                    return Ok(new TicoPayResponseAPI<ServiceDto>(HttpStatusCode.OK,null,services));
                
            }
            catch (Exception ex)
            {
               
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Obtiene un Servicio Especifico / Gets a specific Service.
        /// </summary>
        /// <remarks>
        /// Obtiene los datos un Servicio Especifico / Gets a specific Service.
        /// </remarks>
        /// <param name="detallado">Si es <c>true</c> Trae todos los Datos del Servicio, sino solo trae los Indispensables /
        /// If <c>true</c> gets all Service information , otherwise gets only the main fields.</param>
        /// <param name="Id">Id del Servicio / Service Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el Servicio / Returns a Service -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ServiceDto>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Servicio especificado / Returns this message if the service requested doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
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
               
                var service =  (detallado) ? _serviceAppService.Get(Id) : _serviceAppService.Get(Id);

                if (service == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));
               
                else               
                    return Ok(new TicoPayResponseAPI<ServiceDto>(HttpStatusCode.OK,service));
               
            }
            catch (Exception ex)
            {
                var exceptionDetais = ex.GetBaseException().ToString();
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }

        }

        /// <summary>
        /// Actualiza un Servicio especifico / Updates a Service.
        /// </summary>
        /// <remarks>
        /// Actualiza los datos de un Servicio especifico / Updates a Specific Service.
        /// </remarks>
        /// <param name="input">Datos del Servicio a Actualizar / Service Information.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna Null si el Servicio fue Actualizado / Returns null if the Service was successfully Updated -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ServiceDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los Datos del Servicio a Actualizar o si el mismo no existe "+
            "/ Returns this message if the Service doesn't exist or the are errors in some of the fields", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Servicio especificado", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("Put")]
        [HttpPost]
        public IHttpActionResult Put(ServiceDto input)
        {

            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                _serviceAppService.Update(input);
                return Ok(new TicoPayResponseAPI<ServiceDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Crea un Servicio / Creates a Service.
        /// </summary>
        /// <remarks>
        /// Crea un Servicio / Creates a new Service.
        /// </remarks>
        /// <param name="input">Datos del Servicio a Crear / Service Information.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el Servicio Creado / Returns the newly created Service -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ServiceDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los Datos del Servicio a Crear "+
            "/ Returns this message if there are errors in some of the fields", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Servicio especificado", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("Post")]
        [HttpPost]
        public IHttpActionResult Post(ServiceDto input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
               var service= _serviceAppService.Create(input);
                return Ok(new TicoPayResponseAPI<ServiceDto>(HttpStatusCode.OK, service));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Elimina un Servicio especifico / Deletes a specific Service.
        /// </summary>
        /// <remarks>
        /// Elimina un Servicio especifico / Deletes a specific Service.
        /// </remarks>
        /// <param name="id">Id del Servicio / Service Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna Null si el Servicio fue Eliminado / Returns null if the Service was successfully deleted -> (objectResponse)", Type = typeof(TicoPayResponseAPI<UpdateClientInput>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si el servicio se encuentra asociado a clientes "+
            "/ Returns this message if the Service is used in an invoice or associated with a client.", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Servicio especificado "+
            "/ Returns this message if the Services doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
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
                var service = _serviceAppService.Get(id);

                if (service == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));
                
                if (!_serviceAppService.isAllowedDelete(id))                
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "El servicio se encuentra asociado a clientes."));
                
                _serviceAppService.Delete(id); ;
                return Ok(new TicoPayResponseAPI<UpdateClientInput>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        #region groupConcepts

        /// <summary>
        /// Obtiene todos los Grupos de Conceptos del Tenant / Gets all Concept Groups.
        /// </summary>
        /// <remarks>
        /// Obtiene todos los Grupos de Conceptos del Tenant (Usados para Facturación Recurrente) / Gets all concept Groups for scheduling invoices.
        /// </remarks>
        /// <param name="detallado">Si <c>true</c> trae todos los Campos de los Grupos, sino solo trae los esenciales.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la Lista de Grupos de Conceptos / Returns list of Group Concepts -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<GroupConceptsDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si el servicio se encuentra asociado a clientes.", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Grupos de Conceptos "+
            "/ Returns this message if there are no Concept Groups", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays / Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetGroupsAll")]
        public IHttpActionResult GetGroupsAll(bool detallado)
        {
            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var services = (detallado) ? _groupConceptsAppService.GetGroupConcepts() : _groupConceptsAppService.GetGroupConcepts();

                if (services == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else
                    return Ok(new TicoPayResponseAPI<GroupConceptsDto>(HttpStatusCode.OK, null, services));

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Obtiene un Grupo de Concepto especifico / Gets a specific Concept Group.
        /// </summary>
        /// <remarks>
        /// Obtiene un Grupo de Concepto especifico (Usados para Facturación Recurrente) / Gets a specific Concept Group.
        /// </remarks>
        /// <param name="Id">ID de Grupo de Concepto.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el Grupos de Conceptos solicitado / Returns a Concept Group -> (objectResponse)", Type = typeof(TicoPayResponseAPI<GroupConceptsDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si el servicio se encuentra asociado a clientes.", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Grupos de Conceptos "+
            "/ Returns this message if there are no Concept Groups", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetGroup")]
        public IHttpActionResult GetGroup(Guid Id)
        {
            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var service =  _groupConceptsAppService.Get(Id);

                if (service == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else
                    return Ok(new TicoPayResponseAPI<GroupConceptsDto>(HttpStatusCode.OK, service));

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }

        }

        /// <summary>
        /// Actualiza un Grupo de Conceptos especifico / Updates a Concept Group.
        /// </summary>
        /// <remarks>
        /// Actualiza un Grupo de Conceptos especifico (Usados para Facturación Recurrente) / Updates a Concept Group.
        /// </remarks>
        /// <param name="input">Datos del Grupo de Concepto Actualizados / Concept Group information.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna Null si el Grupos de Conceptos fue actualizado "+
            "/ Returns null of the if the Concept Group was successfully updated -> (objectResponse)", Type = typeof(TicoPayResponseAPI<GroupConceptsDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si existen errores con los Datos del Grupo de Concepto o si el mismo no existe "+
            "/ Returns this message if the Concept Group doesn't exist or some of the fields have errors.", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Grupos de Conceptos", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("PutGroup")]
        [HttpPost]
        public IHttpActionResult PutGroups(UpdateGroupConceptsInput input)
        {

            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            ModelState.Clear();
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                _groupConceptsAppService.Update(input);
                return Ok(new TicoPayResponseAPI<GroupConceptsDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Agrega un Grupo de Conceptos / Creates a Concept Group.
        /// </summary>
        /// <remarks>
        /// Agrega un Grupo de Conceptos (Usados para Facturación Recurrente) / Creates a Concept Group.
        /// </remarks>
        /// <param name="input">Datos del Grupo de Conceptos / Concept Group information.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el Grupo de Conceptos Creado / Returns the Concept group recently created -> (objectResponse)", Type = typeof(TicoPayResponseAPI<GroupConceptsDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si existen errores con los Datos del Grupo de Concepto o si el mismo ya existe "+
            "/ Returns this message if the Concept Already exist or there are problems in some of the fields.", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Grupos de Conceptos", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("PostGroup")]
        [HttpPost]
        public IHttpActionResult PostGroup(CreateGroupConceptsInput input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            ModelState.Clear();
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                var group =_groupConceptsAppService.Create(input);
                return Ok(new TicoPayResponseAPI<GroupConceptsDto>(HttpStatusCode.OK, group));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Eliminar un Grupo de Conceptos / Deletes a Concept Group.
        /// </summary>
        /// <remarks>
        /// Eliminar un Grupo de Conceptos (Usados para Facturación Recurrente) / Deletes a Concept Group.
        /// </remarks>
        /// <param name="id">ID del Grupo de Conceptos a Eliminar / Concept Group Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna Null si el Grupos de Conceptos Eliminado / Returns null if the Concept Group was successfully Deleted -> (objectResponse)", Type = typeof(TicoPayResponseAPI<UpdateClientInput>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si el Grupo de Concepto de servicios se encuentra asociado a clientes "+
            "/ Returns this message if the Concept Group doesn't exist.", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Grupos de Conceptos", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("DeleteGroup")]
        [HttpPost]
        public IHttpActionResult DeleteGroup(Guid id)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            try
            {
                var service = _groupConceptsAppService.Get(id);

                if (service == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                if (!_groupConceptsAppService.isAllowedDelete(id))
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "El grupo de servicios se encuentra asociado a clientes."));

                _groupConceptsAppService.Delete(id); 
                return Ok(new TicoPayResponseAPI<UpdateClientInput>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        #endregion groupConcepts
    }
}
