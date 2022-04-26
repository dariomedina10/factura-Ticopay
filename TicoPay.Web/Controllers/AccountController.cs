using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.UI;
using Abp.Web.Mvc.Models;
using TicoPay.Authorization.Roles;
using TicoPay.MultiTenancy;
using TicoPay.Users;
using TicoPay.Web.Controllers.Results;
using TicoPay.Web.Models.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TicoPay.Web.Infrastructure;
using System.Net;
using Newtonsoft.Json.Linq;
using SendMail;
using Microsoft.Owin.Security.DataProtection;
using TicoPay.Editions;
using Abp.Authorization;
using Abp.Web.Models;
using System.Data.Entity.Validation;
using System.Text;
using TicoPay.Common;
using TicoPay.Application.Helpers;

namespace TicoPay.Web.Controllers
{
    public class AccountController : TicoPayControllerBase
    {
        private readonly TenantManager _tenantManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly IUserAppService _userAppService;
        private readonly LogInManager _loginManager;
        // private readonly IEmailAppService _emailAppService;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public AccountController(
            TenantManager tenantManager,
            UserManager userManager,
            RoleManager roleManager,
            IUnitOfWorkManager unitOfWorkManager,
            IMultiTenancyConfig multiTenancyConfig,
            IUserAppService userAppService, EditionManager editionManager,
            LogInManager loginManager)
        {
            _tenantManager = tenantManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWorkManager = unitOfWorkManager;
            _multiTenancyConfig = multiTenancyConfig;
            _userAppService = userAppService;
            _loginManager = loginManager;
        }

        #region Login / Logout

        //public AgreementConectivity GetTenantByPort(int port)
        //{

        //    var convenio = _tenantManager.GetTenantsAgreementByPort(port);

        //    return convenio.FirstOrDefault();
        //}

