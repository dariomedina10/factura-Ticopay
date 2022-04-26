using Abp.Application.Editions;
using Abp.Web.Mvc.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.Address;
using TicoPay.Authorization;
using TicoPay.Authorization.Roles;
using TicoPay.Clients;
using TicoPay.Editions;
using TicoPay.Invoices;
using TicoPay.MultiTenancy;
using TicoPay.MultiTenancy.Dto;
using TicoPay.Users;
using TicoPay.Web.Infrastructure;
using System.Linq;
using System.IO;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Abp.Domain.Uow;
using Abp.Runtime.Validation;
using TicoPay.Core.Common;
using System.Web;
using Abp.Dependency;
using Abp.Configuration;
using TicoPay.Common;
using TicoPay.Taxes;
using TicoPay.Taxes.Dto;
using TicoPay.Invoices.XSD;
using TicoPay.Api.Controllers;
using TicoPay.Web.Helpers;
using TicoPay.Sellers;
using Abp.Web.Mvc.Models;
using System.Text;
using Abp.Web.Models;
using TicoPay.Banks;
using TicoPay.Banks.Dto;

namespace TicoPay.Web.Controllers
{
    public class SignUpController : TicoPayControllerBase
    {
        private readonly ITenantAppService _tenantAppService;
        private readonly IUserAppService _userAppService;
        private readonly EditionManager _editionManager;
        private readonly IAddressService _addressService;
        private readonly UserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IIocResolver _iocResolver;
        private readonly ISettingManager _settingManager;
        private readonly IClientManager _clientManager;
        private readonly ITaxAppService _taxAppService;
        private readonly ISellerAppService _sellerAppService;
        private readonly IBankAppService _bankAppService;

