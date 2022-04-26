using Microsoft.AspNet.Identity;
using SendMail;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicoPay.Authorization.Roles;
using TicoPay.ReportClosing;
using TicoPay.ReportClosing.Dto;
using TicoPay.Web.Infrastructure;
using TicoPay.MultiTenancy;
using TicoPay.Home;
using Abp.Web.MultiTenancy;
using TicoPay.Common;
using TicoPay.Drawers;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using Abp.Web.Mvc.Models;
using Abp.Web.Models;

namespace TicoPay.Web.Controllers
{
    //[AbpMvcAuthorize]
    [Authorize]
    public class HomeController : TicoPayControllerBase
    {
        private readonly IReportClosingAppService _reportClosingAppService;
        private readonly RoleManager _roleManager;
        private readonly TenantManager _tenantManager;
        private readonly TenantAppService _tenantAppService;
        private readonly IDrawersAppService _drawersAppService;

        public HomeController(IReportClosingAppService reportClosingAppService, RoleManager roleManager, TenantManager tenantManager, TenantAppService tenantAppService, IDrawersAppService drawersAppService)
        {
            _reportClosingAppService = reportClosingAppService;
            _roleManager = roleManager;
            _tenantManager = tenantManager;
            _tenantAppService = tenantAppService;
            _drawersAppService = drawersAppService;
        }