        public ActionResult Login(string returnUrl = "")
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = Request.ApplicationPath;
            }

            return View(
                new LoginFormViewModel
                {
                    ReturnUrl = returnUrl,
                    IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled
                });
        }

        public ActionResult RestorePassword()
        {
            RestorePasswordModel viewModel = new RestorePasswordModel();

            try
            {
                viewModel.ReCaptcha = false;
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ReCaptcha = false;
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }

            return View(viewModel);

        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> IsEmailConfirmedAsync()
        {
            var user = await _userManager.FindByNameOrEmailAsync(User.Identity.GetUserName());
            bool IsEmailConfirmed = false;
            if (user != null)
            {
                IsEmailConfirmed = user.IsEmailConfirmed;
            }
            return Json(new { IsEmailConfirmed = IsEmailConfirmed }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [UnitOfWork]
        public virtual async Task<ActionResult> RestorePassword(RestorePasswordModel viewmodel)
        {
            try
            {

                ModelState.Clear();

                //Google recaptcha        
                var response = Request["g-recaptcha-response"];
                string secretKey = "6LcGFCIUAAAAAFIDCyvW830VvEeE1755VbpBW7pz";
                var client = new WebClient();
                var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
                var obj = JObject.Parse(result);
                var status = (bool)obj.SelectToken("success");

                viewmodel.ReCaptcha = status;

                //_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                User users = await _userManager.FindByEmailAsync(viewmodel.Email);

                //if (users == null || !(_userManager.IsEmailConfirmed(users.Id)))
                //{
                //    viewmodel.ErrorCode = ErrorCodeHelper.Error;
                //    viewmodel.ErrorDescription = "El correo electrónico no se encuentra registrado o el usuario no esta confirmado";
                //    return View("RestorePassword", viewmodel);
                //}


                if ((ModelState.IsValid) && (status) && users != null)
                {

                    SendMailTP mail = new SendMailTP();

                    var provider = new DpapiDataProtectionProvider("TicoPay.Web");

                    _userManager.UserTokenProvider = new DataProtectorTokenProvider<User, long>(provider.Create("ResetPassword")) { TokenLifespan = TimeSpan.FromHours(24) } as IUserTokenProvider<User, long>;

                    string token = await _userManager.GeneratePasswordResetTokenAsync(users.Id);

                    users.PasswordResetCode = token.Substring(0, 327); // guarda el token en la BD

                    await _userManager.UpdateAsync(users);

                    string callbackUrl = Url.Action("ChangePassword", "Account", new { IdUser = users.Id, code = HttpUtility.UrlEncode(users.PasswordResetCode) }, protocol: Request.Url.Scheme);
                    await mail.SendNoReplyMailAsync(
                        new string[] { viewmodel.Email },
                        RestorePasswordModel.subject,
                        RestorePasswordModel.emailbody + "<a href=\"" + callbackUrl + "\">Crear una nueva contraseña</a>" + RestorePasswordModel.emailfooter);

                    viewmodel.ErrorCode = ErrorCodeHelper.None;
                    viewmodel.ErrorDescription = "";
                }
                else
                {
                    if (!status)
                    {
                        viewmodel.ErrorCode = ErrorCodeHelper.Error;
                        viewmodel.ErrorDescription = "Validación de Google reCaptcha fallida";
                    }
                    else
                    {
                        viewmodel.ReCaptcha = false;
                        viewmodel.ErrorCode = ErrorCodeHelper.Error;
                        viewmodel.ErrorDescription = "Error al obtener datos.";
                    }
                }

            }
            catch (Exception ex)
            {
                viewmodel.ReCaptcha = false;
                viewmodel.ErrorCode = ErrorCodeHelper.Error;
                viewmodel.ErrorDescription = "Error al obtener datos.";
            }

            return View("RestorePassword", viewmodel);

        }

        [UnitOfWork]
        public virtual async Task<ActionResult> ChangePassword(long IdUser, string code)
        {
            ChangePasswordModel viewModel = new ChangePasswordModel();

            if (IdUser == 0 || code == null)
            {
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "Inválidos parámetros para resetear la contraseña ";
                return View("ChangePassword", viewModel);
            }

            try
            {
                //_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                var user = await _userManager.FindByIdAsync(IdUser);

                if (user.PasswordResetCode == HttpUtility.UrlDecode(code))
                {
                    viewModel.UserName = user.UserName;
                    viewModel.IdUser = user.Id;
                    viewModel.CodeResetPassword = code;
                    viewModel.ErrorCode = ErrorCodeHelper.Ok;
                    viewModel.ErrorDescription = "";
                }
                else
                {
                    viewModel.ErrorCode = ErrorCodeHelper.None;
                    viewModel.ErrorDescription = "El token para resetear la contraseña ha caducado";
                }
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }

            return View("ChangePassword", viewModel);

        }

        [HttpPost]
        [UnitOfWork]
        public virtual async Task<ActionResult> ChangePassword(ChangePasswordModel viewModel)
        {
            IdentityResult result;

            try
            {
                if (ModelState.IsValid)
                {
                    // _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                    var user = await _userManager.FindByIdAsync(viewModel.IdUser);

                    result = await _userManager.ChangePasswordAsync(user, viewModel.Password);

                    if (result.Succeeded)
                    {
                        user.PasswordResetCode = null; // borra el token en la BD

                        result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            viewModel.ErrorCode = ErrorCodeHelper.None;
                            viewModel.ErrorDescription = "La contraseña ha sido restablecida exitosamente, proceda a iniciar sesión normalmente.";
                            return View("ChangePassword", viewModel);
                        }
                    }
                    else
                    {
                        viewModel.ErrorCode = ErrorCodeHelper.Error;
                        viewModel.ErrorDescription = "Error al intentar cambiar la contraseña";
                    }
                }
                else
                {
                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    viewModel.ErrorDescription = "Verifique los datos usados para cambiar la contraseña.";
                }
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al cambiar la contraseña.";
            }

            return View("ChangePassword", viewModel);

        }

        [HttpPost]
        [DisableAuditing]
        public async Task<JsonResult> Login(LoginViewModel loginModel, string returnUrl = "", string returnUrlHash = "")
        {
            int failedAccessAttemps = 0;
            long UserId = 0;
            Tenant currentTenant = null;
            try
            {
                if (string.IsNullOrWhiteSpace(loginModel.UsernameOrEmailAddress) || (string.IsNullOrWhiteSpace(loginModel.Password)))
                    return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = L("LoginFailed"), Details = "El nombre de usuario o correo electrónico y contraseña son obligatorios." } });
                CheckModelState();
            }
            catch (Exception ex)
            {

                //throw CreateExceptionForFailedLoginAttempt(Abp.Authorization.AbpLoginResultType.InvalidUserNameOrEmailAddress, loginModel.UsernameOrEmailAddress, loginModel.TenancyName);
                return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = L("LoginFailed"), Details = ex.Message } });
            }

            try
            {
                if (AbpSession.TenantId != null)
                {
                    currentTenant = await _tenantManager.GetByIdAsync(AbpSession.TenantId.GetValueOrDefault());
                    if (currentTenant != null)
                    {
                        loginModel.TenancyName = currentTenant.TenancyName;
                    }

                    if (!currentTenant.IsActive && currentTenant.MotiveSuspension == Tenant.InactiveReason.InvoicePending)
                    {

                        throw new UserFriendlyException(L("LoginFailed"), L("TenantIsNotActive", currentTenant.TenancyName, EnumHelper.GetDescription(Tenant.InactiveReason.InvoicePending)));
                    }

                }
                else
                {
                    return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = L("LoginFailed"), Details = "El dominio de su empresa no es correcto, verifique y vuelva a intentar" } });
                }

                CurrentUnitOfWork.SetFilterParameter(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, currentTenant.Id);

                var accessUser = await _userManager.FindByNameOrEmailAsync(loginModel.UsernameOrEmailAddress);
                if (accessUser == null)
                {
                    return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = L("LoginFailed"), Details = "El usuario no existe" } });
                }


                UserId = accessUser.Id;

                failedAccessAttemps = await _userManager.GetAccessFailedCountAsync(UserId);


                var loginResult = await GetLoginResultAsync(
                    accessUser.UserName == null ? accessUser.EmailAddress : accessUser.UserName,
                    loginModel.Password,
                    loginModel.TenancyName
                    );

                await SignInAsync(loginResult.User, loginResult.Identity, loginModel.RememberMe);

                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    returnUrl = Request.ApplicationPath;
                }

                if (!string.IsNullOrWhiteSpace(returnUrlHash))
                {
                    returnUrl = returnUrl + returnUrlHash;
                }

                return Json(new AjaxResponse { TargetUrl = returnUrl });
            }
            catch (UserFriendlyException ex)
            {
                if (ex.Message.Equals("¡Error de inicio de sesión!"))
                {
                    if (UserId != 0)
                    {
                        if (_userManager.IsLockedOut(UserId))
                        {
                            return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = ex.Message, Details = "Cuenta bloqueada, intente mas tarde" } });
                        }
                        else if (failedAccessAttemps == 4)
                        {
                            return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = ex.Message, Details = "Su cuenta será bloqueada, intente en unos minutos" } });
                        }
                        else if (failedAccessAttemps <= 4 && ex.Details.Equals("Contraseña inválida"))
                        {
                            return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = ex.Message, Details = string.Format("{0} Le queda(n) {1} intento(s) antes de bloquear la cuenta", ex.Details, 5 - (failedAccessAttemps + 1)) } });
                        }
                        else
                        {
                            return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = ex.Message, Details = ex.Details } });
                        }
                    }
                    else
                    {
                        return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = ex.Message, Details = ex.Details } });
                    }
                }
                else
                {
                    return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = ex.Message, Details = ex.Details } });
                }
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse { Success = false, Error = new ErrorInfo { Message = ex.Message, Details = ex.Message } });
            }

        }

        private async Task<Abp.Authorization.Users.AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            try
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
            catch (Exception e)
            {
               
                    throw e;
               
            }
        }

        private async Task SignInAsync(User user, ClaimsIdentity identity = null, bool rememberMe = false)
        {
            if (identity == null)
            {
                identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            }

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe }, identity);
        }

        private Exception CreateExceptionForFailedLoginAttempt(AbpLoginResultType result, string usernameOrEmailAddress, string tenancyName)
        {
            switch (result)
            {
                case AbpLoginResultType.Success:
                    return new ApplicationException("Don't call this method with a success result!");
                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                    return new UserFriendlyException(L("LoginFailed"), L("InvalidUserName"));
                case AbpLoginResultType.InvalidPassword:
                    return new UserFriendlyException(L("LoginFailed"), L("InvalidPassword"));
                case AbpLoginResultType.InvalidTenancyName:
                    return new UserFriendlyException(L("LoginFailed"), L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
                case AbpLoginResultType.TenantIsNotActive:                    
                        return new UserFriendlyException(L("LoginFailed"), L("TenantIsNotActive", tenancyName, EnumHelper.GetDescription(Tenant.InactiveReason.inactive)));
                case AbpLoginResultType.UserIsNotActive:
                    return new UserFriendlyException(L("LoginFailed"), L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress));
                case AbpLoginResultType.UserEmailIsNotConfirmed:
                    return new UserFriendlyException(L("LoginFailed"), L("UserEmailIsNotConfirmed"));
                default: //Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.Warn("Unhandled login fail reason: " + result);
                    return new UserFriendlyException(L("LoginFailed"));
            }
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }

        #endregion

        #region Register

        public ActionResult Register()
        {
            return RegisterView(new RegisterViewModel());
        }

        private ActionResult RegisterView(RegisterViewModel model)
        {
            ViewBag.IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled;

            return View("Register", model);
        }

        [HttpPost]
        [UnitOfWork]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                CheckModelState();

                //Get tenancy name and tenant
                //if (!_multiTenancyConfig.IsEnabled)
                //{
                //    model.TenancyName = Tenant.DefaultTenantName;
                //}
                //else if (model.TenancyName.IsNullOrEmpty())
                //{
                //    throw new UserFriendlyException(L("TenantNameCanNotBeEmpty"));
                //}

                //var tenant = await GetActiveTenantAsync(model.TenancyName);

                //Create user
                var user = new User
                {
                    TenantId = AbpSession.GetTenantId(),
                    Name = model.Name,
                    Surname = model.Surname,
                    EmailAddress = model.EmailAddress,
                    IsActive = model.IsActive
                };

                ////Get external login info if possible
                //ExternalLoginInfo externalLoginInfo = null;
                //if (model.IsExternalLogin)
                //{
                //    externalLoginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
                //    if (externalLoginInfo == null)
                //    {
                //        throw new ApplicationException("Can not external login!");
                //    }

                //    user.Logins = new List<UserLogin>
                //    {
                //        new UserLogin
                //        {
                //            LoginProvider = externalLoginInfo.Login.LoginProvider,
                //            ProviderKey = externalLoginInfo.Login.ProviderKey
                //        }
                //    };

                //    if (model.UserName.IsNullOrEmpty())
                //    {
                //        model.UserName = model.EmailAddress;
                //    }

                //    model.Password = Users.User.CreateRandomPassword();

                //    if (string.Equals(externalLoginInfo.Email, model.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
                //    {
                //        user.IsEmailConfirmed = true;
                //    }
                //}
                //else
                //{
                //    //Username and Password are required if not external login
                //    if (model.UserName.IsNullOrEmpty() || model.Password.IsNullOrEmpty())
                //    {
                //        throw new UserFriendlyException(L("FormIsNotValidMessage"));
                //    }
                //}

                user.UserName = model.UserName;
                user.Password = new PasswordHasher().HashPassword(model.Password);

                //Switch to the tenant
                _unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant); 
                _unitOfWorkManager.Current.SetTenantId(AbpSession.GetTenantId());

                //Add default roles
                user.Roles = new List<UserRole>();
                foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    user.Roles.Add(new UserRole { RoleId = defaultRole.Id });
                }

                //Save user
                CheckErrors(await _userManager.CreateAsync(user));
                await _unitOfWorkManager.Current.SaveChangesAsync();


                //var newVm = new RegisterViewModel();
                //newVm.ErrorCode = ErrorCodeHelper.Ok;
                //newVm.ErrorDescription = "¡Grupo guardado exitosamente!";
                return PartialView("Login");
            }
            catch (UserFriendlyException ex)
            {
                ViewBag.IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled;
                ViewBag.ErrorMessage = ex.Message;

                return View("Register", model);
            }
        }

        #endregion

        #region External Login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(
                provider,
                Url.Action(
                    "ExternalLoginCallback",
                    "Account",
                    new
                    {
                        ReturnUrl = returnUrl
                    })
                );
        }

        [UnitOfWork]
        public virtual async Task<ActionResult> ExternalLoginCallback(string returnUrl, string tenancyName = "")
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            //Try to find tenancy name
            if (tenancyName.IsNullOrEmpty())
            {
                var tenants = await FindPossibleTenantsOfUserAsync(loginInfo.Login);
                switch (tenants.Count)
                {
                    case 0:
                        return await RegisterView(loginInfo);
                    case 1:
                        tenancyName = tenants[0].TenancyName;
                        break;
                    default:
                        return View("TenantSelection", new TenantSelectionViewModel
                        {
                            Action = Url.Action("ExternalLoginCallback", "Account", new { returnUrl }),
                            Tenants = tenants.MapTo<List<TenantSelectionViewModel.TenantInfo>>()
                        });
                }
            }

            var loginResult = await _loginManager.LoginAsync(loginInfo.Login, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    await SignInAsync(loginResult.User, loginResult.Identity, false);

                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        returnUrl = Url.Action("Index", "Home");
                    }

                    return Redirect(returnUrl);
                case AbpLoginResultType.UnknownExternalLogin:
                    return await RegisterView(loginInfo, tenancyName);
                default:
                    throw CreateExceptionForFailedLoginAttempt(loginResult.Result, loginInfo.Email ?? loginInfo.DefaultUserName, tenancyName);
            }
        }

        private async Task<ActionResult> RegisterView(ExternalLoginInfo loginInfo, string tenancyName = null)
        {
            var name = loginInfo.DefaultUserName;
            var surname = loginInfo.DefaultUserName;

            var extractedNameAndSurname = TryExtractNameAndSurnameFromClaims(loginInfo.ExternalIdentity.Claims.ToList(), ref name, ref surname);

            var viewModel = new RegisterViewModel
            {
                TenancyName = tenancyName,
                EmailAddress = loginInfo.Email,
                Name = name,
                Surname = surname,
                IsExternalLogin = true
            };

            if (!tenancyName.IsNullOrEmpty() && extractedNameAndSurname)
            {
                return await Register(viewModel);
            }

            return RegisterView(viewModel);
        }

        [UnitOfWork]
        protected virtual async Task<List<Tenant>> FindPossibleTenantsOfUserAsync(UserLoginInfo login)
        {
            List<User> allUsers;
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                allUsers = await _userManager.FindAllAsync(login);
            }

            return allUsers
                .Where(u => u.TenantId != null)
                .Select(u => AsyncHelper.RunSync(() => _tenantManager.FindByIdAsync(u.TenantId.Value)))
                .ToList();
        }

        private static bool TryExtractNameAndSurnameFromClaims(List<Claim> claims, ref string name, ref string surname)
        {
            string foundName = null;
            string foundSurname = null;

            var givennameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
            if (givennameClaim != null && !givennameClaim.Value.IsNullOrEmpty())
            {
                foundName = givennameClaim.Value;
            }

            var surnameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
            if (surnameClaim != null && !surnameClaim.Value.IsNullOrEmpty())
            {
                foundSurname = surnameClaim.Value;
            }

            if (foundName == null || foundSurname == null)
            {
                var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (nameClaim != null)
                {
                    var nameSurName = nameClaim.Value;
                    if (!nameSurName.IsNullOrEmpty())
                    {
                        var lastSpaceIndex = nameSurName.LastIndexOf(' ');
                        if (lastSpaceIndex < 1 || lastSpaceIndex > (nameSurName.Length - 2))
                        {
                            foundName = foundSurname = nameSurName;
                        }
                        else
                        {
                            foundName = nameSurName.Substring(0, lastSpaceIndex);
                            foundSurname = nameSurName.Substring(lastSpaceIndex);
                        }
                    }
                }
            }

            if (!foundName.IsNullOrEmpty())
            {
                name = foundName;
            }

            if (!foundSurname.IsNullOrEmpty())
            {
                surname = foundSurname;
            }

            return foundName != null && foundSurname != null;
        }

        #endregion

        #region Common private methods

        private async Task<Tenant> GetActiveTenantAsync(string tenancyName)
        {
            var tenant = await _tenantManager.FindByTenancyNameAsync(tenancyName);
            if (tenant == null)
            {
                throw new UserFriendlyException(L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L("TenantIsNotActive", tenancyName));
            }

            return tenant;
        }

        #endregion

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(int tenantId, long userId, string code)
        {
            if (userId <= 0 || code == null)
            {
                return RedirectToAction("Login");
            }
            var provider = new DpapiDataProtectionProvider("TicoPay.Web");
            _userManager.UserTokenProvider = new DataProtectorTokenProvider<User, long>(provider.Create("ConfirmEmail")) { TokenLifespan = TimeSpan.FromHours(24) } as IUserTokenProvider<User, long>;

            try
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    _unitOfWorkManager.Current.SetTenantId(tenantId);

                    var tenant = await _tenantManager.FindByIdAsync(tenantId);
                    if (tenant == null)
                    {
                        return RedirectToAction("Login");
                    }
                    var user = _userManager.FindById(userId);
                    if (user == null)
                    {
                        return RedirectToAction("Login");
                    }
                    if (!user.IsEmailConfirmed)
                    {
                        var result = await _userManager.ConfirmEmailAsync(userId, code);
                        await unitOfWork.CompleteAsync();
                        ViewBag.LoginUrl = string.Format("{0}://{1}{2}{3}",
                            Request.Url.Scheme,
                            Request.Url.Host,
                            Request.Url.Port == 80 ? string.Empty : ":" + Request.Url.Port,
                            Request.ApplicationPath);

                        var error = new ErrorViewModel();

                        if (!result.Succeeded)
                        {
                            error.ErrorInfo = new ErrorInfo(@"<p>Correo de confirmación de usuario ha vencido.</p>");
                            StringBuilder msgTokenInvalido = new StringBuilder();
                            msgTokenInvalido.AppendLine(@"<p>Para re enviar el correo de confirmación siga los siguientes pasos:</p>");
                            msgTokenInvalido.AppendLine(@"");
                            msgTokenInvalido.AppendLine(@"<p>1.- Ingrese con su nombre de usuario y contraseña a su dirección de ticopays.com.</p>");
                            msgTokenInvalido.AppendLine(@"<p>2.- Haga clic en el botón 'Re enviar' del menu superior.</p>");
                            msgTokenInvalido.AppendLine(@"<p>3.- Vaya a su buzón de correo eletrónico y busque el correo que le hemos enviado.</p>");
                            msgTokenInvalido.AppendLine(@"<p>4.- Haga clic en el botón confirmar.</p>");
                            msgTokenInvalido.AppendLine(@"");
                            error.ErrorInfo.Details = msgTokenInvalido.ToString();
                            error.Exception = new Exception("Token Error");
                        }

                        return View((result.Succeeded ? "ConfirmEmail" : "Error"), error);
                    }
                    else
                    {
                        return RedirectToAction("Login");
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login");
            }
        }
    }
}