using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Uow;
using Abp.UI;
using Abp.Web.Models;
using Abp.WebApi.Controllers;
using LinqKit;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TicoPay.Api.Models;
using TicoPay.Clients;
using TicoPay.Clients.Dto;
using TicoPay.Groups.Dto;
using TicoPay.MultiTenancy;
using TicoPay.MultiTenancy.Dto;
using TicoPay.Roles;
using TicoPay.Roles.Dto;
using TicoPay.Users;
using TicoPay.Users.Dto;

namespace TicoPay.Api.Controllers
{
    /// <summary>
    /// Conjunto de Métodos que manejan la seguridad del Web Api
    /// </summary>
    public class AccountController : AbpApiController
    {
        /// <exclude />
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        private readonly UserManager _userManager;
        private readonly TenantManager _tenantManager;
        private readonly LogInManager _loginManager;
        private readonly IRoleAppService _roleAppService;
        private readonly IUserAppService _userAppService;
        private readonly ITenantAppService _tenantAppService;
        private readonly IClientAppService _clientAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        static AccountController()
        {
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
        }

        /// <exclude />
        public AccountController(UserManager userManager, TenantManager tenantManager, LogInManager loginManager, IRoleAppService roleAppService,IUserAppService userAppService, ITenantAppService tenantAppService, 
            IClientAppService clientAppService, IUnitOfWorkManager unitOfWorkManager)
        {
            _userManager = userManager;
            _tenantManager = tenantManager;
            _loginManager = loginManager;
            _roleAppService = roleAppService;
            _userAppService= userAppService;
            _tenantAppService = tenantAppService;
            _clientAppService = clientAppService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <exclude />
        [HttpGet]
        //[ResponseType(typeof(AgreementConectivity))]
        public IHttpActionResult GetTenantByPort(int port)
        {

            var convenio = _tenantManager.GetTenantsAgreementByPort(port);
            return Ok(new { results = convenio });
            // return convenio.FirstOrDefault();
        }

        /// <summary>
        /// Verifica si el Tenant tiene permiso para usar el Conector de Ticopays para su sistema / Verify if a Tenant has permission to use a Ticopays Connector.
        /// </summary>
        /// <remarks>
        /// Verifica si el Tenant tiene permiso para usar el Conector de Ticopays para su sistema / Verify if a Tenant has permission to use a Ticopays Connector.
        /// </remarks>
        /// <param name="tenantName">Nombre del Tenant / Tenant name.</param>
        /// <param name="ConnectorType">Nombre del Conector / Conector Name.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna True si el Tenant tiene permiso para usar el conector / Returns True if the Tenant has permission to use the connector -> (objectResponse)", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que el Tenant solicitado no tiene permiso para usar el conector " + 
            "/ Returns this message when the Tenant cant access the Connector", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays " + 
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [HttpGet]
        [AllowAnonymous]
        [WrapResult(WrapOnSuccess = false, WrapOnError = false)]
        public IHttpActionResult VerifyAllowedConnector(string tenantName,string ConnectorType)
        {
            try
            {
                CheckModelState();

                var predicate = PredicateBuilder.New<Tenant>(true);
                predicate = predicate.And(a => a.TenancyName == tenantName);
                TenantDto tenantFound = _tenantAppService.GetBy(predicate);
                if (tenantFound != null)
                {
                    Tenant ticoPayTenant = _tenantManager.GetByName("ticopay");
                    var clientPredicate = PredicateBuilder.New<Client>(true);
                        clientPredicate = clientPredicate.And(d => d.Identification == tenantFound.IdentificationNumber && d.TenantId == ticoPayTenant.Id);
                    ClientDto client = null;                    
                    using (_unitOfWorkManager.Current.SetTenantId(ticoPayTenant.Id))
                    {
                        _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant);                        
                        client = _clientAppService.Get(clientPredicate);
                        if (client == null)
                        {
                            _unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MustHaveTenant);
                            return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND, "El Tenant indicado no es cliente de Ticopay"));
                        }
                        else
                        {
                            ListResultDto<GroupDto> groups = null;
                            groups = _clientAppService.GetGroups(client.Id);
                            if (groups != null)
                            {
                                foreach (GroupDto Category in groups.Items)
                                {
                                    if (Category.Name.Contains(ConnectorType))
                                    {
                                        _unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MustHaveTenant);
                                        return Ok(new TicoPayResponseAPI<bool>(HttpStatusCode.OK, true));
                                    }
                                }
                            }
                            _unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MustHaveTenant);
                            return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND, "El Tenant no posee el Conector Habilitado"));                            
                        }
                    }                        
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND, "El Tenant indicado no existe"));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Verifica existencia de Tenant / Verify if a Tenant exist in Ticopays.
        /// </summary>
        /// <remarks>Verifica si el Tenant solicitado existe en Ticopay / Verify if a Tenant exist in Ticopays</remarks>
        /// <param name="tenantName">Nombre del Tenant a chequear / Name of the Tenant to check.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna True si el Tenant existe / Returns True if the Tenant exist -> (objectResponse)", Type= typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que el Tenant solicitado no existe en Ticopay "+
            "/ Returns this message when the Tenant is not found", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [HttpGet]
        [AllowAnonymous]
        [WrapResult(WrapOnSuccess = false, WrapOnError = false)]
        public IHttpActionResult VerifyDomain(string tenantName)
        {
            try
            {
                CheckModelState();

                var predicate = PredicateBuilder.New<Tenant>(true);
                predicate = predicate.And(a => a.TenancyName == tenantName);
                TenantDto tenantFound = _tenantAppService.GetBy(predicate);
                if (tenantFound != null)
                {
                    return Ok(new TicoPayResponseAPI<bool>(HttpStatusCode.OK, true));
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND, "El Tenant indicado no existe"));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }


        /// <summary>
        /// Autentifica al usuario y devuelve el token de sesión / Authenticates a user and returns the session token.
        /// </summary>
        /// <remarks>Autentifica un usuario en Ticopays, si las credenciales son validas devuelve el token de sesión.
        /// (El Token expira en 25 min) / Authenticates a user in Ticopays, if the credentials are valid returns a session token (The Token has a 25 min duration)</remarks>
        /// <param name="loginModel">Datos de inicio de sesión del usuario / User credentials.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna el token de la sesión / Returns the Session Token -> (objectResponse)", Type= typeof(TicoPayResponseAPI<Token>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Bad Request : Retorna mensaje de error al iniciar sesión: {'Usuario o Contraseña incorrectas' ," +
            "'El Tenant no se encuentra activo' , 'Su usuario ha sido bloqueado', 'El Tenant no existe' , 'Usuario se encuentra inactivo' ," +
            " 'Debe confirmar el correo electrónico'} "+
            "/ Returns this message when there is an error at login {Invalid user or password, Tenant is disabled, user is blocked, tenant not found, User is disabled}", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopay "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [HttpPost]
        [AllowAnonymous]
        [WrapResult(WrapOnSuccess = false, WrapOnError = false)]
        public async Task<IHttpActionResult> Authenticate(LoginModel loginModel)
        {
            try
            {
                CheckModelState();

                try
                {
                    await _tenantAppService.CheckTenantAPIPermission(loginModel.TenancyName);
                    await _tenantAppService.CheckTenantValidConfig(loginModel.TenancyName);
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ex.GetBaseException().Message));
                }

                var loginResult = await GetLoginResultAsync(
                    loginModel.UsernameOrEmailAddress,
                    loginModel.Password,
                    loginModel.TenancyName
                    );

                var ticket = new AuthenticationTicket(loginResult.Identity, new AuthenticationProperties());

                var currentUtc = new SystemClock().UtcNow;
                ticket.Properties.IssuedUtc = currentUtc;
                ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(30));
               
                Token token = new Token { TokenAuthenticate = OAuthBearerOptions.AccessTokenFormat.Protect(ticket) };
                
                return Ok(new TicoPayResponseAPI<Token>(HttpStatusCode.OK, token));
            }
            catch(UserFriendlyException ex)
            {
                if (ex.Message == "LoginFailed")
                {
                    if(ex.Details == "InvalidUserNameOrPassword")
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "Usuario o Contraseña incorrectas"));
                    }
                    else if (ex.Details == "TenantIsNotActive")
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "El Tenant no se encuentra activo"));
                    }
                    else if (ex.Details == "Your user is blocked")
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "Su usuario ha sido bloqueado"));
                    }
                    else if (ex.Details == "ThereIsNoTenantDefinedWithName")
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "El Tenant no existe"));
                    }
                    else if (ex.Details == "UserIsNotActiveAndCanNotLogin")
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "Usuario se encuentra inactivo"));
                    }
                    else if (ex.Details == "Your email address is not confirmed. You can not login")
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "Debe confirmar el correo electrónico"));
                    }                    
                    else
                    {
                        return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.BADREQUEST, ex.GetBaseException().Message));
                    }
                }
                else
                {
                    return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
                }
            }
            catch (Exception ex)
            {                
                    return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));                
            }
            
        }


        /// <summary>
        /// Autentifica al usuario (Solo para uso interno de Ticopay).
        /// </summary>
        /// <remarks>Autentifica un usuario en Ticopay (Solo para uso interno de Ticopay), si las credenciales son validas devuelve el token de sesión.
        /// (El Token expira en 25 min)</remarks>
        /// <param name="loginModel">Datos de inicio de sesión del usuario.</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [AllowAnonymous]
        [WrapResult(WrapOnSuccess = false, WrapOnError = false)]
        public async Task<IHttpActionResult> InternalAuthenticate(LoginModel loginModel)
        {
            try
            {
                CheckModelState();
                try
                {
                    await _tenantAppService.CheckTenantValidConfig(loginModel.TenancyName);
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, ex.GetBaseException().Message));
                }

                var loginResult = await GetLoginResultAsync(
                    loginModel.UsernameOrEmailAddress,
                    loginModel.Password,
                    loginModel.TenancyName
                    );

                var ticket = new AuthenticationTicket(loginResult.Identity, new AuthenticationProperties());

                var currentUtc = new SystemClock().UtcNow;
                ticket.Properties.IssuedUtc = currentUtc;
                ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(30));

                Token token = new Token { TokenAuthenticate = OAuthBearerOptions.AccessTokenFormat.Protect(ticket) };

                return Ok(new TicoPayResponseAPI<Token>(HttpStatusCode.OK, token));
            }
            catch (UserFriendlyException ex)
            {
                if (ex.Message == "LoginFailed")
                {
                    if (ex.Details == "InvalidUserNameOrPassword")
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "Usuario o Contraseña incorrectas"));
                    }
                    else if (ex.Details == "TenantIsNotActive")
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "El Tenant no se encuentra activo"));
                    }
                    else if (ex.Details == "Your user is blocked")
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "Su usuario ha sido bloqueado"));
                    }
                    else if (ex.Details == "ThereIsNoTenantDefinedWithName")
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "El Tenant no existe"));
                    }
                    else if (ex.Details == "UserIsNotActiveAndCanNotLogin")
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "Usuario se encuentra inactivo"));
                    }
                    else if (ex.Details == "Your email address is not confirmed. You can not login")
                    {
                        return Content(HttpStatusCode.BadRequest, new TicoPayResponseErrorAPI(Error.BADREQUEST, "Debe confirmar el correo electrónico"));
                    }
                    else
                    {
                        return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.BADREQUEST, ex.GetBaseException().Message));
                    }
                }
                else
                {
                    return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Extiende la duración de el tiempo de sesión de un token / Extends the session token duration time.
        /// </summary>
        /// <param name="currentToken">Token actual de sesión / Current session token.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK :Retorna el mismo token de la sesión si fue extendida, " +
            "si la sesión había expirado retorna un nuevo token con los parámetros de tiempo solicitados "+
            "/ Returns the same token if the session was successfully extended, otherwise if the session had already expired returns a new token with the requested time span -> (objectResponse)", Type= typeof(TicoPayResponseAPI<Token>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+ 
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult RefreshToken([FromBody] RefreshCredentials currentToken)
        {
            try
            {
                var ticket = OAuthBearerOptions.AccessTokenFormat.Unprotect(currentToken.Token);
                var currentUtc = new SystemClock().UtcNow;
                Token token = new Token { TokenAuthenticate = currentToken.Token };
                if (currentUtc.Add(TimeSpan.FromSeconds(30)) > ticket.Properties.ExpiresUtc)
                {
                    token.TokenAuthenticate = null;
                    if (currentToken.AdditionalTimeType == TimeLapsType.Minutes)
                    {
                        if ((currentToken.AdditionalTimeAmount >= 1.0) && (currentToken.AdditionalTimeAmount <= 120.0))
                        {
                            if (currentUtc < ticket.Properties.IssuedUtc.Value.AddMinutes(120))
                            {
                                ticket.Properties.ExpiresUtc = ticket.Properties.ExpiresUtc.Value.Add(TimeSpan.FromMinutes(currentToken.AdditionalTimeAmount));
                                token = new Token { TokenAuthenticate = OAuthBearerOptions.AccessTokenFormat.Protect(ticket) };
                            }
                        }
                    }
                    if (currentToken.AdditionalTimeType == TimeLapsType.Hours)
                    {
                        if ((currentToken.AdditionalTimeAmount >= 0.1) && (currentToken.AdditionalTimeAmount <= 24.0))
                        {
                            if (currentUtc < ticket.Properties.IssuedUtc.Value.AddHours(24))
                            {
                                ticket.Properties.ExpiresUtc = ticket.Properties.ExpiresUtc.Value.Add(TimeSpan.FromHours(currentToken.AdditionalTimeAmount));
                                token = new Token { TokenAuthenticate = OAuthBearerOptions.AccessTokenFormat.Protect(ticket) };
                            }
                        }
                    }
                    if (currentToken.AdditionalTimeType == TimeLapsType.Days)
                    {
                        if ((currentToken.AdditionalTimeAmount >= 1.0) && (currentToken.AdditionalTimeAmount <= 30.0))
                        {
                            if (currentUtc < ticket.Properties.IssuedUtc.Value.AddDays(30))
                            {
                                ticket.Properties.ExpiresUtc = ticket.Properties.ExpiresUtc.Value.Add(TimeSpan.FromDays(currentToken.AdditionalTimeAmount));
                                token = new Token { TokenAuthenticate = OAuthBearerOptions.AccessTokenFormat.Protect(ticket) };
                            }
                        }

                    }
                }
                return Ok(new TicoPayResponseAPI<Token>(HttpStatusCode.OK, token));
            }
            catch(Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }            
        }



        /// <summary>
        /// Obtiene los Roles de un Usuario / Gets a user roles.
        /// </summary>
        /// <remarks>Obtiene los roles asignados a un usuario especifico del Tenant / Gets the permission roles assigned to a user</remarks>
        /// <param name="UserId">Código de identificación del usuario / User Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la lista de roles si el usuario existe / Returns the Role list if the user exist -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<UserRoleDto>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que el usuario solicitado no existe en Ticopays "+
            "/ Returns this message when the required user doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro) "+
            "/ Returns this message if you are not logged in or the token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetRolesByUser")]
        public IHttpActionResult GetRolesByUser(long UserId)
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
                }
                var roles = _roleAppService.GetUserRoles(UserId);


                if (roles == null)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                }
                else
                {
                    return Ok(new TicoPayResponseAPI<UserRoleDto>(HttpStatusCode.OK, null, roles));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Obtener los permisos de un Usuario / Gets the user Permissions.
        /// </summary>
        /// <remarks>Obtener los permisos asignados a un Usuario especifico del Tenant / Gets the permission assigned to the user.</remarks>
        /// <param name="UserId">Código del Usuario / User Id.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la lista de permisos si el usuario existe / Returns a list of permissions if the user exist-> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<PermissionListDto>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que el usuario solicitado no existe en Ticopays o no tiene permisos asignados "+ 
            "/ Returns this message when the requested user doesn't exist", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro) "+
            "/ Returns this message if you are not logged in or the session token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+ 
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetPermissionsByUser")]
        public async Task<IHttpActionResult> GetPermissionsByUser(long UserId)
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
                }
                var permisos = await _roleAppService.getPermissions(UserId);


                if (permisos == null)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                }
                else
                {
                    return Ok(new TicoPayResponseAPI<PermissionListDto>(HttpStatusCode.OK, null, permisos));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        /// <summary>
        /// Obtiene todos los Usuarios / Gets all the Tenant users.
        /// </summary>
        /// <remarks>Obtiene todos los Usuarios del Tenant / Gets all the Tenant users</remarks>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "OK : Retorna la lista de usuarios del Tenant / Returns the list of Tenant users -> (listObjectResponse)", Type = typeof(TicoPayResponseAPI<UserListDto>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Not Found : Retorna mensaje notificando que el Tenant solicitado no posee usuarios / Returns this message if the Tenant has no users", Type = typeof(TicoPayResponseErrorAPI))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized : Retorna este mensaje si un Token valido no fue enviado en el Header o el mismo expiro "+
            "/ Returns this message if you are not logged in or the session token expired", Type = typeof(TicoPayResponseAPI<bool>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error : Retorna mensaje Error interno en el Servicio de Ticopays "+
            "/ Returns this message when there is an internal error in the Ticopays Service", Type = typeof(TicoPayResponseErrorAPI))]
        [Authorize]
        [HttpGet]
        [Route("GetUsersAll")]
        public async Task<IHttpActionResult> GetUsersAll()
        {
            try
            {
                if (AbpSession.TenantId == null)
                {
                    return Content(HttpStatusCode.Unauthorized, new TicoPayResponseErrorAPI(Error.NOTAUTHORIZED));
                }
           
                var user = await _userAppService.GetUsers();

                if (user == null)
                {
                    return Content(HttpStatusCode.NotFound, new TicoPayResponseErrorAPI(Error.RECORDNOTFOUND));

                }
                else
                {
                    return Ok(new TicoPayResponseAPI<UserListDto>(HttpStatusCode.OK, null, user));
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new TicoPayResponseErrorAPI(Error.SERVERERROR, ex.GetBaseException().Message));
            }

        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _loginManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private Exception CreateExceptionForFailedLoginAttempt(AbpLoginResultType result, string usernameOrEmailAddress, string tenancyName)
        {
            switch (result)
            {
                case AbpLoginResultType.Success:
                    return new ApplicationException("Don't call this method with a success result!");
                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                    return new UserFriendlyException("LoginFailed", "InvalidUserNameOrPassword");
                case AbpLoginResultType.InvalidPassword:
                    return new UserFriendlyException("LoginFailed", "InvalidUserNameOrPassword");
                case AbpLoginResultType.InvalidTenancyName:
                    return new UserFriendlyException("LoginFailed", "ThereIsNoTenantDefinedWithName");
                case AbpLoginResultType.TenantIsNotActive:
                    return new UserFriendlyException("LoginFailed", "TenantIsNotActive");
                case AbpLoginResultType.UserIsNotActive:
                    return new UserFriendlyException("LoginFailed", "UserIsNotActiveAndCanNotLogin");
                case AbpLoginResultType.UserEmailIsNotConfirmed:
                    return new UserFriendlyException("LoginFailed", "Your email address is not confirmed. You can not login");
                case AbpLoginResultType.LockedOut:
                    return new UserFriendlyException("LoginFailed", "Your user is blocked");
                default: //Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.Warn("Unhandled login fail reason: " + result);
                    return new UserFriendlyException("LoginFailed");
            }
        }

        /// <exclude />
        protected virtual void CheckModelState()
        {
            if (!ModelState.IsValid)
            {
                throw new UserFriendlyException("Invalid request!");
            }
        }
    }
}