        public SignUpController(IBankAppService bankAppService, ITenantAppService tenantAppService, IClientAppService clientAppServices, IInvoiceAppService invoiceAppServices, IUserAppService userAppService, EditionManager editionManager, IAddressService addressService, UserManager userManager, IUnitOfWorkManager unitOfWorkManager, IIocResolver iocResolver, ISettingManager settingManager, IClientManager clientManager, ITaxAppService taxAppService, ISellerAppService sellerAppService)
        {
            _tenantAppService = tenantAppService;
            _userAppService = userAppService;
            _editionManager = editionManager;
            _addressService = addressService;
            _userManager = userManager;
            _unitOfWorkManager = unitOfWorkManager;
            _iocResolver = iocResolver;
            _settingManager = settingManager;
            _clientManager = clientManager;
            _taxAppService = taxAppService;
            _sellerAppService = sellerAppService;
            _bankAppService = bankAppService;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Create(string planType)
        {
            TicoPayEdition edition = await GetEditionByNameOrDefault(planType);

            if (edition == null)
            {
                return RedirecttoPlanError();
            }

            if (edition.Name != EditionManager.FreeEditionName)
            {
                var input = new CreateTenantInput();
                input.EditionId = edition.Id;
                input.EditionDisplayName = edition.DisplayName;
                input.EditionName = edition.Name;
                input.StepRegister = 1;

                return View(input);
            }
            else
            {
                return RedirectToAction("LandingPage", "Home");
            }
        }

        private ActionResult RedirecttoPlanError()
        {
            var error = new ErrorViewModel();

            error.ErrorInfo = new ErrorInfo(@"<p>el plan seleccionado no existe</p>");
            StringBuilder msgplannoexiste = new StringBuilder();
            msgplannoexiste.AppendLine(@"<p>para solicitar ayuda escriba un correo a soporte@ticopays.com</p>");
            msgplannoexiste.AppendLine(@"");
            error.ErrorInfo.Details = msgplannoexiste.ToString();
            error.Exception = new Abp.UI.UserFriendlyException("no existe el plan seleccionado");

            return View("error", error);
        }

        private ActionResult RedirectToTokenError()
        {
            var error = new ErrorViewModel();

            error.ErrorInfo = new ErrorInfo(@"<p>Parece que enviaste el mismo formulario dos veces.</p>");
            StringBuilder msgPlanNoExiste = new StringBuilder();
            msgPlanNoExiste.AppendLine(@"<p>Por favor verifique su correo electrónico y siga las instrucciones, si desea ayuda escriba un correo a soporte@ticopays.com</p>");
            msgPlanNoExiste.AppendLine(@"");
            error.ErrorInfo.Details = msgPlanNoExiste.ToString();
            error.Exception = new Abp.UI.UserFriendlyException("ha enviado el formulario dos veces");

            return View("Error", error);
        }

        private async Task<TicoPayEdition> GetEditionByNameOrDefault(string planType)
        {
            TicoPayEdition edition = null;
            if (!string.IsNullOrWhiteSpace(planType))
            {
                edition = await _editionManager.GetTicoPayEditionForSale(planType);
            }

            return edition;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public async Task<ActionResult> Create(CreateTenantInput input)
        {
            Edition edition = await GetEditionByIdOrDefault(input.EditionName);
            if (edition==null)
            {
                return RedirecttoPlanError();
            }


            var message = ModelState.ToErrorMessage();
            if (message  == "current token is used")
            {
                return RedirectToTokenError();
            }


            var modelerror = ModelState;


            try
            {
                ModelState.Clear();
                ModelState.AddValidationErrors(input, _iocResolver);
                input.Sellers = _sellerAppService.GetSellers();

                if (input.StepRegister == 0)
                {
                    input.StepRegister = 1;
                    input.EditionId = edition.Id;
                    input.EditionDisplayName = edition.DisplayName;
                    input.EditionName = edition.Name;
                    return View(input);
                }
                else if (ModelState.IsValid && input.StepRegister == 1)
                {

                    if (!_tenantAppService.validTenantClientIdentification(input))
                    {
                        input.StepRegister++;
                    }
                    else
                    {
                        input.ErrorCode = ErrorCodeHelper.Error;
                        input.ErrorDescription = "Existe un cliente con el mismo número de cédula.";
                        return View(input);
                    }

                    input.BussinesName = input.ComercialName;
                    input.EditionId = edition.Id;
                    input.EditionDisplayName = edition.DisplayName;
                    input.EditionName = edition.Name;
                    return View(input);
                }
                else if (ModelState.IsValid && input.StepRegister == 2)
                {
                    if (!_tenantAppService.IsValidTenancyName(input.TenancyName))
                    {
                        input.ErrorCode = ErrorCodeHelper.Error;
                        input.ErrorDescription = "Nombre de sub dominio es inválido.";
                        return View(input);
                    }

                    if (_tenantAppService.IsTenancyNameTaken(input.TenancyName))
                    {
                        input.ErrorCode = ErrorCodeHelper.Error;
                        input.ErrorDescription = "Nombre de sub dominio ya existe.";
                        return View(input);
                    }

                    input.BarrioId = null;
                    input.Seller = _sellerAppService.GetSeller(input.SellerCode);
                    var createOutput = await _tenantAppService.CreateTenant(input);
                    string nameAndSurname = string.Format("{0} {1}", createOutput.AdminUser.Name, createOutput.AdminUser.Surname);

                    var provider = new DpapiDataProtectionProvider("TicoPay.Web");
                    _userManager.UserTokenProvider = new DataProtectorTokenProvider<User, long>(provider.Create("ConfirmEmail")) { TokenLifespan = TimeSpan.FromHours(24) } as IUserTokenProvider<User, long>;
                    _userManager.EmailService = new IdentityMessageService();

                    using (var unitOfWork = _unitOfWorkManager.Begin())
                    {
                        _unitOfWorkManager.Current.SetTenantId(createOutput.Tenant.Id);
                        string code = await _userManager.GenerateEmailConfirmationTokenAsync(createOutput.AdminUser.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { tenantId = createOutput.Tenant.Id, userId = createOutput.AdminUser.Id, code = code }, protocol: Request.Url.Scheme);
                        code = System.Web.HttpUtility.UrlEncode(code);
                        await _userManager.SendEmailAsync(createOutput.AdminUser.Id, "Confirmar Cuenta", GetAccountConfirmationEmailBody(nameAndSurname, createOutput.Tenant.TenancyName, createOutput.AdminUser.UserName, callbackUrl));
                        await unitOfWork.CompleteAsync();
                    }
                    ViewBag.TenantName = createOutput.Tenant.TenancyName;
                    ViewBag.UserName = createOutput.AdminUser.UserName;

                    var urlRedirect = string.Format("{0}://{1}{2}Home/Index", Request.Url.Scheme, createOutput.Tenant.TenancyName + ((Request.Url.Authority.AllIndexesOf(".").Count > 1) ? Request.Url.Authority.Substring(Request.Url.Authority.IndexOf(".")) : "." + Request.Url.Authority ), Url.Content("~"));

                    ViewBag.url = urlRedirect;

                    User user = await _userManager.GetUserByIdAsync(createOutput.AdminUser.Id);

                    _settingManager.ChangeSettingForUser(user.ToUserIdentifier(), Abp.Localization.LocalizationSettingNames.DefaultLanguage, "en");

                    CreateTaxInput impExento = new CreateTaxInput { Name = "Exento", Rate = 0, TaxTypes = ImpuestoTypeCodigo.Exento };
                    CreateTaxInput impSobreVentas = new CreateTaxInput { Name = "Impuesto sobre Ventas", Rate = 13, TaxTypes = ImpuestoTypeCodigo.Impuesto_General_Sobre_Ventas };

                    _taxAppService.Create(impExento, createOutput.Tenant.Id);
                    _taxAppService.Create(impSobreVentas, createOutput.Tenant.Id);

                    //Aqui se crean los bancos 
                    CreateBankInput bank1 = new CreateBankInput { Name = "BANCO NACIONAL DE COSTA RICA", ShortName = "BNCR"};
                    CreateBankInput bank2 = new CreateBankInput { Name = "BANCO DE COSTA RICA", ShortName = "BCRI" };
                    CreateBankInput bank3 = new CreateBankInput { Name = "BANCO BAC SAN JOSE SA", ShortName = "BSNJ" };
                    CreateBankInput bank4 = new CreateBankInput { Name = "BANCO BCT S.A.", ShortName = "CCIO" };
                    CreateBankInput bank5 = new CreateBankInput { Name = "BANCO CATHAY DE COSTA RICA, S.A.", ShortName = "KTAY" };
                    CreateBankInput bank6 = new CreateBankInput { Name = "BANCO CENTRAL DE COSTA RICA", ShortName = "BCCR" };
                    CreateBankInput bank7 = new CreateBankInput { Name = "BANCO CITIBANK DE COSTA RICA S.A.", ShortName = "BACU" };
                    CreateBankInput bank8 = new CreateBankInput { Name = "BANCO GENERAL (COSTA RICA), S.A.", ShortName = "BAGE" };
                    CreateBankInput bank9 = new CreateBankInput { Name = "BANCO HSBC (COSTA RICA) S.A.", ShortName = "BXBA" };
                    CreateBankInput bank10 = new CreateBankInput { Name = "BANCO IMPROSA, S.A.", ShortName = "BIMP" };
                    CreateBankInput bank11 = new CreateBankInput { Name = "BANCO LAFISE S.A.", ShortName = "BCCE" };
                    CreateBankInput bank12 = new CreateBankInput { Name = "BANCO PROMERICA", ShortName = "PRMK" };
                    CreateBankInput bank13 = new CreateBankInput { Name = "SCOTIABANK DE COSTA RICA, S.A.", ShortName = "NOSC" };
                    CreateBankInput bank14 = new CreateBankInput { Name = "BANCO POPULAR", ShortName = "BPCR" };

                    _bankAppService.Create(bank1, createOutput.Tenant.Id);
                    _bankAppService.Create(bank2, createOutput.Tenant.Id);
                    _bankAppService.Create(bank3, createOutput.Tenant.Id);
                    _bankAppService.Create(bank4, createOutput.Tenant.Id);
                    _bankAppService.Create(bank5, createOutput.Tenant.Id);
                    _bankAppService.Create(bank6, createOutput.Tenant.Id);
                    _bankAppService.Create(bank7, createOutput.Tenant.Id);
                    _bankAppService.Create(bank8, createOutput.Tenant.Id);
                    _bankAppService.Create(bank9, createOutput.Tenant.Id);
                    _bankAppService.Create(bank10, createOutput.Tenant.Id);
                    _bankAppService.Create(bank11, createOutput.Tenant.Id);
                    _bankAppService.Create(bank12, createOutput.Tenant.Id);
                    _bankAppService.Create(bank13, createOutput.Tenant.Id);
                    _bankAppService.Create(bank14, createOutput.Tenant.Id);

                    TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient();
                    if (ticoTalkClient.IsAuthenticated)
                    {
                        await ticoTalkClient.CreateSmsSender(input.IdentificationNumber, input.IdentificationType, input.Name, input.CodigoMoneda.ToString());
                    }
                    return View("RegisterResult");
                }

                input.EditionId = edition.Id;
                input.EditionDisplayName = edition.DisplayName;
                input.EditionName = edition.Name;
                input.ErrorCode = ErrorCodeHelper.Error;
                input.ErrorDescription = ModelState.ToErrorMessage();
                return View(input);
            }
            catch (AbpValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                input.ErrorDescription = ex.GetModelErrorMessage();
                input.ErrorCode = ErrorCodeHelper.Error;
                return View(input);
            }
            catch (DbEntityValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                input.ErrorDescription = ex.GetModelErrorMessage();
                input.ErrorCode = ErrorCodeHelper.Error;
                return View(input);
            }
            catch (Exception e)
            {
                input.ErrorDescription = e.Message;
                input.ErrorCode = ErrorCodeHelper.Error;
                return View(input);
            }
            finally
            {
                input.EditionId = edition.Id;
                input.EditionDisplayName = edition.DisplayName;
                input.EditionName = edition.Name;
            }
        }



        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class PreventDuplicateRequestAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {


                if (!context.HttpContext.Request.Form.AllKeys.Contains("__RequestVerificationToken"))
                    return;

                var currentToken = context.HttpContext.Request.Form["__RequestVerificationToken"].ToString();
                var lastToken = (string)context.HttpContext.Session["LastProcessedToken"];

                if (lastToken == currentToken)
                {
                    context.Controller.ViewData.ModelState.AddModelError("TokenValidation", "current token is used");
                    return;
                }
                context.HttpContext.Session["LastProcessedToken"] = currentToken;
            }
        }

        public async Task<ActionResult> ReSendConfirmEmail(long userId)
        {
            try
            {
                var userAdmin = await _userManager.GetUserByIdAsync(userId);

                var provider = new DpapiDataProtectionProvider("TicoPay.Web");
                _userManager.UserTokenProvider = new DataProtectorTokenProvider<User, long>(provider.Create("ConfirmEmail")) { TokenLifespan = TimeSpan.FromHours(24) } as IUserTokenProvider<User, long>;
                _userManager.EmailService = new IdentityMessageService();

                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    if (AbpSession.TenantId == null)
                    {
                        return Json(new { value = -1, error = Error.NOTAUTHORIZED }, JsonRequestBehavior.AllowGet);
                    }

                    var tenant = _tenantAppService.GetById(AbpSession.TenantId.Value);

                    _unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId);
                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(userId);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { tenantId = tenant.Id, userId = userId, code = code }, protocol: Request.Url.Scheme);
                    code = System.Web.HttpUtility.UrlEncode(code);
                    await _userManager.SendEmailAsync(userId, "Confirmar Cuenta", GetAccountConfirmationEmailBody(userAdmin.Name, tenant.TenancyName, userAdmin.UserName, callbackUrl));
                    await unitOfWork.CompleteAsync();

                    return Json(new { value = 1 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { value = -1, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task<Edition> GetEditionByIdOrDefault(string name)
        {
            Edition edition = await _editionManager.GetTicoPayEditionForSale(name);
            //if (edition == null)
            //{
            //    edition = await _editionManager.FindByNameAsync(EditionManager.FreeEditionName);
            //}
            //if (edition == null)
            //{
            //    edition = _editionManager.Editions.FirstOrDefault();
            //}
            return edition;
        }


     

        private string GetAccountConfirmationEmailBody(string nameAndSurname, string tenantName, string userName, string activationLink)
        {
            string body = string.Empty;
            string templatePath = HttpContext.Server.MapPath("~/Content/_AccountConfirmationTemplate.html");
            if (System.IO.File.Exists(templatePath))
            {
                using (StreamReader vReader = new StreamReader(templatePath))
                {
                    body = vReader.ReadToEnd();
                }
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var tenantUrl = string.Format("{0}://{1}.{2}{3}{4}",
                        Request.Url.Scheme,
                        tenantName,
                        Request.Url.Host.Replace("www.", "").Replace(tenantName + ".", ""),
                        Request.Url.Port == 80 ? string.Empty : ":" + Request.Url.Port,
                        Request.ApplicationPath);

                    body = body.Replace("{PART_NameAndSurname}", nameAndSurname);
                    body = body.Replace("{PART_ActivationLink}", activationLink);
                    body = body.Replace("{PART_TenantUrl}", tenantUrl);
                    body = body.Replace("{PART_UserName}", userName);
                }
            }
            return body;
        }

    }
}