        public async Task<ActionResult> Index()
        {
            // pregunta si tiene el perfil de hacienda

            ViewBag.IsGrantedTaxAdministration = false;
            //ViewBag.istutorialCia = false;
            //ViewBag.istutorialSer = false;
            //ViewBag.istutorialCli = false;
            if (await IsGrantedTaxAdministration())
            {
                ViewBag.IsGrantedTaxAdministration = true;
                return RedirectToAction("Index", "ReportTaxAdministration");
            }
            DashboardDto model = new DashboardDto();

            
            try
            {

                if (AbpSession.TenantId != null)
                {
                    var currentTenant = await _tenantManager.GetByIdAsync(AbpSession.TenantId.GetValueOrDefault()); //_tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());


                    if (currentTenant != null)
                    {
                        model.IsTutorialCompania = currentTenant.IsTutorialCompania;
                        model.IsTutorialClients = currentTenant.IsTutorialClients;
                        model.IsTutotialServices = currentTenant.IsTutotialServices;
                        model.IsTutorialProduct = currentTenant.IsTutorialProduct;
                        if (TempData["Moneda"] == null)
                            model.Moneda = currentTenant.CodigoMoneda;
                        else
                        {
                            if (TempData["Moneda"].Equals("USD"))
                                model.Moneda = Invoices.XSD.FacturaElectronicaResumenFacturaCodigoMoneda.USD;
                            else
                                model.Moneda = Invoices.XSD.FacturaElectronicaResumenFacturaCodigoMoneda.CRC;
                        }

                    }
                }
            if (IsGranted("Dashboard"))
            {
                DateTime tempDate = DateTimeZone.Now();
                model.InitialDate = new DateTime(tempDate.Year, 1, 1, 00, 00, 00);
                model.FinalDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 23, 59, 00);

                var drawerUser = _drawersAppService.getUserDrawers(null);
                model.BranchOffice = _drawersAppService.getUserbranch().ToList();
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();

                if (TempData["DrawerId"] != null && TempData["DrawerId"].ToString() != string.Empty)
                {
                    Guid DrawerId;
                    if (Guid.TryParse(TempData["DrawerId"].ToString(), out DrawerId))
                        model.DrawerId = DrawerId;
                }

                if (TempData["BranchOfficeId"] != null && TempData["BranchOfficeId"].ToString() != string.Empty)
                {
                    Guid BranchOfficeId;
                    if (Guid.TryParse(TempData["BranchOfficeId"].ToString(), out BranchOfficeId))
                        model.BranchOfficeId = BranchOfficeId;
                }


                model.InvoicesList = _reportClosingAppService.GetInvoices(model.InitialDate, model.FinalDate, model.Moneda, model.BranchOfficeId, model.DrawerId);
                model.ClientCount = _reportClosingAppService.GetCountClientsActive();
                model.UserCount = _reportClosingAppService.GetCountUsersActive();
                model.PaymentList = _reportClosingAppService.GetPayments(model.InitialDate, model.FinalDate, model.Moneda, model.BranchOfficeId, model.DrawerId);
                model.NoteCreditList = _reportClosingAppService.GetNoteCredit(model.InitialDate, model.FinalDate, model.Moneda, model.BranchOfficeId, model.DrawerId);
                model.NoteDebitList = _reportClosingAppService.GetNoteDebit(model.InitialDate, model.FinalDate, model.Moneda, model.BranchOfficeId, model.DrawerId);
                model.IsUSDOfCRC = _reportClosingAppService.GetUSDOfCRC(model.Moneda);


                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }

               
            }
            catch (Exception ex)
            {
                //model.ErrorCode = ErrorCodeHelper.Error;
                //model.ErrorDescription = "Error al cargar el Dashboard TicoPay. " + ex.Message;

                return View("Error", new ErrorViewModel { ErrorInfo = new ErrorInfo(-1, ex.Message) });
            }
            

       
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult LandingPage()
        {
            if (User.Identity.IsAuthenticated || (AbpSession.TenantId != null && AbpSession.TenantId > 0))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> Contact(ContactDTO view)
        {
            var response = Request["g-recaptcha-response"];
            string secretKey = "6LcGFCIUAAAAAFIDCyvW830VvEeE1755VbpBW7pz";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            view.ReCaptcha = status;

           if(status)
                ModelState.Clear();

            if (ModelState.IsValid && (status))
            {
                StringBuilder body = new StringBuilder();
                body.AppendLine("<div>");
                body.AppendLine(string.Format("<p><strong>Propecto:</strong> {0}</p>", view.Name));
                body.AppendLine(string.Format("<p><strong>Email de Contacto:</strong> {0}</p>", view.Email));
                body.AppendLine(string.Format("<p><strong>Télefono de Contacto:</strong> {0}</p>", view.Telefono));
                body.AppendLine(string.Format("<p><strong>Solicitud:</strong> {0}</p>", view.Subject));
                body.AppendLine("<p><strong>Origen:</strong> Landing Page</p>");
                body.AppendLine(string.Format("<p><strong>Mensaje:</strong> {0}</p>", view.Message));
                body.AppendLine(string.Format("<p><strong>IP:</strong> {0} </p>", (!string.IsNullOrEmpty(Request.UserHostAddress) ? Request.UserHostAddress : "unknow")));
                body.AppendLine("</div>");

                SendMailTP mail = new SendMailTP();
                await mail.SendNoReplyMailAsync(new string[] { "info@ticopays.com" }, "CONTACTO: " + view.Subject, body.ToString());

                return RedirectToAction("LandingPage");
            }
            else
            {
                ViewBag.isContactFormRequest = true;

                return View("LandingPage");
            }
        }

        [AllowAnonymous]
        public ActionResult TermsAndConditions()
        {
            return PartialView("_TermsAndConditions");
        }

        [AllowAnonymous]
        public ActionResult AboutUs()
        {
            return PartialView("_AboutUs");
        }

        private async Task<bool> IsGrantedTaxAdministration()
        {
            var userRoleId = _roleManager.GetRoleByUser(User.Identity.GetUserId<long>());
            var role = await _roleManager.GetRoleByIdAsync(userRoleId);
           
            if (role != null && role.Name == StaticRoleNames.Tenants.TaxAdministration)
            {
                return true;
            }
            return false;
        }

        [AllowAnonymous]
        public ActionResult RedirectLogin(string TenancyName, string Domain, string protocolo)
        {
            if (_tenantAppService.IsTenancyNameTaken(TenancyName))
            {
                string ruta = "{0}//{1}.{2}/Account/Login";                
                ruta = string.Format(ruta, protocolo,TenancyName, Domain);
                return Json(new { success=true, message= ruta }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "<strong>Sub-dominio no válido</strong>" }, JsonRequestBehavior.AllowGet);
            
        }

        [AllowAnonymous]
        public ActionResult GetMoneda(string id, string DrawerId, string BranchOfficeId)
        {
            TempData["Moneda"] = id;
            TempData["DrawerId"] = DrawerId;
            TempData["BranchOfficeId"] = BranchOfficeId;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Error()
        {
            return View("Error");
        }
    }
}