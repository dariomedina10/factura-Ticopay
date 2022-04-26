using Abp.Dependency;
using Abp.WebApi.Controllers;
using LinqKit;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Web.Http;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.GroupConcept.Dto;
using TicoPay.Groups;
using TicoPay.Groups.Dto;
using TicoPay.Services.Dto;

namespace TicoPay.Api.Controllers
{

    /// <summary>
    /// Conjunto de Métodos que manejan la consulta y creación de clientes en Ticopays / Methods that manage the Clients
    /// </summary>
    [Abp.Runtime.Validation.DisableValidation]
    public class ClientController : AbpApiController
    {
        private readonly IClientAppService _clientAppClient;
        private readonly IGroupAppService _groupAppService;
        private readonly IIocResolver _iocResolver;

        /// <exclude />
        public ClientController(IClientAppService clientAppClient, IGroupAppService groupAppService, IIocResolver iocResolver)
        {
            _clientAppClient = clientAppClient;
            _groupAppService = groupAppService;
            _iocResolver = iocResolver;
        }

        /// <summary>
        /// Obtiene la lista de todos los clientes / Gets a list of all the Tenant Client's.
        /// </summary>
        /// <remarks>Obtiene la lista de todos los clientes del Tenant en Ticopays , La opción de detallado permite retornar menos data (solo campos importantes) para mejorar tiempos de respuesta
        /// /  Gets a list of all the Tenant Client's, if the detallado Option is send in true , then the Api returns all the client's data , if not , then it only returns the main fields to improve response times
        /// </remarks>
        /// <param name="detallado">Si <c>true</c> [detallado] retorna toda la información de los clientes , sino retorna solo los datos principales / If <c>true</c> [detallado] returns all client Data, if not returns only the Main Fields.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna lista de Clientes / Returns the list of Clients -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<ClientDto>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Clientes creados/ Returns this message if there is no clients", Type = typeof(TicoPayResponseErrorAPI))]
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
                {
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
                }
                var clients = (detallado) ? _clientAppClient.GetClients() : _clientAppClient.GetClients(false);
               
                
                if (clients == null)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                }
                else
                {
                    return Ok(new TicoPayResponseAPI<ClientDto>(HttpStatusCode.OK,null,clients));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Obtiene los datos de un Cliente especifico / Gets the Data of a specific Client.
        /// </summary>
        /// <remarks>
        /// Obtiene un cliente especifico, La opción de detallado permite retornar menos data (solo campos importantes) para mejorar tiempos de respuesta
        /// Gets a specific Client, if the Detallado option is true then gets all the client data, if not , only gets the main Fields to improve response time
        /// </remarks>
        /// <param name="detallado">Si <c>true</c> [detallado] retorna los datos completos del cliente , sino retorna solo los datos principales
        /// If <c>true</c> [detallado] returns all client data, if not  returns only the main fields.</param>
        /// <param name="Id">Id del Cliente / Client Id (not the same as the identification number).</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el Cliente / Returns the Client -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ClientDto>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Cliente solicitado / Returns this message when the Client doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
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
               
                var client =  (detallado) ? _clientAppClient.Get(Id) : _clientAppClient.Get(Id);

                if (client == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));
               
