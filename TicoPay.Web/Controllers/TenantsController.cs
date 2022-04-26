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
using TicoPay.Common;
using System.Reflection;
using LinqKit;
using TicoPay.Invoices.XSD;

namespace TicoPay.Web.Controllers
{
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    public class TenantsController : TicoPayControllerBase
    {
        private readonly ITenantAppService _tenantAppService;
        private readonly IUserAppService _userAppService;
        private readonly EditionManager _editionManager;
        private readonly IAddressService _addressService;
        private readonly UserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IIocResolver _iocResolver;
        private readonly int _maxFileSize = 1024;

        public TenantsController(ITenantAppService tenantAppService, IClientAppService clientAppServices, IInvoiceAppService invoiceAppServices, IUserAppService userAppService, EditionManager editionManager, IAddressService addressService, UserManager userManager, IUnitOfWorkManager unitOfWorkManager, IIocResolver iocResolver)
        {
            _tenantAppService = tenantAppService;
            _userAppService = userAppService;
            _editionManager = editionManager;
            _addressService = addressService;
            _userManager = userManager;
            _unitOfWorkManager = unitOfWorkManager;
            _iocResolver = iocResolver;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Tenants)]
        [Authorize]
        public ActionResult Index()
        {
            UpdateTenantInput output = new UpdateTenantInput();
            User user = new User();
            var edition = new TicoPayEdition();
            try
            {
                output = _tenantAppService.GetEdit(AbpSession.TenantId.GetValueOrDefault());
                
                output.LogoBase64String = output.LogoData == null ? string.Empty : "data:image/png;base64," + Convert.ToBase64String(output.LogoData, 0, output.LogoData.Length);
                output.PasswordTribunet = CryptoHelper.Desencriptar(output.PasswordTribunet);
                //if (((output.CertifiedID == null) && (output.ValidateHacienda)) || ((output.CertifiedChange) && (output.ValidateHacienda)))
                //{
                //    if (output.FileName != null)
                //    {
                //        output.CertifiedChange = false;
                //    }
                //    else
                //    {
                //        output.CertifiedChange = true;
                //        output.ErrorCode = ErrorCodeHelper.Error;
                //        output.ErrorDescription = "Debe seleccionar un certificado!";
                //    }
                //}
                //ViewBag.CostoSms = output.CostoSms;

                //user = _userAppService.GetUserByRole(StaticRoleNames.Tenants.Admin);
                output.ErrorCode = ErrorCodeHelper.None;
                output.ErrorDescription = "";

                edition = (TicoPayEdition)_editionManager.GetEdition(output.EditionId);
                if (edition != null)
                {
                    output.IsFreeEdition = edition.Name == EditionManager.FreeEditionName;
                }

            }
            catch (Exception)
            {
                output.ErrorCode = ErrorCodeHelper.Error;
                output.ErrorDescription = "Error al obtener datos.";
            }
            finally
            {
                output.DistritoID = _addressService.GetDistritoIdByBarrioId((output.BarrioId == null) ? -1 : (int)output.BarrioId);
                output.BarriosList = _addressService.GetBarriosByDistritoId(output.DistritoID);
                output.CantonID = _addressService.GetCantonIdByDistritoId(output.DistritoID);
                output.Distritos = _addressService.GetDistritosByCantonId(output.CantonID);
                output.Province = _addressService.GetAllProvincias();
                output.ProvinciaID = _addressService.GetProvinciaIdByCantonId(output.CantonID);
                output.Cantons = _addressService.GetCantonesByProvinciaId(output.ProvinciaID);
                output.EditionSelect = LoadEditions(edition);
                output.CountrySelect = _addressService.GetAllCountries();
                //output.AlternativeEmail = user.;
                if (TempData["Error"] != null)
                {
                    output.ErrorCode = ErrorCodeHelper.Ok;
                    output.ErrorDescription = TempData["Mensaje"].ToString();
                }


            }
            return View(output);
        }

        private List<TicoPayEdition> LoadEditions(TicoPayEdition edition)
        {
            var predicate = PredicateBuilder.New<TicoPayEdition>(true);
            predicate = predicate.And(d => !d.IsDeleted && (((d.CloseForSale || !d.CloseForSale) && d.Id == edition.Id) || (!(!d.CloseForSale && d.Name == edition.Name && d.Id != edition.Id) && (!d.CloseForSale && d.Name != edition.Name && d.Id != edition.Id))) && ((d.Price >= edition.Price && d.EditionType == edition.EditionType) || (d.Price >= edition.Price && edition.EditionType == TicopayEditionType.Annual && d.EditionType == TicopayEditionType.Monthly) || (d.Price >= (edition.Price * 12) && edition.EditionType == TicopayEditionType.Monthly && d.EditionType == TicopayEditionType.Annual || (d.Price >= edition.Price && edition.EditionType == d.EditionType))));

            return  _tenantAppService.GetAllTicoPayEditions().Where(predicate).OrderBy(d => d.EditionType).ThenBy(d => d.Price).ToList();
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Tenants)]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Index(UpdateTenantInput viewModel)
        {
            // Modificar datos de tenant
            var edition = (TicoPayEdition)_editionManager.GetEdition(viewModel.EditionId);
            if (AbpSession.TenantId == viewModel.Id)
            {
                try
                {
                    if (edition != null)
                    {
                        viewModel.IsFreeEdition = (edition.Name == EditionManager.FreeEditionName);
                    }
                    if (viewModel.IsFreeEdition)
                    {
                        viewModel.SmsNoficicarFacturaACobro = false;
                    }
                    if (viewModel.CostoSms < 10 && viewModel.CodigoMoneda == FacturaElectronicaResumenFacturaCodigoMoneda.CRC)
                    {
                        viewModel.CostoSms = 10;//TODO: se debe mejorar y obtener un valor por defecto del costo por SMS o el minimo de todos los tenant
                    }else if ((viewModel.CostoSms < 0.02m || viewModel.CostoSms > 1) && viewModel.CodigoMoneda == FacturaElectronicaResumenFacturaCodigoMoneda.USD)
                    {
                        viewModel.CostoSms = 0.02m;//TODO: se debe mejorar y obtener un valor por defecto del costo por SMS o el minimo de todos los tenant
                    }

                    if (viewModel.ShowServiceCodePdf)
                    {
                        viewModel.ShowServiceCodePdf = true;
                    }

                    if (viewModel.ValidateHacienda)
                    {
                        if (string.IsNullOrWhiteSpace(viewModel.UserTribunet) || (string.IsNullOrWhiteSpace(viewModel.PasswordTribunet)))
                        {
                            viewModel.ErrorCode = ErrorCodeHelper.Error;
                            viewModel.ErrorDescription = "Debe ingresar las credenciales de hacienda";
                            return View(viewModel);
                        }
                        else
                        {
                         
                            if (viewModel.TipoFirma != null)
                            {



                                if (!viewModel.TipoFirma.Equals(Tenant.FirmType.Firma))
                                {
                                    //if ((viewModel.CertifiedID == null)) //|| (viewModel.CertifiedChange)))
                                    //{
                                    if (viewModel.file != null)
                                    {
                                        string archivo = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + viewModel.file.FileName).ToLower();
                                        string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/"), System.IO.Path.GetFileName(archivo));

                                        // string path2 = Server.MapPath("~/Uploads/") +  archivo;
                                        if (!Directory.Exists(Server.MapPath("~/Uploads/")))
                                            Directory.CreateDirectory(Server.MapPath("~/Uploads/"));

                                        viewModel.file.SaveAs(Server.MapPath("~/Uploads/" + archivo));
                                        viewModel.CertifiedPath = path;
                                        viewModel.FileName = viewModel.file.FileName;
                                    }
                                    else
                                    {
                                        if ((viewModel.CertifiedID == null))
                                        {
                                            viewModel.CertifiedChange = true;
                                            viewModel.ErrorCode = ErrorCodeHelper.Error;
                                            viewModel.ErrorDescription = "Debe seleccionar un certificado!";
                                            return View(viewModel);
                                        }
                                    }
                                    //}
                                    if ((viewModel.FirmaRecurrente == null) && (viewModel.TipoFirma.ToString() == ""))
                                    {
                                        viewModel.CertifiedChange = true;
                                        viewModel.ErrorCode = ErrorCodeHelper.Error;
                                        viewModel.ErrorDescription = "Debe seleccionar una Firma Prederteminada Facturación Recurrente!";
                                        return View(viewModel);
                                    }
                                }

                            }
                            else
                            {
                                viewModel.ErrorCode = ErrorCodeHelper.Error;
                                viewModel.ErrorDescription = "Debe seleccionar un tipo de firma para facturación";
                                return View(viewModel);
                            }

                        }

                       
                    }

                    ModelState.Clear();
                    ModelState.AddValidationErrors(viewModel, _iocResolver);

                    if (ModelState.IsValid)
                    {
                        viewModel.IsTutorialCompania = true;
                        if (@viewModel.CertifiedPath != null)
                        {
                            var isValidCert = CertsHelper.IsValidX509Certificate2(@viewModel.CertifiedPath, viewModel.CertifiedPassword);
                            if (!isValidCert)
                            {
                                viewModel.ErrorCode = ErrorCodeHelper.Error;
                                viewModel.ErrorDescription = "El certificado o la clave de certificado son inválidos.";
                                return View(viewModel);
                            }
                            viewModel.CertifiedRoute = System.IO.File.ReadAllBytes(@viewModel.CertifiedPath);
                        }

                        if (viewModel.LogoFile != null && (viewModel.LogoFile.ContentLength / 1024) > _maxFileSize)
                        {
                            viewModel.ErrorCode = ErrorCodeHelper.Error;
                            viewModel.ErrorDescription = "El logo de la empresa no debe ser mayor que " + _maxFileSize / 1024 + " MB";
                            return View(viewModel);
                        }

                        await _tenantAppService.Update(viewModel);

                        if (@viewModel.CertifiedPath != null)
                        {
                            // borra el certificado del directorio temporal
                            FileInfo filecert = new FileInfo(@viewModel.CertifiedPath);
                            filecert.Delete();
                        }

                        if (viewModel.SmsNoficicarFacturaACobro && !viewModel.IsFreeEdition)
                        {
                            TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient();
                            var smsSender = await ticoTalkClient.GetSmsSender(viewModel.Name);
                            if (smsSender == null || string.IsNullOrWhiteSpace(smsSender.Name))
                            {
                                await ticoTalkClient.CreateSmsSender(viewModel.IdentificationNumber, viewModel.IdentificationType, viewModel.Name, viewModel.CodigoMoneda.ToString());
                            }
                            if (smsSender != null && (viewModel.CostoSms >= 10 && viewModel.CodigoMoneda == FacturaElectronicaResumenFacturaCodigoMoneda.CRC || viewModel.CostoSms >= 0.02m && viewModel.CodigoMoneda == FacturaElectronicaResumenFacturaCodigoMoneda.USD)) //TODO: se debe mejorar y obtener un valor por defecto del costo por SMS o el minimo de todos los tenant
                            {
                                await ticoTalkClient.UpdateCostoSms(smsSender.IdentificationNumber, viewModel.CostoSms);
                            }
                        }

                        TempData["Error"] = "OK";
                        TempData["Mensaje"] = "¡Compañía guardada exitosamente!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        viewModel.ErrorCode = ErrorCodeHelper.Error;
                        viewModel.ErrorDescription = ModelState.ToErrorMessage();
                    }
                }
                catch (AbpValidationException ex)
                {
                    ModelState.AddRange(ex.GetModelErrors());
                    viewModel.ErrorDescription = ex.GetModelErrorMessage();
                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    return View(viewModel);
                }
                catch (DbEntityValidationException ex)
                {
                    ModelState.AddRange(ex.GetModelErrors());
                    viewModel.ErrorDescription = ex.GetModelErrorMessage();
                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    return View(viewModel);
                }
                catch (Exception e)
                {
                    edition = (TicoPayEdition)_editionManager.GetEdition(viewModel.EditionId);
                    viewModel.ErrorDescription = e.Message;
                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    return View(viewModel);
                }
                finally
                {
                    viewModel.CertifiedChange = false;
                    viewModel.DistritoID = _addressService.GetDistritoIdByBarrioId((viewModel.BarrioId == null) ? -1 : (int)viewModel.BarrioId);
                    viewModel.BarriosList = _addressService.GetBarriosByDistritoId(viewModel.DistritoID);
                    viewModel.CantonID = _addressService.GetCantonIdByDistritoId(viewModel.DistritoID);
                    viewModel.Distritos = _addressService.GetDistritosByCantonId(viewModel.CantonID);
                    viewModel.Province = _addressService.GetAllProvincias();
                    viewModel.ProvinciaID = _addressService.GetProvinciaIdByCantonId(viewModel.CantonID);
                    viewModel.Cantons = _addressService.GetCantonesByProvinciaId(viewModel.ProvinciaID);
                    viewModel.EditionSelect = LoadEditions(edition);
                    viewModel.CountrySelect = _addressService.GetAllCountries();
                }
            }
            else
            {
                viewModel.CertifiedChange = false;
                viewModel.DistritoID = _addressService.GetDistritoIdByBarrioId((viewModel.BarrioId == null) ? -1 : (int)viewModel.BarrioId);
                viewModel.BarriosList = _addressService.GetBarriosByDistritoId(viewModel.DistritoID);
                viewModel.CantonID = _addressService.GetCantonIdByDistritoId(viewModel.DistritoID);
                viewModel.Distritos = _addressService.GetDistritosByCantonId(viewModel.CantonID);
                viewModel.Province = _addressService.GetAllProvincias();
                viewModel.ProvinciaID = _addressService.GetProvinciaIdByCantonId(viewModel.CantonID);
                viewModel.Cantons = _addressService.GetCantonesByProvinciaId(viewModel.ProvinciaID);
                viewModel.EditionSelect = LoadEditions(edition);
                viewModel.CountrySelect = _addressService.GetAllCountries();

                viewModel.ErrorDescription = "Opción no permitida";
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                return View(viewModel);
            }

            return View(viewModel);
        }

        //[AllowAnonymous]
        //public async Task<ActionResult> Create(string planType)
        //{
        //    Edition edition = await GetEditionByNameOrDefault(planType);

        //    var input = new CreateTenantInput();
        //    input.EditionId = edition.Id;
        //    input.EditionDisplayName = edition.DisplayName;
        //    input.EditionName = edition.Name;
        //    input.CountrySelect = _addressService.GetAllCountries();
        //    input.Province = _addressService.GetAllProvincias();
        //    return View(input);
        //}

        //[HttpPost]
        //public async Task<ActionResult> Create(CreateTenantInput input)
        //{
        //    Edition edition = await GetEditionByIdOrDefault(input.EditionName);
        //    try
        //    {
        //        ModelState.Clear();
        //        ModelState.AddValidationErrors(input, _iocResolver);

        //        if (ModelState.IsValid)
        //        {
        //            var createOutput = await _tenantAppService.CreateTenant(input);
        //            string nameAndSurname = string.Format("{0} {1}", createOutput.AdminUser.Name, createOutput.AdminUser.Surname);

        //            var provider = new DpapiDataProtectionProvider("TicoPay.Web");
        //            _userManager.UserTokenProvider = new DataProtectorTokenProvider<User, long>(provider.Create("ConfirmEmail")) as IUserTokenProvider<User, long>;
        //            _userManager.EmailService = new IdentityMessageService();

        //            using (var unitOfWork = _unitOfWorkManager.Begin())
        //            {
        //                _unitOfWorkManager.Current.SetTenantId(createOutput.Tenant.Id);
        //                string code = await _userManager.GenerateEmailConfirmationTokenAsync(createOutput.AdminUser.Id);
        //                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { tenantId = createOutput.Tenant.Id, userId = createOutput.AdminUser.Id, code = code }, protocol: Request.Url.Scheme);
        //                await _userManager.SendEmailAsync(createOutput.AdminUser.Id, "Confirmar Cuenta", GetAccountConfirmationEmailBody(nameAndSurname, createOutput.Tenant.TenancyName, createOutput.AdminUser.UserName, callbackUrl));
        //                await unitOfWork.CompleteAsync();
        //            }
        //            ViewBag.TenantName = createOutput.Tenant.TenancyName;
        //            ViewBag.UserName = createOutput.AdminUser.UserName;

        //            TicoTalkClient ticoTalkClient = TicoTalkClient.GetAuthenticatedClient();
        //            if (ticoTalkClient.IsAuthenticated)
        //            {
        //                await ticoTalkClient.CreateSmsSender(input.IdentificationNumber, input.IdentificationType, input.Name);
        //            }
        //            return View("RegisterResult");
        //        }

        //        input.EditionId = edition.Id;
        //        input.EditionDisplayName = edition.DisplayName;
        //        input.EditionName = edition.Name;
        //        input.CountrySelect = _addressService.GetAllCountries();
        //        input.Province = _addressService.GetAllProvincias();
        //        input.ErrorCode = ErrorCodeHelper.Error;
        //        input.ErrorDescription = ModelState.ToErrorMessage();
        //        return View(input);
        //    }
        //    catch (AbpValidationException ex)
        //    {
        //        ModelState.AddRange(ex.GetModelErrors());
        //        input.ErrorDescription = ex.GetModelErrorMessage();
        //        input.ErrorCode = ErrorCodeHelper.Error;
        //        return View(input);
        //    }
        //    catch (DbEntityValidationException ex)
        //    {
        //        ModelState.AddRange(ex.GetModelErrors());
        //        input.ErrorDescription = ex.GetModelErrorMessage();
        //        input.ErrorCode = ErrorCodeHelper.Error;
        //        return View(input);
        //    }
        //    catch (Exception e)
        //    {
        //        input.ErrorDescription = e.Message;
        //        input.ErrorCode = ErrorCodeHelper.Error;
        //        return View(input);
        //    }
        //    finally
        //    {
        //        input.EditionId = edition.Id;
        //        input.EditionDisplayName = edition.DisplayName;
        //        input.EditionName = edition.Name;
        //        input.DistritoID = _addressService.GetDistritoIdByBarrioId((int)input.BarrioId);
        //        input.BarriosList = _addressService.GetBarriosByDistritoId(input.DistritoID);
        //        input.CantonID = _addressService.GetCantonIdByDistritoId(input.DistritoID);
        //        input.Distritos = _addressService.GetDistritosByCantonId(input.CantonID);
        //        input.Province = _addressService.GetAllProvincias();
        //        input.ProvinciaID = _addressService.GetProvinciaIdByCantonId(input.CantonID);
        //        input.Cantons = _addressService.GetCantonesByProvinciaId(input.ProvinciaID);
        //        input.CountrySelect = _addressService.GetAllCountries();
        //    }
        //}

        public ActionResult GetCanton(int? id)
        {
            var cantones = _addressService.GetCantonesByProvinciaId(id);
            var resp = cantones.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.NombreCanton
            }).ToList();
            resp.Insert(0, new SelectListItem() { Value = "", Text = "Seleccione un Canton" });
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDistritos(int? id)
        {
            var distritos = _addressService.GetDistritosByCantonId(id);
            var resp = distritos.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.NombreDistrito
            }).ToList();

            resp.Insert(0, new SelectListItem() { Value = "", Text = "Seleccione un Distrito" });
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBarrios(int? id)
        {
            var barrio = _addressService.GetBarriosByDistritoId(id);
            var resp = barrio.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.NombreBarrio
            }).ToList();
            resp.Insert(0, new SelectListItem() { Value = "", Text = "Seleccione un Barrio" });
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        private async Task<Edition> GetEditionByIdOrDefault(string name)
        {
            Edition edition = await _editionManager.FindByNameAsync(name);
            if (edition == null)
            {
                edition = await _editionManager.FindByNameAsync(EditionManager.FreeEditionName);
            }
            if (edition == null)
            {
                edition = _editionManager.Editions.FirstOrDefault();
            }
            return edition;
        }

        private async Task<Edition> GetEditionByNameOrDefault(string planType)
        {
            Edition edition = null;
            if (!string.IsNullOrWhiteSpace(planType))
            {
                edition = await _editionManager.FindByNameAsync(planType);
            }
            if (edition == null)
            {
                edition = await _editionManager.FindByNameAsync(EditionManager.FreeEditionName);
            }

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
                        Request.Url.Host.Replace("www.", ""),
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

        public ActionResult GetLogo()
        {
            var current = _tenantAppService.GetEdit(AbpSession.TenantId.GetValueOrDefault());
            if (current != null && current.LogoData != null && current.LogoData.Length > 0)
            {
                string logoBase64String = "data:image/png;base64," + Convert.ToBase64String(current.LogoData, 0, current.LogoData.Length);
                return Json(logoBase64String, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult IsAddressCompleted()
        {
            UpdateTenantInput output = new UpdateTenantInput();
            bool IsAddressCompleted = false;
            try
            {
                output = _tenantAppService.GetEdit(AbpSession.TenantId.GetValueOrDefault());

                if (output.BarrioId != null && output.CountryID != null && output.Address != null)
                {
                    IsAddressCompleted = true;
                }
            }
            catch (Exception)
            {
                IsAddressCompleted = false;
            }
            return Json(new { IsAddressCompleted = IsAddressCompleted }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCostoSms()
        {
            var tenant = _tenantAppService.GetEdit(AbpSession.TenantId.GetValueOrDefault());
            return Json($"{tenant.CostoSms} {tenant.CodigoMoneda.ToString()}", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public FileResult Download(FileDownload file)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            byte[] fileBytes = null;
            string fileName = null;
            switch (file)
            {
                case FileDownload.Manual_Firma_Digital:
                    fileBytes = ExtractResource("TicoPay.Web.Download.Manual.pdf");
                    fileName = "Manual.pdf";
                    break;
                case FileDownload.Instalador_Firma_Digital:
                    fileBytes = ExtractResource("TicoPay.Web.Download.Setup.msi");
                    fileName = "Setup.msi";
                    break;
                default:
                    break;
            }
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public static byte[] ExtractResource(String filename)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (Stream resFilestream = a.GetManifestResourceStream(filename))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }
    }
}