                else               
                    return Ok(new TicoPayResponseAPI<ClientDto>(HttpStatusCode.OK,client));
               
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }

        }

        /// <summary>
        /// Busca un Cliente por su numero de identificación o su email / Gets a Client by its Identification number or Email.
        /// </summary>
        /// <remarks>
        /// Obtiene la lista de todos los clientes que coinciden con la búsqueda (Puede existir mas de un cliente con el mismo Email en cuyo caso retornara el primero),
        /// La opción de detallado permite retornar menos data (solo campos importantes) para mejorar tiempos de respuesta
        /// / Gets the Client that match the search parameter , in case of email gets the first client that match the email
        /// </remarks>
        /// <param name="detallado">Si <c>true</c> [detallado] retorna los datos completos del cliente , sino retorna solo los datos principales 
        /// / If <c>true</c> [detallado] returns all Client data, if not returns only the main fields.</param>
        /// <param name="Id">Número de Identificación (Cédula, NITE, DIMEX, Pasaporte, Etc) 
        /// / Identification number (Personal Identification, Legal Identification, Migration Identity Document, Special Tax number Identification, Foreign Document ).
        /// </param>
        /// <param name="email">Correo Electrónico / Email.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el Cliente / Returns a Client -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ClientDto>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los parámetros de búsqueda enviados", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Cliente solicitado / Returns this message when the requested client could not be found", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("Search")]
        public IHttpActionResult GetSearch(bool detallado, string Id = null, string email = null)
        {
            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var predicate = PredicateBuilder.New<Client>(true);

                if (!string.IsNullOrEmpty(Id))
                {
                    predicate = predicate.And(d => d.Identification == Id || d.IdentificacionExtranjero == Id);
                }

                if (email != null)
                {
                    predicate = predicate.And(d => d.Email == email);
                }

                var client = (detallado) ? _clientAppClient.Get(predicate) : _clientAppClient.Get(predicate);

                if (client == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else
                    return Ok(new TicoPayResponseAPI<ClientDto>(HttpStatusCode.OK, client));

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }

        }


        /// <summary>
        /// Actualiza un Cliente Especifico / Updates a Client.
        /// </summary>
        /// <remarks>
        /// Actualiza los datos de un cliente especifico / Updates a client information
        /// </remarks>
        /// <param name="input">Cliente con Datos Actualizados / Client with the new data.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el Cliente / Returns the updated Client -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ClientDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos del cliente enviado o si el cliente no existe" +
            "Validaciones : " +
            "Cédula Física : El número de Cédula física, no debe tener un 0 al inicio" +
            "Cédula Física : El número de Cédula física, debe contener 9 dígitos" +
            "Cédula Física : Para el tipo de identificación seleccionada el Apellido es obligatorio" +
            "Cédula Jurídica : El número de cédula jurídica, debe contener 10 dígitos" +
            "DIMEX : El número DIMEX, no debe tener un 0 al inicio" +
            "DIMEX : El número DIMEX, debe contener 11 0 12 dígitos" +
            "DIMEX : Para el tipo de identificación seleccionada el Apellido es obligatorio" +
            "NITE : El número NITE, debe contener 10 dígitos" +
            "NITE : Para el tipo de identificación seleccionada el Apellido es obligatorio "+
            "/ Returns this message when there is a problem with the new data or de client doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Cliente solicitado", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]    
        [Route("Put")]
        [HttpPost]
        public  IHttpActionResult Put(ClientDto input)
        {
           
            if (AbpSession.TenantId == null)           
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            ModelState.Clear();
            if (!ModelState.IsValid)            
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));
          
            try
            {
                _clientAppClient.Update(input);
                return Ok(new TicoPayResponseAPI<ClientDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Crea un Cliente / Creates a new Client.
        /// </summary>
        /// <remarks>
        /// Crea un Cliente con los datos proporcionados / Creates a new client with the information provided
        /// </remarks>
        /// <param name="input">Cliente a Crear / Client to create.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el Cliente / Returns the newly created Client -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ClientDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si hay problemas con los datos del cliente enviado" + 
            "Validaciones : " +
            "Cédula Física : El número de Cédula física, no debe tener un 0 al inicio" +
            "Cédula Física : El número de Cédula física, debe contener 9 dígitos" +
            "Cédula Física : Para el tipo de identificación seleccionada el Apellido es obligatorio" +
            "Cédula Jurídica : El número de cédula jurídica, debe contener 10 dígitos" +
            "DIMEX : El número DIMEX, no debe tener un 0 al inicio" +
            "DIMEX : El número DIMEX, debe contener 11 0 12 dígitos" +
            "DIMEX : Para el tipo de identificación seleccionada el Apellido es obligatorio" +
            "NITE : El número NITE, debe contener 10 dígitos" +
            "NITE : Para el tipo de identificación seleccionada el Apellido es obligatorio "+
            "/ Returns this message when there is an error with the client information", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Cliente solicitado", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]       
        [Route("Post")]
        [HttpPost]
        public  IHttpActionResult Post(ClientDto input)
        {
           

            if (AbpSession.TenantId == null)           
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            ModelState.Clear();
            ModelState.AddValidationErrors(input, _iocResolver);

            if (!ModelState.IsValid)            
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));
            
            try
            {
               var client= _clientAppClient.Create(input);
                return Ok(new TicoPayResponseAPI<ClientDto>(HttpStatusCode.OK, client));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Elimina un Cliente Especifico que no tenga facturas ni notas de débito pendientes/ Deletes a Client (That doesn't have any pending invoices or Debit Memo).
        /// </summary>
        /// <remarks>
        /// Elimina un Cliente especifico del Tenant que no tenga facturas ni notas de débito pendientes/ Deletes a Client (That doesn't have any pending invoices or Debit Memo)
        /// </remarks>
        /// <param name="id">Id del Cliente / Client ID (Not the same as Identification number).</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna null si el cliente fue eliminado / Returns null if the Client was deleted -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ClientDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si el cliente posee facturas pendientes / Returns this message if the Client has pending invoices", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe el Cliente a eliminar / Returns this message when the Client to Delete doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("Delete")]
        [HttpPost]
        public  IHttpActionResult Delete(Guid id)
        {
           
            if (AbpSession.TenantId == null)               
            {
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            }            
            try
            {
               var client = _clientAppClient.Get(id);
                
                if (client == null){
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));
                }

                if (!_clientAppClient.isAllowedDelete(id)){
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "El cliente posee facturas pendientes."));
                }
                _clientAppClient.Delete(id); 
                return Ok(new TicoPayResponseAPI<ClientDto>(HttpStatusCode.OK, null));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Obtiene los Servicios Recurrentes de un Cliente / Gets the Scheduled Services of a Client.
        /// </summary>
        /// <remarks>
        /// Obtiene los Servicios Recurrentes de un Cliente (Utilizado en facturación Recurrente) / Gets the Scheduled Services of a Client (Used in Scheduled invoicing)
        /// </remarks>
        /// <param name="Id">Id del Cliente / Client Id (Not the same as Identification number).</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna los Servicios Recurrentes de un Cliente / Returns all Scheduled services of the Client -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<ServiceDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si el cliente posee facturas pendientes", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Servicios Recurrentes para el cliente especificado "+
            "/ Returns this message when there are no Scheduled services for that client", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetServices")]
        public IHttpActionResult GetServices(Guid Id)
        {
            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var services = _clientAppClient.GetServices(Id);

                if (services == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else
                    return Ok(new TicoPayResponseAPI<ServiceDto>(HttpStatusCode.OK, null, services));

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }

        }

        /// <summary>
        /// Actualizar Servicios recurrentes de un Cliente / Updates a Scheduled service of a Client.
        /// </summary>
        /// <remarks>
        /// Actualizar Servicios recurrentes de un Cliente (Utilizado en facturación Recurrente).
        /// </remarks>
        /// <param name="input">Datos del Cliente y los Servicios a Actualizar / Client Information and Scheduled Services to Update.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna Null si los Servicios Recurrentes fueron actualizados correctamente / Return null if the Scheduled Services were successfully updated -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ClientDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje indicando que alguno de los Servicios Recurrentes a agregar no existe o el cliente no existe "+
            "/ Returns this message when a Scheduled service or the client doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que alguno de los servicios a agregar no existe o el cliente no existe", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("PutServices")]
        [HttpPost]
        public IHttpActionResult PutServices(ClientServicesDto input)
        {

            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            ModelState.Clear();
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                _clientAppClient.UpdateServices(input.ClientId, input.Services);
                return Ok(new TicoPayResponseAPI<ClientDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Asigna Servicios Recurrentes a un Cliente / Set Scheduled services to a Client.
        /// </summary>
        /// <remarks>
        /// Asigna Servicios Recurrentes a un Cliente (Utilizado en facturación Recurrente) / Set Scheduled services to a Client (To be used in Scheduled Invoicing).
        /// </remarks>
        /// <param name="input">Servicios Recurrentes a Asignar / Scheduled Services to assign.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna Null si los Servicios Recurrentes fueron asignados al cliente correctamente / Returns null if the Scheduled services were successfully assign -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ClientDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje indicando que alguno de los Servicios Recurrentes a agregar no existe o el cliente no existe "+
            "/ Returns this message when one of the Scheduled services to assign doesn't exist or the Client doesn't existe", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que alguno de los servicios a agregar no existe o el cliente no existe", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("PostServices")]
        [HttpPost]
        public IHttpActionResult PostServices(ClientServicesDto input)
        {

            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            ModelState.Clear();
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                _clientAppClient.AddServices(input.ClientId, input.Services);
                return Ok(new TicoPayResponseAPI<ClientDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Obtiene los Grupos o Categorías de un Cliente / Gets the Client Categories.
        /// </summary>
        /// <remarks>
        /// Obtiene todos los Grupos o Categorías de un Cliente especifico / Gets the Categories or Classification of a Client
        /// </remarks>
        /// <param name="Id">Id de Cliente / Client ID (Not the same as Identification number).</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la lista de grupos de un cliente / Returns the Client Categories List -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<GroupDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si el cliente posee facturas pendientes", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Categorías para ese cliente o el mismo no existe "+
            "/ Returns this message when the are no categories assigned to that Client o the Client doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetGroups")]
        public IHttpActionResult GetGroups(Guid Id)
        {
            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var groups = _clientAppClient.GetGroups(Id);

                if (groups == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else
                    return Ok(new TicoPayResponseAPI<GroupDto>(HttpStatusCode.OK, null, groups));

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }

        }

        /// <summary>
        /// Actualiza los Grupos o Categorías de un cliente / Updates Categories assigned to a Client.
        /// </summary>
        /// <remarks>
        /// Actualiza los Grupos o Categorías de un cliente / Updates Categories assigned to a Client
        /// </remarks>
        /// <param name="input">ID de Cliente y Grupos a Asignar / Client Id and Categories to Assign.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna null si los grupos fueron actualizados / Return null if the Categories were successfully assigned -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ClientDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si los grupos agregados tienen errores / Returns this message when the assigned categories contains errors", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Categorías para ese cliente o el mismo no existe", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("PutGroups")]
        [HttpPost]
        public IHttpActionResult PutGroups(ClientGroupsDto input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            ModelState.Clear();
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                _clientAppClient.AddGroups(input.ClientId, input.Groups, true);
                return Ok(new TicoPayResponseAPI<ClientDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Agrega los Grupos o Categorías de un cliente / Adds Categories assigned to a Client.
        /// </summary>
        /// <remarks>
        /// Agrega los Grupos o Categorías de un cliente  / Adds Categories assigned to a Client.
        /// </remarks>
        /// <param name="input">Id de Cliente y Grupos a Actualizar / Client Id and Categories to Assign.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna null si los grupos fueron asignados / Returns null if the categories were successfully assigned -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ClientDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si los grupos agregados tienen errores / Returns this message when the categories added have errors", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Categorías para ese cliente o el mismo no existe", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("PostGroups")]
        [HttpPost]
        public IHttpActionResult PostGroups(ClientGroupsDto input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            ModelState.Clear();
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                _clientAppClient.AddGroups(input.ClientId, input.Groups, false);
                return Ok(new TicoPayResponseAPI<ClientDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Obtiene los Grupos de Conceptos de un Cliente.
        /// </summary>
        /// <remarks>
        /// Obtiene los Grupos de Conceptos de un Cliente (Utilizado para la facturación Recurrente).
        /// </remarks>
        /// <param name="Id">ID de Cliente.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna null si los grupos fueron asignados en -> (objectResponse)", Type = typeof(TicoPayResponseAPI<GroupConceptsDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si los grupos agregados tienen errores", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro / Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Grupos de Conceptos para ese cliente o el mismo no existe", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays / Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetGroupConcepts")]        
        public IHttpActionResult GetGroupConcepts(Guid Id)
        {
            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var groups = _clientAppClient.GetGroupsConcepts(Id);

                if (groups == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else
                    return Ok(new TicoPayResponseAPI<GroupConceptsDto>(HttpStatusCode.OK, null, groups));

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }

        }

        /// <summary>
        /// Actualiza los Grupos de Conceptos de un Cliente.
        /// </summary>
        /// <remarks>
        /// Actualiza los Grupos de Conceptos de un Cliente (Utilizado para la Facturación Recurrente).
        /// </remarks>
        /// <param name="input">ID de Cliente y la Lista de grupos de conceptos a actualizar.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna null si los grupos de conceptos fueron actualizados en -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ClientGroupsConceptsDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si los grupos de conceptos a actualizar tienen errores o el Cliente no existe", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro / Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Grupos de Conceptos para ese cliente o el mismo no existe", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays / Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("PutGroupConcepts")]
        [HttpPost]
        public IHttpActionResult PutGroupConcepts(ClientGroupsConceptsDto input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            ModelState.Clear();
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                _clientAppClient.AddGroupsConcepts(input.ClientId, input.Groups, true);
                return Ok(new TicoPayResponseAPI<ClientGroupsConceptsDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Asigna los Grupos de Conceptos de un Cliente / Assign the Invoice Concept group of a Client.
        /// </summary>
        /// <remarks>
        /// Asigna los Grupos de Conceptos de un Cliente (Utilizado para la Facturación Recurrente) / Assign the Invoice Concept group of a Client.
        /// </remarks>
        /// <param name="input">ID de Cliente y la Lista de grupos de conceptos a asignar / Client Id and Invoice Group Concepts to assign.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna null si los grupos de conceptos fueron asignados al cliente / Returns null if the Invoice Concept groups were assigned -> (objectResponse)", Type = typeof(TicoPayResponseAPI<ClientGroupsConceptsDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si los grupos de conceptos a asignar tienen errores o el Cliente no existe "+
            "/ Returns this message if the Concept groups have errors or the Client doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Grupos de Conceptos para ese cliente o el mismo no existe", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("PostGroupConcepts")]
        [HttpPost]
        public IHttpActionResult PostGroupConcepts(ClientGroupsConceptsDto input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            ModelState.Clear();
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                _clientAppClient.AddGroupsConcepts(input.ClientId, input.Groups, false);
                return Ok(new TicoPayResponseAPI<ClientGroupsConceptsDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }


        /// <summary>
        /// Obtiene la lista de Grupos o Categorías de Clientes / Gets a list of the Client Categories.
        /// </summary>
        /// <remarks>
        /// Obtiene la lista de Grupos o Categorías de Clientes del Tenant / Gets a list of the client Categories.
        /// </remarks>
        /// <param name="detallado">Si es <c>true</c> devuelve la información completa de las categorías , sino solo los campos esenciales.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la lista de las Categorías disponibles / Returns the list of Categories -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<GroupDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si los grupos agregados tienen errores", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existen Categorías creadas para el Tenant "+
            "/ Returns this message when there are no categories available", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetGroupAll")]
        public IHttpActionResult GetGroupAll(bool detallado)
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
                }
                var groups = (detallado) ? _groupAppService.GetGroups() : _groupAppService.GetGroups();

                if (groups == null)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                }
                else
                {
                    return Ok(new TicoPayResponseAPI<GroupDto>(HttpStatusCode.OK, null, groups));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Obtiene la información de una Categoría de Cliente / Gets a specific Client Category.
        /// </summary>
        /// <remarks>
        ///  Obtiene la información de una Categoría de Cliente especifica / Gets a specific Client Category.
        /// </remarks>
        /// <param name="detallado">Si es <c>true</c> devuelve la información completa de las categorías , sino solo los campos esenciales.</param>
        /// <param name="Id">Id de Categoría / Category Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la Categoría solicitada / Returns a Category -> (objectResponse)", Type = typeof(TicoPayResponseAPI<GroupDto>))]
        // [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si los grupos agregados tienen errores", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe la Categoría solicitada / Returns this message when the Category doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetGroup")]
        public IHttpActionResult GetGroup(bool detallado, Guid Id)
        {
            try
            {
                if (AbpSession.TenantId == null)
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

                var group = (detallado) ? _groupAppService.Get(Id) : _groupAppService.Get(Id);

                if (group == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                else
                    return Ok(new TicoPayResponseAPI<GroupDto>(HttpStatusCode.OK, group));

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }

        }

        /// <summary>
        /// Actualiza un Categoría de Cliente / Updates a Client Category.
        /// </summary>
        /// <remarks>
        /// Actualiza un Categoría de Cliente del Tenant / Updates a Client Category
        /// </remarks>
        /// <param name="input">Datos de la Categoría a actualizar / Category to Update.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna null si la Categoría fue actualizada / Returns null if the Category was successfully updated-> (objectResponse)", Type = typeof(TicoPayResponseAPI<GroupDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si los datos de la Categoría tienen errores o si la Categoría a actualizar no existe "+
            "/ Returns this message when the Category doesn't exist or the information has errors", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe la Categoría solicitada", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("PutGroup")]
        [HttpPost]
        public IHttpActionResult PutGroup(UpdateGroupInput input)
        {

            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            ModelState.Clear();
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
                _groupAppService.Update(input);
                return Ok(new TicoPayResponseAPI<GroupDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }
        }

        /// <summary>
        /// Crea un Grupo o Categoría / Creates a new Category.
        /// </summary>
        /// <remarks>
        /// Crea un Grupo o Categoría para el Tenant / Creates a new Category
        /// </remarks>
        /// <param name="input">Datos de la Categoría a Crear / Category information.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la Categoría creada / Returns the newly created Category -> (objectResponse)", Type = typeof(TicoPayResponseAPI<GroupDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si los datos de la Categoría tienen errores "+
            "/ Returns this message if the category information contains errors", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        // [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe la Categoría solicitada", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("PostGroup")]
        [HttpPost]
        public IHttpActionResult PostGroup(GroupDto input)
        {
            if (AbpSession.TenantId == null)
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));

            ModelState.Clear();
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ModelState.ToErrorMessage()));

            try
            {
               var group= _groupAppService.Create(input);
                return Ok(new TicoPayResponseAPI<GroupDto>(HttpStatusCode.OK, group));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

        /// <summary>
        /// Elimina un Grupo o Categoría de Cliente / Deletes a Client Category.
        /// </summary>
        /// <remarks>
        /// Elimina un Grupo o Categoría de Cliente del Tenant / Deletes a Client Category.
        /// </remarks>
        /// <param name="id">Id del Cliente a Eliminar / Category Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna Null si la Categoría fue eliminada / Returns null if the Category was successfully deleted -> (objectResponse)", Type = typeof(TicoPayResponseAPI<GroupDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna este mensaje si la categoría se encuentra asociada a un cliente "+
            "/ Return this message if the category is already associated to a client an cant be deleted", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que no existe la Categoría a eliminar / Returns this message if the Category doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [Route("DeleteGroup")]
        [HttpPost]
        public IHttpActionResult DeleteGroup(Guid id)
        {

            if (AbpSession.TenantId == null)
            {
                return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
            }
            try
            {
                var group =  _groupAppService.Get(id);
                if (group == null)
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                if (!_groupAppService.isAllowedDelete(id))
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "La característica se encuentra asociado a un cliente."));
                }
                _groupAppService.Delete(id); ;
                return Ok(new TicoPayResponseAPI<GroupDto>(HttpStatusCode.OK, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));

            }
        }

    }
}
