using Abp.Domain.Uow;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.Invoices.Dto;
using TicoPay.MultiTenancy;
using TicoPay.Users;
using TicoPay.Web.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TicoPay.Clients.Dto;
using Abp.Dependency;
using TicoPay.Services;
using TicoPay.Services.Dto;
using TicoPay.Invoices.XSD;
using System.Data.Entity.Validation;
using TicoPay.Application.Helpers;
using Abp.Web.Mvc.Models;
using Abp.Web.Models;
using static TicoPay.MultiTenancy.Tenant;
using Abp.UI;
using TicoPay.Common;
using TicoPay.Web.Models.Invoice;
using TicoPay.Inventory;
using TicoPay.Inventory.Dto;
using TicoPay.Printers;
using System.Web;
using System.Transactions;
using TicoPay.Drawers;
using TicoPay.BranchOffices;
using Abp.Domain.Repositories;

namespace TicoPay.Web.Controllers
{
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    [Authorize]
    public class InvoiceController : TicoPayControllerBase
    {
        public readonly IInvoiceAppService _invoiceAppClient;
        public readonly IClientAppService _clientAppClient;
        public readonly IServiceAppService _serviceAppService;
        public readonly IInventoryAppServices _inventoryAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IIocResolver _iocResolver;
        private readonly TenantManager _tenantManager;
        private readonly UserManager _userManager;
        private readonly IClientManager _clientManager;
        public readonly IDrawersAppService _drawersAppService;
        private readonly IRepository<BranchOffice, Guid> _branchOfficeRepository;


        public const decimal rango = (decimal)0.1;

        public InvoiceController(IInvoiceAppService invoiceAppClient, IClientAppService clientAppClient, IUnitOfWorkManager unitOfWorkManager,
            IServiceAppService serviceAppService, IIocResolver iocResolver, TenantManager tenantManager, UserManager userManager, IClientManager clientManager, IInventoryAppServices inventoryAppService, IDrawersAppService drawersAppService, IRepository<BranchOffice, Guid> branchOfficeRepository)
        {
            _invoiceAppClient = invoiceAppClient;
            _clientAppClient = clientAppClient;
            _serviceAppService = serviceAppService;
            _tenantManager = tenantManager;
            _unitOfWorkManager = unitOfWorkManager;
            _userManager = userManager;
            _iocResolver = iocResolver;
            _inventoryAppService = inventoryAppService;
            _clientManager = clientManager;
            _drawersAppService = drawersAppService;
            _branchOfficeRepository = branchOfficeRepository;
        }

        public ActionResult Index(int? page )
        {
            SearchInvoicesInput model = new SearchInvoicesInput();
            model.Page = page;
            
            try
            {
                
                model.Drawer = _drawersAppService.getUserDrawersOpen();
                ViewBag.isOpenDrawer = (model.Drawer != null);
                ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintCommands", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);
                ViewBag.PaymentInvoiceBanks = PaymentInvoiceBanks();
                model.Groups = _clientManager.GetAllListGroups();
                model.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(null);
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las facturas";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }
            return View(model);
        }

        //[HttpPost]
        public ActionResult PayInvoice(int typePayment, decimal balance, string trans, Guid invoiceId) // verificar si se usa, si no borrar
        {
            try
            {
                List<PaymentInvoceDto> listPaymetnInvoce = new List<PaymentInvoceDto>();
                
                PaymentInvoceDto paymetnInvoice = new PaymentInvoceDto();
                paymetnInvoice.TypePayment=  typePayment;
                paymetnInvoice.Trans = trans;
                paymetnInvoice.Balance = balance;
                listPaymetnInvoce.Add(paymetnInvoice);               

             
                _invoiceAppClient.PayInvoiceList(listPaymetnInvoce,  invoiceId);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(-1, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult PayInvoiceList(int? typePaymentCash, decimal? balanceCash,
            int? typePaymentCreditCard, decimal? balanceCreditCard, string transCreditCard,
            int? typePaymentDeposit, decimal? balanceDeposit, string transDeposit,
            int? typePaymentCheck, decimal? balanceCheck, string transCheck, int? typePositiveBalance,
            decimal? balancePositiveBalance, Guid invoiceId, Guid? bankId, string userCard)
        {
            try
            {
                List<PaymentInvoceDto> listPaymetnInvoce = new List<PaymentInvoceDto>();

                if (typePaymentCash != null)
                {
                    PaymentInvoceDto paymetnInvoceCash = new PaymentInvoceDto();
                    paymetnInvoceCash.TypePayment = typePaymentCash.Value;
                    paymetnInvoceCash.Trans = "";
                    paymetnInvoceCash.Balance = balanceCash.Value;
                    listPaymetnInvoce.Add(paymetnInvoceCash);
                }

                if (typePaymentCreditCard != null)
                {
                    PaymentInvoceDto paymetnInvoceCreditCard = new PaymentInvoceDto();
                    paymetnInvoceCreditCard.TypePayment = typePaymentCreditCard.Value;
                    paymetnInvoceCreditCard.Trans = transCreditCard;
                    paymetnInvoceCreditCard.Balance = balanceCreditCard.Value;
                    paymetnInvoceCreditCard.UserCard = userCard;
                    listPaymetnInvoce.Add(paymetnInvoceCreditCard);
                }

                if (typePaymentDeposit != null)
                {
                    PaymentInvoceDto paymetnInvoceDeposit = new PaymentInvoceDto();
                    paymetnInvoceDeposit.TypePayment = typePaymentDeposit.Value;
                    paymetnInvoceDeposit.Trans = transDeposit;
                    paymetnInvoceDeposit.Balance = balanceDeposit.Value;
                    paymetnInvoceDeposit.BankId = bankId;
                    listPaymetnInvoce.Add(paymetnInvoceDeposit);
                }

                if (typePaymentCheck != null)
                {
                    PaymentInvoceDto paymetnInvoceCheck = new PaymentInvoceDto();
                    paymetnInvoceCheck.TypePayment = typePaymentCheck.Value;
                    paymetnInvoceCheck.Trans = transCheck;
                    paymetnInvoceCheck.Balance = balanceCheck.Value;
                    listPaymetnInvoce.Add(paymetnInvoceCheck);
                }

                if (typePositiveBalance != null)
                {
                    PaymentInvoceDto paymetnInvoceCash = new PaymentInvoceDto();
                    paymetnInvoceCash.TypePayment = typePositiveBalance.Value;
                    paymetnInvoceCash.Trans = "";
                    paymetnInvoceCash.Balance = balancePositiveBalance.Value;
                    listPaymetnInvoce.Add(paymetnInvoceCash);
                }


                _invoiceAppClient.PayInvoiceList(listPaymetnInvoce, invoiceId);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(-1, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult CalculateTotals(int Qty, decimal? discountpercentage, Guid ServiceId, int GridId)
        {
            AddServiceInvoice viewModel = new AddServiceInvoice();
            try
            {
                var service = _invoiceAppClient.GetService(ServiceId);
                string[] serviceSelect = new string[] { ServiceId.ToString() };

                decimal discountPercentageValue = 0;
                if (discountpercentage != null)
                    discountPercentageValue = discountpercentage.Value;

                var amountdesc = ((service.Price * Qty) * discountPercentageValue) / 100;

                viewModel.Services = _invoiceAppClient.GetAllServices();
                viewModel.ClientServiceList = serviceSelect; ;
                viewModel.Id = service.Id;
                viewModel.Name = service.Name;
                viewModel.Rate = service.Tax.Rate;
                viewModel.Price = service.Price;
                viewModel.TaxAmount = (((service.Price * Qty) - amountdesc) * service.Tax.Rate) / 100;
                viewModel.Quanty = Qty;
                viewModel.Total = ((service.Price * Qty) - amountdesc) + viewModel.TaxAmount;
                viewModel.DiscountAmount = discountPercentageValue;
                viewModel.GridId = GridId;
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }

            return PartialView("_AddServiceInvoice", viewModel);
        }

        public ActionResult CalculateTotalNewPrice(int Qty, decimal? discountpercentage, Guid ServiceId, int GridId, decimal price)
        {
            AddServiceInvoice viewModel = new AddServiceInvoice();
            try
            {
                var service = _invoiceAppClient.GetService(ServiceId);
                string[] serviceSelect = new string[] { ServiceId.ToString() };

                decimal discountPercentageValue = 0;
                if (discountpercentage != null)
                    discountPercentageValue = discountpercentage.Value;

                var amountdesc = ((price * Qty) * discountPercentageValue) / 100;

                viewModel.Services = _invoiceAppClient.GetAllServices();
                viewModel.ClientServiceList = serviceSelect; ;
                viewModel.Id = service.Id;
                viewModel.Name = service.Name;
                viewModel.Rate = service.Tax.Rate;
                viewModel.Price = price;
                viewModel.TaxAmount = (((price * Qty) - amountdesc) * service.Tax.Rate) / 100;
                viewModel.Quanty = Qty;
                viewModel.Total = ((price * Qty) - amountdesc) + viewModel.TaxAmount;
                viewModel.DiscountAmount = discountPercentageValue;
                viewModel.GridId = GridId;
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }

            return PartialView("_AddServiceInvoice", viewModel);
        }

        public ActionResult GetService(Guid ServiceId)
        {
            AddServiceInvoice viewModel = new AddServiceInvoice();
            try
            {
                var service = _invoiceAppClient.GetService(ServiceId);
                string[] serviceSelect = new string[] { ServiceId.ToString() };

                viewModel.Services = _invoiceAppClient.GetAllServices();
                viewModel.ClientServiceList = serviceSelect; ;
                viewModel.Id = service.Id;
                viewModel.Name = service.Name;
                viewModel.Rate = service.Tax.Rate;
                viewModel.Price = service.Price;
                viewModel.TaxAmount = (service.Price * service.Tax.Rate) / 100;
                viewModel.Quanty = 1;
                viewModel.Total = viewModel.Price + viewModel.TaxAmount;
                viewModel.DiscountAmount = 0;
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }

            return PartialView("_AddServiceInvoice", viewModel);
        }

        public ActionResult VoidInvoice(Guid clientId, Guid invoiceId)
        {
            _invoiceAppClient.VoidInvoice(invoiceId);
            return SearchInvoices(clientId, invoiceId,false,null,string.Empty);
        }


        public ActionResult ShowNotesInfo(Guid invoiceId)
        {
            var invoice = _invoiceAppClient.Get(invoiceId);
            // return PartialView("_notesPartial", invoice.Notes);
            return PartialView("_notesPartial", invoice.Notes);
        }

      

        //[HttpPost]
        //public async Task<ActionResult> Create(IList<ItemInvoice> gridData, Guid clientId, int formapago, string nrotransaccion)
        //{
        //    CreateInvoiceInput viewModel = new CreateInvoiceInput();
        //    try
        //    {
        //        User user = null;
        //        Tenant output = null;
        //        try
        //        {
        //            output = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());
        //            user = await _userManager.FindByNameOrEmailAsync(User.Identity.GetUserName());

        //            if (!(output.BarrioId != null && output.CountryID != null && output.Address != null && user.IsEmailConfirmed))
        //            {
        //                viewModel.ErrorCode = ErrorCodeHelper.CantCreateInvoices;
        //                viewModel.ErrorDescription = ((user.IsEmailConfirmed) ? "" : "Confirme su correo electrónico en su buzón de correo. ") + ((output.BarrioId != null && output.CountryID != null && output.Address != null) ? "" : "Complete su dirección fiscal.");
        //                return Json(new { value = -1, error = viewModel.ErrorDescription }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            viewModel.ErrorCode = ErrorCodeHelper.CantCreateInvoices;
        //            viewModel.ErrorDescription = "Por favor confirme su usuario y verifique su dirección fiscal.";
        //            return Json(new { value = -1, error = viewModel.ErrorDescription }, JsonRequestBehavior.AllowGet);
        //        }

        //        bool monthlyLimitReached = await _invoiceAppClient.InvoicesMonthlyLimitReachedAsync(AbpSession.TenantId.Value);
        //        if (monthlyLimitReached)
        //        {
        //            viewModel.ErrorCode = ErrorCodeHelper.InvoicesMonthlyLimitReached;
        //            viewModel.ErrorDescription = "Has alcanzado el limite mensual de facturas. Puedes obtener un plan con mayor número de facturas o esperar hasta el próximo mes.";
        //            return Json(new { value = -1, error = viewModel.ErrorDescription }, JsonRequestBehavior.AllowGet);
        //        }

        //        viewModel.InvoiceLines = gridData;
        //        viewModel.ClientId = clientId;
        //        viewModel.PaymentType = (PaymetnMethodType)formapago;
        //        viewModel.Transaction = nrotransaccion;

        //        if (ModelState.IsValid)
        //        {
        //            _invoiceAppClient.Create(viewModel);
        //            ModelState.Clear();
        //            return Json(new { value = 1 }, JsonRequestBehavior.AllowGet);
        //        }

        //        return Json(new { value = -1 }, JsonRequestBehavior.AllowGet);
        //        //viewModel.ErrorCode = ErrorCodeHelper.Error;
        //        //viewModel.ErrorDescription = "Error al intentar crear la factura!";
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { value = -1 }, JsonRequestBehavior.AllowGet);
        //    }
        //    // return View("_createPartial", viewModel);
        //}

        [HttpPost]
        [UnitOfWork(isTransactional: false, IsDisabled = true)]
        public async Task<ActionResult> CreateList(IList<ItemInvoice> gridData, Guid? clientId, int? typePaymentCash, decimal? balanceCash,
            int? typePaymentCreditCard, decimal? balanceCreditCard, string transCreditCard,
            int? typePaymentDeposit, decimal? balanceDeposit, string transDeposit,
            int? typePaymentCheck, decimal? balanceCheck, string transCheck, int? typePositiveBalance,
            decimal? balancePositiveBalance, int? typeDiscount, decimal? discount, decimal? discountPercentage, int? dayCredit, int ConditionSaleType,
            FacturaElectronicaResumenFacturaCodigoMoneda CoinType, FirmType firmType, string ClientName, TypeDocumentInvoice  TypeDocument, IdentificacionTypeTipo? IdentificationTypes, 
            string Identification, string ClientPhoneNumber, string ClientMobilNumber, string ClientEmail, Guid? bankId, string userCard, string generalObservation, string Sid, bool Iscontingencia, string ConsecutivoContigencia, DateTime Fechacontigencia, string MotivoContigencia)
        {
            CreateInvoiceInput viewModel = new CreateInvoiceInput();
            try
            {
                //User user = null;
                //Tenant output = null;
                //try
                //{
                //    output = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());
                //    user = await _userManager.FindByNameOrEmailAsync(User.Identity.GetUserName());

                //    if (!(output.BarrioId != null && output.CountryID != null && output.Address != null && user.IsEmailConfirmed))
                //    {
                //        viewModel.ErrorCode = ErrorCodeHelper.CantCreateInvoices;
                //        viewModel.ErrorDescription = (user.IsEmailConfirmed) ? "" : "Confirme su correo electrónico en su buzón de correo. " + ((output.BarrioId != null && output.CountryID != null && output.Address != null) ? "" : "Complete su dirección fiscal.");
                //        return Json(new { value = -1, error = viewModel.ErrorDescription }, JsonRequestBehavior.AllowGet);
                //    }
                //}
                //catch (Exception)
                //{
                //    viewModel.ErrorCode = ErrorCodeHelper.CantCreateInvoices;
                //    viewModel.ErrorDescription = "Por favor confirme su usuario y verifique su dirección fiscal.";
                //    return Json(new { value = -1, error = viewModel.ErrorDescription }, JsonRequestBehavior.AllowGet);
                //}

                //bool monthlyLimitReached = await _invoiceAppClient.InvoicesMonthlyLimitReachedAsync(AbpSession.TenantId.Value);
                //if (monthlyLimitReached)
                //{
                //    viewModel.ErrorCode = ErrorCodeHelper.InvoicesMonthlyLimitReached;
                //    viewModel.ErrorDescription = "Has alcanzado el limite mensual de facturas. Puedes obtener un plan con mayor número de facturas o esperar hasta el próximo mes.";
                //    return Json(new { value = -1, error = viewModel.ErrorDescription }, JsonRequestBehavior.AllowGet);
                //}

                //if ((firmType== FirmType.Llave) && (_invoiceAppClient.isdigitalPendingInvoice(output.Id)))
                //{
                //    viewModel.ErrorCode = ErrorCodeHelper.InvoicesMonthlyLimitReached;
                //    viewModel.ErrorDescription = "Posee facturas pendientes por firma digital. Por favor complete el proceso de firma y envío a hacienda de estas factura para poder generar nuevas facturas con llave criptográfica.";
                //    return Json(new { value = -1, error = viewModel.ErrorDescription }, JsonRequestBehavior.AllowGet);
                //}

                var fechaActual = DateTimeZone.Now();
                var fecha = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day);
                if ((Iscontingencia) && ((Fechacontigencia > fecha) || (Fechacontigencia < fecha.AddDays(-2))))
                {
                    viewModel.ErrorCode = ErrorCodeHelper.InvoicesMonthlyLimitReached;
                    viewModel.ErrorDescription = "La fecha de contigencia no puede ser mayor a la fecha actual ni menor de dos dias de la fecha actual.";
                    return Json(new { value = -1, error = viewModel.ErrorDescription }, JsonRequestBehavior.AllowGet);
                }

                try
                {
                        await _invoiceAppClient.TenantCantDoInvoices(User.Identity.GetUserName(), firmType);
                }
                catch (UserFriendlyException ex)
                {
                    viewModel.ErrorCode = ex.Code;
                    viewModel.ErrorDescription = ex.Message;
                    return Json(new { value = -1, error = viewModel.ErrorDescription }, JsonRequestBehavior.AllowGet);
                }

                viewModel.Drawer = _drawersAppService.getUserDrawersOpen();
                if (viewModel.Drawer == null)
                {
                    viewModel.ErrorCode = ErrorCodeHelper.Error;
                    viewModel.ErrorDescription = "No existen cajas abiertas.";
                    return Json(new { value = -1, error = viewModel.ErrorDescription }, JsonRequestBehavior.AllowGet);
                }
                ViewBag.isOpenDrawer = (viewModel.Drawer != null);

                viewModel.InvoiceLines = gridData;
               
                viewModel.ClientId = clientId;
                viewModel.DiscountGeneral = discount;
                viewModel.TypeDiscountGeneral = typeDiscount;
                viewModel.DiscountPercentage = discountPercentage;
                viewModel.ConditionSaleType = ConditionSaleType;
                viewModel.DayCredit = dayCredit;
                viewModel.Coin = CoinType;
                viewModel.TipoFirma = firmType;
                viewModel.ClientName = ClientName;
                viewModel.ClientIdentificationType = IdentificationTypes;
                viewModel.ClientIdentification = Identification;
                viewModel.ClientMobilNumber = ClientMobilNumber;
                viewModel.ClientPhoneNumber = ClientPhoneNumber;
                viewModel.ClientEmail = ClientEmail;
                viewModel.TypeDocument = TypeDocument;
                viewModel.IsContingency = Iscontingencia;
                viewModel.DateContingency = Fechacontigencia;
                viewModel.ConsecutiveNumberContingency = ConsecutivoContigencia;
                viewModel.ReasonContingency = MotivoContigencia;
                viewModel.GeneralObservation = generalObservation;


                ModelState.Clear();
                ModelState.AddValidationErrors(viewModel, _iocResolver);

                if (ModelState.IsValid)
                {
                    List<PaymentInvoceDto> listPaymetnInvoce = new List<PaymentInvoceDto>();

                    if (typePaymentCash != null)
                    {
                        PaymentInvoceDto paymetnInvoceCash = new PaymentInvoceDto();
                        paymetnInvoceCash.TypePayment = typePaymentCash.Value;
                        paymetnInvoceCash.Trans = "";
                        paymetnInvoceCash.Balance = balanceCash.Value;
                        listPaymetnInvoce.Add(paymetnInvoceCash);
                    }

                    if (typePaymentCreditCard != null)
                    {
                        PaymentInvoceDto paymetnInvoceCreditCard = new PaymentInvoceDto();
                        paymetnInvoceCreditCard.TypePayment = typePaymentCreditCard.Value;
                        paymetnInvoceCreditCard.Trans = transCreditCard;
                        paymetnInvoceCreditCard.Balance = balanceCreditCard.Value;
                        paymetnInvoceCreditCard.UserCard = userCard;
                        listPaymetnInvoce.Add(paymetnInvoceCreditCard);
                    }

                    if (typePaymentDeposit != null)
                    {
                        PaymentInvoceDto paymetnInvoceDeposit = new PaymentInvoceDto();
                        paymetnInvoceDeposit.TypePayment = typePaymentDeposit.Value;
                        paymetnInvoceDeposit.Trans = transDeposit;
                        paymetnInvoceDeposit.Balance = balanceDeposit.Value;
                        paymetnInvoceDeposit.BankId = bankId;
                        listPaymetnInvoce.Add(paymetnInvoceDeposit);
                    }

                    if (typePaymentCheck != null)
                    {
                        PaymentInvoceDto paymetnInvoceCheck = new PaymentInvoceDto();
                        paymetnInvoceCheck.TypePayment = typePaymentCheck.Value;
                        paymetnInvoceCheck.Trans = transCheck;
                        paymetnInvoceCheck.Balance = balanceCheck.Value;
                        listPaymetnInvoce.Add(paymetnInvoceCheck);
                    }

                    if (typePositiveBalance != null)
                    {
                        PaymentInvoceDto paymetnInvoceCash = new PaymentInvoceDto();
                        paymetnInvoceCash.TypePayment = typePositiveBalance.Value;
                        paymetnInvoceCash.Trans = "";
                        paymetnInvoceCash.Balance = balancePositiveBalance.Value;
                        listPaymetnInvoce.Add(paymetnInvoceCash);
                    }

                   var invoicenew= _invoiceAppClient.CreateList(viewModel, listPaymetnInvoce);
                    ModelState.Clear();
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("Print", "PdfGenerator", new { id = invoicenew.Id, type= invoicenew.TypeDocument});
                    return Json(new { value = 1, url=redirectUrl, newInvoiceId=invoicenew.Id, newInvoiceTenantId=invoicenew.TenantId, newInvoicetipoDocumento= invoicenew.TypeDocument }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { value = -1, error = ModelState.ToErrorMessage() }, JsonRequestBehavior.AllowGet);
                //viewModel.ErrorCode = ErrorCodeHelper.Error;
                //viewModel.ErrorDescription = "Error al intentar crear la factura!";
            }
            catch (Exception ex)
            {
                return Json(new { value = -1, error = ex.Message, stackTrace = ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }
            // return View("_createPartial", viewModel);
        }

        [HttpPost]
        public JsonResult getService(int page, int limit, string search)
        {
            //
            SearchServicesInput filter = new SearchServicesInput { NameFilter = search, Page = page, MaxResultCount = limit };
            SearchProductsInput filterProduct = new SearchProductsInput { NameFilter = search, Page = page, MaxResultCount = limit };
            var service = _serviceAppService.getServices(filter);
            var product = _inventoryAppService.getProducts(filterProduct);
            return Json(new { servicio = service , products= product },JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create()
        {
            CreateInvoiceInput viewModel = new CreateInvoiceInput();
            ViewBag.MaxLineDetails = -1;
            try
            {
                var tenant = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());

                if (tenant.IsPos)
                {
                    var printer = new Printer((PrinterTypes)tenant.PrinterType);
                    ViewBag.MaxLineDetails = printer.MaxLineDetails;
                }
                //ValidateTribunet validateHacienda = new ValidateTribunet();

                //string validate = validateHacienda.SendResponseTribunet("GET", null, null, null, "50612101700310174178800100001010000000070192382415", null);

                //string readText = "";
                //var encodedTextBytes = Convert.FromBase64String(readText);
                //string plainText = Encoding.UTF8.GetString(encodedTextBytes);
                viewModel.Drawer = _drawersAppService.getUserDrawersOpen();
                ViewBag.isOpenDrawer = (viewModel.Drawer != null);
                ViewBag.Taxs = _serviceAppService.GetAllTaxes();
                ViewBag.PaymentInvoiceBanks = PaymentInvoiceBanks();
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
                viewModel.PaymentTypes = SelectPaymentTypes();
                viewModel.ConditionSaleTypes = EnumHelper.GetSelectListValues(typeof(FacturaElectronicaCondicionVenta)).Take(2);
                viewModel.TipoFirma = tenant.TipoFirma;
                viewModel.FirmTypes = EnumHelper.GetSelectListValues(typeof(FirmType)).Take(2);
                viewModel.ValidateHacienda = tenant.ValidateHacienda;
                viewModel.IsInvoice = true;
                viewModel.TypeDocument = TypeDocumentInvoice.Invoice;

                viewModel.IsPos = tenant.IsPos?1:0;
                viewModel.PrinterType = tenant.PrinterType;            
                ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintCommands", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);


                viewModel.Coin = tenant.CodigoMoneda;

                bool monthlyLimitReached = await _invoiceAppClient.InvoicesMonthlyLimitReachedAsync(AbpSession.TenantId.Value);
                if (monthlyLimitReached)
                {
                    viewModel.ErrorCode = ErrorCodeHelper.InvoicesMonthlyLimitReached;
                    viewModel.ErrorDescription = "Has alcanzado el limite mensual de facturas. Puedes obtener un plan con mayor número de facturas o esperar hasta el próximo mes.";
                }
            }
            catch (Exception ex)
            {
                //viewModel.ErrorCode = ErrorCodeHelper.Error;
                //viewModel.ErrorDescription = "Error al obtener datos.";
                return View("Error", new ErrorViewModel { ErrorInfo = new ErrorInfo(-1, ex.Message) });
            }

            return View("_createPartial", viewModel);
        }

        public List<SelectListItem> PaymentInvoiceBanks()
        {
            var PaymentInvoiceBanks = _invoiceAppClient.GetAllBanks();
            var list = PaymentInvoiceBanks.Where(p => p.Name != null).Select(p => new SelectListItem { Text = p.Name, Value = p.Id.ToString() }).ToList();

            return list;
        }

        public ActionResult AddService()
        {
            AddServiceInvoice viewModel = new AddServiceInvoice();
            try
            {
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
                viewModel.Services = _invoiceAppClient.GetAllServices();
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }

            return PartialView("_AddServiceInvoice", viewModel);
        }

        public ActionResult AddClientNew()
        {
            CreateClientInput viewModel = new CreateClientInput();
            try
            {
                viewModel.ErrorCode = ErrorCodeHelper.None;
                viewModel.ErrorDescription = "";
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al obtener datos.";
            }

            return PartialView("_AddClientNewPartial", viewModel);
        }

        [HttpPost]
        public JsonResult CreateClient(string name, string lastName,
            string identification, string email, Invoices.XSD.IdentificacionTypeTipo identificationType, string identificacionExtranjero)
        {
            try
            {
                CreateClientInput client = new CreateClientInput
                {
                    Name = name,
                    LastName = lastName,
                    IdentificationType = identificationType,
                    Identification = identification,
                    Email = email,
                    IdentificacionExtranjero= identificacionExtranjero
                };

                ModelState.AddValidationErrors(client, _iocResolver);

                Client clienteNew = null;
                if (ModelState.IsValid)
                {
                    clienteNew = _clientAppClient.CreateManual(client);
                }

                if (clienteNew != null)
                    return Json(new { success = true, name = clienteNew.Name + " " + clienteNew.LastName, id = clienteNew.Id, typeIdentification= clienteNew.IdentificationType.ToString(),
                        identification= clienteNew.Identification,
                        identificacionExtranjero= clienteNew.IdentificacionExtranjero,
                        phoneNumber= clienteNew.PhoneNumber,
                        mobilNumber= clienteNew.MobilNumber,
                        email= clienteNew.Email
                    }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false, name = ModelState.ToErrorMessage(), id = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, name = e.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateService(string nameService, int CantService, decimal priceService,
            Guid taxService, string unitMeasurementsService, decimal? discountpercentage)
        {
            try
            {
                CreateServiceInput service = new CreateServiceInput
                {
                    Name = nameService,
                    Price = priceService,
                    TaxId = taxService,
                    IsRecurrent = false
                    //UnitMeasurement = (UnidadMedidaType)unitMeasurementsService
                };

                Service serviceNew = null;
                if (ModelState.IsValid)
                {
                    serviceNew = _serviceAppService.CreateServiceInvoice(service);
                }
                if (serviceNew != null)
                {
                    AddServiceInvoice viewModel = new AddServiceInvoice();
                    string[] serviceSelect = new string[] { serviceNew.Id.ToString() };

                    decimal discountPercentageValue = 0;
                    if (discountpercentage != null)
                        discountPercentageValue = discountpercentage.Value;

                    var amountdesc = ((serviceNew.Price * CantService) * discountPercentageValue) / 100;

                    viewModel.Services = _invoiceAppClient.GetAllServices();
                    viewModel.ClientServiceList = serviceSelect; ;
                    viewModel.Id = serviceNew.Id;
                    viewModel.Name = serviceNew.Name;
                    viewModel.Rate = serviceNew.Tax.Rate;
                    viewModel.Price = serviceNew.Price;
                    viewModel.TaxAmount = (((serviceNew.Price * CantService) - amountdesc) * serviceNew.Tax.Rate) / 100;
                    viewModel.Quanty = CantService;
                    viewModel.Total = ((serviceNew.Price * CantService) - amountdesc) + viewModel.TaxAmount;
                    viewModel.DiscountAmount = discountPercentageValue;

                    return Json(new { success = true, id = viewModel.Id, name = viewModel.Name, cantidad = viewModel.Quanty.ToString(), precio = viewModel.Price.ToString(), descuento = viewModel.DiscountAmount.ToString(), impuesto = viewModel.TaxAmount.ToString(), total = viewModel.Total.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = true, name = "¡Error al crear el servicio!", id = 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, name = e.Message, id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteNote(Guid invoiceId, Guid noteId)
        {
            try
            {
                _invoiceAppClient.DeleteNotes(invoiceId, noteId);

            }
            catch (Exception)
            {
                return Content("-1");
            }
            try
            {
                var invoice = _invoiceAppClient.Get(invoiceId);
                return PartialView("_notesPartial", invoice.Notes);
            }
            catch (Exception)
            {
                return Content("-2");
            }
        }

        [HttpPost]
        public ActionResult ShowClientsList(int? page, string q)
        {
            ViewBag.Query = q;
            SearchClientsInvoicesInput view = new SearchClientsInvoicesInput();
            view.Query = q;
            view.Entities = _invoiceAppClient.SearchClients(q, null);
            return PartialView("_showClientPartial", view);
        }

        public ActionResult SearchClients(string q, int? page)
        {
            ViewBag.Query = q;
            SearchClientsInvoicesInput view = new SearchClientsInvoicesInput();
            view.Query = q;
            view.Entities = _invoiceAppClient.SearchClients(q, page);
            return PartialView("_showClientPartial", view);
        }

        [HttpPost]
        public ActionResult ShowRegistersList(int? page, string q)
        {
            ViewBag.Query = q;
            SearchRegistersInvoicesInput view = new SearchRegistersInvoicesInput();
            view.Query = q;
            view.Entities = _invoiceAppClient.SearchRegisters(q, null);
            return PartialView("_showRegisterPartial", view);
        }

        public ActionResult SearchRegisters(string q, int? page)
        {
            ViewBag.Query = q;
            SearchRegistersInvoicesInput view = new SearchRegistersInvoicesInput();
            view.Query = q;
            view.Entities = _invoiceAppClient.SearchRegisters(q, page);
            return PartialView("_showRegisterPartial", view);
        }

        public ActionResult SearchInvoices(Guid? clientId, Guid? invoiceId,  bool? isComplete, int? codeError, string Error)
        {
            SearchInvoicesInput tempView = new SearchInvoicesInput();
            ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintCommands", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);
            ViewBag.PaymentInvoiceBanks = PaymentInvoiceBanks();
            try
            {
                tempView.Drawer = _drawersAppService.getUserDrawersOpen();
                ViewBag.isOpenDrawer = (tempView.Drawer != null);
                tempView.ClientId = clientId;                
                tempView.InvoiceSelect = invoiceId;
                tempView.@Entities = _invoiceAppClient.SearchInvoices(tempView);
                if (clientId != null)
                {
                    tempView.ClientInfo = _invoiceAppClient.GetClient(clientId.Value);
                    tempView.ClientName = tempView.ClientInfo.Name + " " + tempView.ClientInfo.LastName;
                }                
                tempView.Groups = _clientManager.GetAllListGroups();
                tempView.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(null);
                tempView.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();

                if (codeError!=null)
                {
                    tempView.ErrorCode = Convert.ToInt32(codeError);
                    tempView.ErrorDescription = Error;
                }
                foreach (var item in tempView.Entities)
                {
                    var line = item.InvoiceLines.FirstOrDefault();
                    IList<ListItems> newList = new List<ListItems>();
                    var list = new ListItems();
                    //var @service = _invoiceAppClient.GetService(item.InvoiceLines.FirstOrDefault().ServiceId.Value);
                    //item.ServiceId = @service.Id;
                    item.ServiceName = line.Title;
                    //item.InvoiceLines.FirstOrDefault().Service = @service;
                    list.InvoiceLines = item.InvoiceLines;
                    list.ClientId = item.ClientId;
                    list.Notes = item.Notes;
                    list.Status = item.Status;
                    list.Balance = item.Balance;
                    list.Rate = line.Tax != null ? line.Tax.Rate : 0;
                    list.CodigoMoneda = item.CodigoMoneda;
                    if (item.ClientId!=null)
                        list.PaymentInvoicesReverse = _invoiceAppClient.GetPaymentReverse(item.ClientId.Value, item.CodigoMoneda);
                    newList.Add(list);
                    item.ListItems = newList;
                }
            }
            catch (Exception)
            {

                // int hola = 1;
            }
            if (Convert.ToBoolean(isComplete))
                return View("Index", tempView);

            return PartialView("_listPartial", tempView);
        }

        public ActionResult FilterInvoices(SearchInvoicesInput viewModel)
        {
            try
            {
                ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintCommands", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);
                viewModel.Drawer = _drawersAppService.getUserDrawersOpen();
                ViewBag.isOpenDrawer = (viewModel.Drawer != null);
                viewModel.@Entities = _invoiceAppClient.SearchInvoices(viewModel);
                viewModel.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(null);
                viewModel.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                if (viewModel.ClientId != null)
                {
                    viewModel.ClientInfo = _invoiceAppClient.GetClient(viewModel.ClientId.Value);
                }
                foreach (var item in viewModel.Entities)
                {
                    var line = item.InvoiceLines.FirstOrDefault();
                    IList<ListItems> newList = new List<ListItems>();
                    var list = new ListItems();
                    //var @service = _invoiceAppClient.GetService(item.InvoiceLines.FirstOrDefault().ServiceId.Value);
                    //item.ServiceId = @service.Id;
                    item.ServiceName = line.Title;
                    //item.InvoiceLines.FirstOrDefault().Service = @service;
                    list.InvoiceLines = item.InvoiceLines;
                    list.ClientId = item.ClientId;
                    list.Notes = item.Notes;
                    list.Status = item.Status;
                    list.Balance = item.Balance;
                    list.Rate = line.Tax != null ? line.Tax.Rate : 0;
                    list.CodigoMoneda = item.CodigoMoneda;
                    if (item.ClientId != null)
                        list.PaymentInvoicesReverse = _invoiceAppClient.GetPaymentReverse(item.ClientId.Value, item.CodigoMoneda);
                    newList.Add(list);
                    item.ListItems = newList;
                }
            }
            catch (Exception)
            {
                viewModel.ErrorCode = ErrorCodeHelper.Error;
                viewModel.ErrorDescription = "Error al buscar las facturas";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", viewModel);
            }
            return View(viewModel);
        }


        public ActionResult AjaxPage(Guid? clientId, int? page, Status? status, DateTime? startDueDate, DateTime? endDueDate, TypeDocumentInvoice? documentType)
        {
            ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintCommands", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);
            SearchInvoicesInput model = new SearchInvoicesInput();
            model.Page = page;
            model.ClientId = clientId;
            model.StartDueDate = startDueDate;
            model.EndDueDate = endDueDate;
            model.TypeDocument = documentType;
            if (status != null)
                model.Status = (Status)status;

            try
            {
                model.Drawer = _drawersAppService.getUserDrawersOpen();
                ViewBag.isOpenDrawer = (model.Drawer != null);
                model.Entities = _invoiceAppClient.SearchInvoices(model);
                model.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(null);
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                foreach (var item in model.Entities)
                {
                    var line = item.InvoiceLines.FirstOrDefault();
                    IList<ListItems> newList = new List<ListItems>();
                    var list = new ListItems();
                    //var @service = _invoiceAppClient.GetService(item.InvoiceLines.FirstOrDefault().ServiceId.Value);
                    //item.ServiceId = @service.Id;
                    item.ServiceName = line.Title;
                    //item.InvoiceLines.FirstOrDefault().Service = @service;
                    list.InvoiceLines = item.InvoiceLines;
                    list.ClientId = item.ClientId;
                    list.Notes = item.Notes;
                    list.Status = item.Status;
                    list.Balance = item.Balance;
                    list.Rate = line.Tax != null ? line.Tax.Rate : 0;
                    list.CodigoMoneda = item.CodigoMoneda;
                    list.TypeDocument = documentType;
                    if (item.ClientId != null)
                        list.PaymentInvoicesReverse = _invoiceAppClient.GetPaymentReverse(item.ClientId.Value, item.CodigoMoneda);
                    newList.Add(list);
                    item.ListItems = newList;
                    list.SaleTotal = item.SaleTotal;
                    list.VoucherKey = item.VoucherKey;
                }
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los Clientes";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_listPartial", model);
            }
            return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmarRecepcion(int tenantId, Guid clientId, Guid invoiceId)
        {
            if (tenantId <= 0 || clientId == null || invoiceId == null)
            {
                return View("Error");
            }

            try
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    _unitOfWorkManager.Current.SetTenantId(tenantId);

                    try
                    {
                        var tenant = await _tenantManager.FindByIdAsync(tenantId);
                        if (tenant == null)
                        {
                            return View("Error", new ErrorViewModel { ErrorInfo = new ErrorInfo(1,"Tenemos problemas confirmando su factura","Compañia no existe") } );
                        }
                        //var client = _invoiceAppClient.GetClient(clientId);
                        //if (client == null)
                        //{
                        //    return View("Error", new ErrorViewModel { ErrorInfo = new ErrorInfo(2, "Tenemos problemas confirmando su factura", "Cliente no existe") });
                        //}
                        var isInvoiceReceptionConfirmed = _invoiceAppClient.IsInvoiceReceptionConfirmed(clientId, invoiceId);
                        if (!isInvoiceReceptionConfirmed)
                        {
                            await _invoiceAppClient.ConfirmInvoiceReceptionAsync(clientId, invoiceId);
                            return View("_InvoiceReceptionConfirmation");
                        }
                        else
                        {
                            return View("Error", new ErrorViewModel { ErrorInfo = new ErrorInfo(3, "Factura ya esta confirmada","Ya ha sido confirmada la recepción de tu factura.") });
                        }
                    }
                    catch (Exception ex)
                    {
                        return View("Error", new ErrorViewModel { ErrorInfo = new ErrorInfo(-1, ex.Message) });
                    }
                    finally
                    {
                        await unitOfWork.CompleteAsync();
                    }
                }
            }
            catch (Exception)
            {
            }
            return View("Error");
        }

        public JsonResult Resend(Guid id)
        {
            try
            {
                _invoiceAppClient.ResendInvoice(id);
                return Json(new { success = true, code = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, code = -1 }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ResendNote(Guid id)
        {
            try
            {
                _invoiceAppClient.ResendNote(id);
                return Json(new { success = true, code = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, code = -1 }, JsonRequestBehavior.AllowGet);
            }
        }

        public IList<SelectListItem> SelectPaymentTypes()
        {
            IList<SelectListItem> listSelectPaymentTypes = new List<SelectListItem>();

            listSelectPaymentTypes.Add(new SelectListItem { Value = ((int)PaymetnMethodType.Cash).ToString(CultureInfo.InvariantCulture), Text = "Efectivo", Selected = true });
            listSelectPaymentTypes.Add(new SelectListItem { Value = ((int)PaymetnMethodType.Card).ToString(CultureInfo.InvariantCulture), Text = "Tarjeta" });
            listSelectPaymentTypes.Add(new SelectListItem { Value = ((int)PaymetnMethodType.Check).ToString(CultureInfo.InvariantCulture), Text = "Cheque" });
            listSelectPaymentTypes.Add(new SelectListItem { Value = ((int)PaymetnMethodType.Deposit).ToString(CultureInfo.InvariantCulture), Text = "Depósito/Transferencia" });

            return listSelectPaymentTypes;
        }

        public JsonResult SearchPositiveBalance(Guid clientId, FacturaElectronicaResumenFacturaCodigoMoneda typeCoin)
        {
            try
            {
                List<Payment> paymentInvoicesReverse = new List<Payment>();
                paymentInvoicesReverse = _invoiceAppClient.GetPaymentReverse(clientId, typeCoin);
                return Json(new { success = true, code = paymentInvoicesReverse.Sum(a => a.Amount) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, code = -1 }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdatePriceService(Guid id, decimal price)
        {
            try
            {
                var service = _serviceAppService.GetEdit(id);
                UpdateServiceInput viewModel = new UpdateServiceInput();
                viewModel.Id = id;
                viewModel.Price = price;
                viewModel.Name = service.Name;


                if (ModelState.IsValid)
                {
                    _serviceAppService.Update(viewModel);

                    return Json(new { success = true, mensaje = "¡Servicio actualizado exitosamente!" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, mensaje = "Error en los datos." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                if (e.Message.Equals("Validation failed for one or more entities. See 'EntityValidationErrors' property for more details."))
                    return Json(new { success = false, mensaje = "¡Por favor verifique los datos!" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false, mensaje = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult addLine(/*Guid? invoiceID*/ IList<line> list)
        {
            //IList<line> list = null;
            //if (invoiceID != null)
            //{
            //    var invoice = _invoiceAppClient.Get((Guid)invoiceID);
            //    list = (from c in invoice.InvoiceLines
            //            where c.IsDeleted == false
            //            select new line
            //            {

            //                LineNumber = c.LineNumber,
            //                LineTotal = c.LineTotal,
            //                Note = c.Note,
            //                PricePerUnit = c.PricePerUnit,
            //                Quantity = c.Quantity,
            //                SubTotal = c.SubTotal,
            //                TaxAmount = c.TaxAmount,
            //                TaxId = c.TaxId,
            //                Title = c.Title,
            //                Total = c.Total,
            //                UnitMeasurement = c.UnitMeasurement,
            //                UnitMeasurementOthers = c.UnitMeasurementOthers

            //            }).ToList();
            //}
            LineDetailsViewModel model = new LineDetailsViewModel();
            var tenant = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());
            var unidaddefecto = tenant.UnitMeasurementDefault.Value;
            ViewBag.UnitMeasurementDefault = unidaddefecto;
            model.Lines = list;
            model.Taxes= _serviceAppService.GetAllTaxes();
            return PartialView("_linePartial", model);
        }

      

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmarRecepcionNota(int tenantId, Guid invoiceId, Guid noteId)
        {
            if (noteId == null || invoiceId == null)
            {
                return View("Error");
            }

            try
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    _unitOfWorkManager.Current.SetTenantId(tenantId);

                    var tenant = await _tenantManager.FindByIdAsync(tenantId);
                    if (tenant == null)
                    {
                        return View("Error");
                    }
                    var isNoteReceptionConfirmed = _invoiceAppClient.IsNoteReceptionConfirmed(invoiceId, noteId);
                    if (!isNoteReceptionConfirmed)
                    {
                        await _invoiceAppClient.ConfirmNoteReceptionAsync(invoiceId, noteId);
                        await unitOfWork.CompleteAsync();
                        return View("_NoteReceptionConfirmation");
                    }
                }
            }
            catch (Exception)
            {
            }
            return View("Error");
        }

        public ActionResult newNote(Guid? invoiceId, bool isReverseTotal, SearchInvoicesInput modelS)
        {
            NoteDto model = new NoteDto();
            IList<line> list = null;
            IList<line> listAux = new List<line>();
            ViewBag.MaxLineDetails = -1;
           ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintCommands", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);
            try
            {
                var tenant = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());
                if (tenant.IsPos)
                {
                var printer = new Printer((PrinterTypes)tenant.PrinterType);
                ViewBag.MaxLineDetails = printer.MaxLineDetails;
                }

                ViewBag.PrintPos = tenant.IsPos;

                if (invoiceId != null)
                {
                    var invoice = _invoiceAppClient.Get((Guid)invoiceId);
                    model.IsPos = invoice.Tenant.IsPos ? 1 : 0;
                    model.Drawer = _drawersAppService.getUserDrawersOpen();
                    ViewBag.isOpenDrawer = (model.Drawer != null);
                    model.InvoiceId = invoice.Id;
                    model.NumberInvoiceRef = invoice.ConsecutiveNumber;
                    model.ClientId = invoice.ClientId;
                    model.ClientName = invoice.ClientName;
                    model.TaxAmount = invoice.TotalTax;
                    model.Amount = invoice.Total;
                    model = getTypeFirm(model);
                    model.DocumentRef = new Document { Id = invoice.Id, ConsecutiveNumber = invoice.ConsecutiveNumber, TypeDocument = EnumHelper.GetDescription(invoice.TypeDocument), TypeDocumentCodigo= (int)invoice.TypeDocument, IsReverseTotal = isReverseTotal };
                    model.NoteType = NoteType.Credito;
                    model.NoteReasons = isReverseTotal ? NoteReason.Reversa_documento : NoteReason.Corregir_Monto_Factura;
                    model.CodigoMoneda = (NoteCodigoMoneda)invoice.CodigoMoneda;
                   
                    //var list = (from c in invoice.InvoiceLines where c.IsDeleted==false select new NoteLineDto {
                    //    CodeTypes=c.CodeTypes, LineNumber=c.LineNumber, LineTotal=c.LineTotal, LineType=c.LineType,
                    //    Note=c.Note,PricePerUnit=c.PricePerUnit, ProductId=c.ProductId, Quantity=c.Quantity, Service=c.Service,
                    //    ServiceId =c.ServiceId,
                    //     SubTotal=c.SubTotal,/* Tax=c.Tax,*/ TaxAmount= c.TaxAmount, TaxId=c.TaxId, TenantId=c.TenantId, Title=c.Title, Total=c.Total, UnitMeasurement=c.UnitMeasurement,UnitMeasurementOthers=c.UnitMeasurementOthers 

                    //}).ToList();

                    listAux = (from c in invoice.InvoiceLines
                               where c.IsDeleted == false
                               select new line
                               {
                                   LineNumber = c.LineNumber,
                                   LineTotal = c.LineTotal,
                                   Note = c.Note,
                                   PricePerUnit = c.PricePerUnit,
                                   Quantity = c.Quantity,
                                   SubTotal = c.SubTotal,
                                   TaxAmount = c.TaxAmount,
                                   TaxId = c.TaxId,
                                   Title = c.Title,
                                   Total = c.Total,
                                   DiscountPercentage = c.DiscountPercentage,
                                   UnitMeasurement = c.UnitMeasurement,
                                   UnitMeasurementOthers = c.UnitMeasurementOthers

                               }).ToList();                   

                    if (invoice.Notes.Count()>0)
                    {

                        listAux = resumirLines(listAux);
                        foreach (var item in listAux)
                        {
                            decimal precioUnit = Invoice.GetValor(item.PricePerUnit);
                            decimal cantidad = Invoice.GetValor(item.Quantity);
                            var total = Invoice.GetValor(precioUnit * cantidad);
                            if (total != item.Total)
                            {
                                item.PricePerUnit = item.Total;
                                item.Quantity = 1;
                            }

                            var newLine = item;                           
                            decimal cant = Invoice.GetValor(item.Quantity);
                            decimal precio = Invoice.GetValor(item.PricePerUnit);
                            decimal subtotal = Invoice.GetValor(item.Total);

                            foreach (var note in invoice.Notes)
                            {
                                cant += note.NotesLines.Where(y => y.Title == item.Title && item.DiscountPercentage==y.DiscountPercentage).Sum(x => x.Quantity) * (note.NoteType == NoteType.Credito ? -1 : 1);     
                                subtotal += note.NotesLines.Where(y => y.Title == item.Title && item.DiscountPercentage == y.DiscountPercentage).Sum(x => x.Total) * (note.NoteType == NoteType.Credito ? -1 : 1);
                            }
                            cant = (subtotal > 0 && cant == 0) ? item.Quantity : cant;
                            var price = cant > 0 ? Invoice.GetValor(Invoice.GetValor(subtotal) / cant) : 0;
                            if (precio == price)
                            {
                                newLine.Quantity = cant;
                                newLine.PricePerUnit = Invoice.GetValor(price);
                            }
                            else
                            {
                                //var result = cant * Invoice.GetValor(subtotal);
                                newLine.Quantity = Invoice.GetValor(subtotal) > 0 ? 1 : 0;
                                newLine.PricePerUnit = Invoice.GetValor(subtotal);
                            }
                            

                        }
                        listAux = listAux.Where(X => X.Quantity > 0).Distinct().ToList();
                    }
                   
                    // model.NotesLines = list;
                   
                }
               

            }
            catch (Exception ex)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = ex.GetBaseException().Message;
            }

            ViewBag.Line = listAux;
            return View(model);
        }

        [HttpPost]
        public ActionResult ApplyNote(NoteDto input, string Sid/*Guid invoiceId, double amount, int reason, int type, Guid clientId, decimal montotax, decimal totalNota*/)
        {
            string urlprint= string.Empty;
            Guid idNote;
            int tenantId = 0;
            SearchInvoicesInput model = new SearchInvoicesInput();
            model.ClientId = input.ClientId;
            if ((input.NoteType == NoteType.Debito) && (input.DocumentRef.TypeDocument == "Factura Electrónica"))
            {
                input.Status = Status.Parked;
                input.ConditionSaleType = FacturaElectronicaCondicionVenta.Credito;
                if (input.ClientId != null)
                {
                    var client = _clientAppClient.Get((Guid)input.ClientId);                    
                    input.CreditTerm = (client.CreditDays != null && client.CreditDays > 0) ? client.CreditDays.Value : 1;
                }else
                    input.CreditTerm =  1;

            }
            else
            {
                input.Status = Status.Completed;
                input.ConditionSaleType = FacturaElectronicaCondicionVenta.Contado;
                input.CreditTerm = 0;

            }

            input.DueDate = DateTime.Now.AddDays(input.CreditTerm);

            input.Drawer = _drawersAppService.getUserDrawersOpen();
            var isOpenDrawer = (input.Drawer != null);

            if (isOpenDrawer)
            {
                ViewBag.isOpenDrawer = true;
            }
            else
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Debe abrir una caja para realizar notas de crédito o débito..";
                return Json(new { codeError = model.ErrorCode, Error = model.ErrorDescription, Url = string.Empty });
            }

            try
            {
                if (input.Total <= 0)
                {
                    model.ErrorCode = ErrorCodeHelper.Error;
                    model.ErrorDescription = "El monto de la nota no puede ser menor a cero.";
                    // return Json(new { codeError = model.ErrorCode, Error = model.ErrorDescription, Url = string.Empty });
                }

                if (input.TipoFirma == FirmType.Llave && _invoiceAppClient.isdigitalPendingNote(AbpSession.TenantId.GetValueOrDefault()))
                {
                    model.ErrorCode = ErrorCodeHelper.Error;
                    model.ErrorDescription = "Existen Notas pendientes por firma digital.";
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("SearchInvoices", "Invoice", new { clientId = input.ClientId, invoiceId = (Guid)input.InvoiceId, isComplete = true, codeError = model.ErrorCode, Error = model.ErrorDescription });
                    return Json(new { codeError = model.ErrorCode, Error = model.ErrorDescription, Url = redirectUrl });
                }

                var fechaActual = DateTimeZone.Now();
                var fecha = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day);
                if ((input.IsContingency) && ((input.DateContingency > fecha) || (input.DateContingency < fecha.AddDays(-2))))
                {
                    model.ErrorCode = ErrorCodeHelper.Error;
                    model.ErrorDescription = "La fecha de contigencia no puede ser mayor a la fecha actual ni menor de dos dias de la fecha actual.";
                    return Json(new { codeError = model.ErrorCode, Error = model.ErrorDescription, Url = string.Empty });
                }

                if ((input.DocumentRef.TypeDocument == EnumHelper.GetDescription(TypeDocumentInvoice.Invoice)) || (input.DocumentRef.TypeDocument == EnumHelper.GetDescription(TypeDocumentInvoice.Ticket)))
                {
                    var invoice = _invoiceAppClient.Get((Guid)input.InvoiceId);
                    tenantId = invoice.TenantId;
                    if (input.NoteType == NoteType.Debito && invoice.Status == Status.Completed && input.NoteReasons != NoteReason.Reversa_documento)
                    {
                        model.ErrorCode = ErrorCodeHelper.Error;
                        model.ErrorDescription = "No se puede aplicar una nota de débito a una factura o tiquete pagado.";
                        var redirectUrl = new UrlHelper(Request.RequestContext).Action("SearchInvoices", "Invoice", new { clientId = input.ClientId, invoiceId = (Guid)input.InvoiceId, isComplete = true, codeError = model.ErrorCode, Error = model.ErrorDescription });
                        return Json(new { codeError = model.ErrorCode, Error = model.ErrorDescription, Url = redirectUrl });
                    }

                    using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
                    {
                        Scope = TransactionScopeOption.RequiresNew,
                        IsTransactional = true,
                        IsolationLevel = IsolationLevel.ReadUncommitted
                    }))
                    {
                        var notenew = _invoiceAppClient.CreateNote(input);
                        idNote = notenew.Id;
                        _unitOfWorkManager.Current.SaveChanges();
                        unitOfWork.Complete();
                    }

                    var note = _invoiceAppClient.GetNote(idNote);
                    var document = string.Empty;
                    urlprint = new UrlHelper(Request.RequestContext).Action("Print", "PdfGenerator", new { id = idNote, type = note.NoteType==NoteType.Credito? TypeDocumentInvoice.NotaCredito: TypeDocumentInvoice.NotaDebito });
                }
                else
                {
                    var originNote = _invoiceAppClient.GetNote(input.DocumentRef.Id);
                    decimal amountAlreadyReversed = _invoiceAppClient.GetNoteAmountAlreadyReversed(input.DocumentRef.ConsecutiveNumber);

                    if (amountAlreadyReversed >= originNote.Total)
                    {
                        /// model.CanBeReversed = false;
                        model.ErrorCode = ErrorCodeHelper.Error;
                        model.ErrorDescription = $"No se puede reversar la nota {input.DocumentRef.ConsecutiveNumber}, la misma ya fue totalmente reversada.";
                        var redirectUrl = new UrlHelper(Request.RequestContext).Action("SearchInvoices", "Invoice", new { clientId = input.ClientId, invoiceId = (Guid)input.InvoiceId, isComplete = true, codeError = model.ErrorCode, Error = model.ErrorDescription });
                        return Json(new { codeError = model.ErrorCode, Error = model.ErrorDescription, Url = redirectUrl });


                    }
                    if (input.Total > (originNote.Total + rango))
                    {
                        model.ErrorCode = ErrorCodeHelper.Error;
                        model.ErrorDescription = $"El monto máximo permitido es {originNote.Amount - amountAlreadyReversed} ya que la nota {input.DocumentRef.ConsecutiveNumber} tiene uno o mas reversos parciales.";
                        // return Json(new { codeError = model.ErrorCode, Error = model.ErrorDescription, Url = string.Empty });

                    }
                    using (var unitOfWork = _unitOfWorkManager.Begin(new UnitOfWorkOptions
                    {
                        Scope = TransactionScopeOption.RequiresNew,
                        IsTransactional = true,
                        IsolationLevel = IsolationLevel.ReadUncommitted
                    }))
                    {
                    var notenew = _invoiceAppClient.ReverseNote(input);
                    idNote = notenew.Id;
                        _unitOfWorkManager.Current.SaveChanges();
                        unitOfWork.Complete();
                    }

                    var note = _invoiceAppClient.GetNote(idNote);
                    urlprint = new UrlHelper(Request.RequestContext).Action("Print", "PdfGenerator", new { id = idNote, type = note.NoteType == NoteType.Credito ? TypeDocumentInvoice.NotaCredito : TypeDocumentInvoice.NotaDebito });
                }

                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "Nota generada correctamente";

            }
            catch (Exception ex)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = ex.Message;
                return Json(new { codeError = model.ErrorCode, Error = model.ErrorDescription, Url = string.Empty });

            }

            
            var redirect = new UrlHelper(Request.RequestContext).Action("SearchInvoices", "Invoice", new { clientId = input.ClientId, invoiceId = (Guid)input.InvoiceId, isComplete = true, codeError = model.ErrorCode, Error = model.ErrorDescription });           
            return Json(new { codeError = model.ErrorCode, Error = model.ErrorDescription, Url = redirect, Urlprint=urlprint, noteId= idNote, noteTenantId=tenantId, noteTypeDocument=TypeDocumentInvoice.NotaCredito });

            //return Content("1"); 
        }


        public ActionResult ReverseNote(Guid InvoiceId, Guid noteId, SearchInvoicesInput modelS)
        {
            ViewBag.MaxLineDetails = -1;
            ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintCommands", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);
            NoteDto model = new NoteDto();
          
            IList<line> list = null;
            try
            {
              
                var tenant = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());
                model.Drawer = _drawersAppService.getUserDrawersOpen();
                ViewBag.isOpenDrawer = (model.Drawer != null);

                if (tenant.IsPos) {
                var printer = new Printer((PrinterTypes)tenant.PrinterType);
                ViewBag.MaxLineDetails = printer.MaxLineDetails;
                }
                

                var invoice = _invoiceAppClient.Get(InvoiceId);
                if (invoice == null)
                {
                    model.ErrorCode = ErrorCodeHelper.Error;
                    model.ErrorDescription = "No se encontró la factura!";

                }
                var note = invoice.Notes.Where(n => n.Id == noteId).FirstOrDefault();

                decimal amountAlreadyReversed = _invoiceAppClient.GetNoteAmountAlreadyReversed(note.ConsecutiveNumber);
                if (amountAlreadyReversed >= note.Amount)
                {
                   // SearchInvoicesInput modelIndex = new SearchInvoicesInput();
                    model.ErrorCode = ErrorCodeHelper.Error;
                    model.ErrorDescription = $"No se puede reversar la nota {note.ConsecutiveNumber}, la misma ya fue totalmente reversada.";
                   // model.ClientId = invoice.ClientId;
                  //  var redirectUrl = new UrlHelper(Request.RequestContext).Action("SearchInvoices", "Invoice", new { clientId = invoice.ClientId, isComplete = true, codeError = model.ErrorCode, Error = model.ErrorDescription });
                  //  return Json(new { Url = redirectUrl });
                    return RedirectToAction("SearchInvoices", "Invoice", new  { clientId = invoice.ClientId, invoiceId= InvoiceId, isComplete = true, codeError = model.ErrorCode, Error = model.ErrorDescription });
                }

                list = (from c in note.NotesLines
                        where c.IsDeleted == false
                        select new line
                        {
                            LineNumber = c.LineNumber,
                            LineTotal = c.LineTotal,
                            Note = c.Notes,
                            PricePerUnit = c.PricePerUnit,
                            Quantity = c.Quantity,
                            SubTotal = c.SubTotal,
                            TaxAmount = c.TaxAmount,
                            TaxId = c.TaxId,
                            Title = c.Title,
                            Total = c.Total,
                            DiscountPercentage = c.DiscountPercentage,
                            UnitMeasurement = c.UnitMeasurement,
                            UnitMeasurementOthers = c.UnitMeasurementOthers

                        }).ToList();

                // model.NotesLines = list;
                model.InvoiceId = invoice.Id;
                model.IsPos = invoice.Tenant.IsPos ? 1 : 0;
                model.NumberInvoiceRef = invoice.ConsecutiveNumber;
                model.ClientId = invoice.ClientId;
                model.ClientName = invoice.ClientName;
                model.TaxAmount = note.TaxAmount;
                model.Amount = note.Total;
                model.CodigoMoneda = note.CodigoMoneda;
                model = getTypeFirm(model);
                model.DocumentRef = new Document { Id = note.Id, ConsecutiveNumber = note.ConsecutiveNumber, TypeDocument = "Nota " + EnumHelper.GetDescription(note.NoteType),
                    TypeDocumentCodigo = note.NoteType== NoteType.Credito? (int)TypeDocumentInvoice.NotaCredito: (int)TypeDocumentInvoice.NotaDebito, IsReverseTotal=true };
                model.NoteType = (note.NoteType == NoteType.Credito ? NoteType.Debito : NoteType.Credito);
                model.NoteReasons =  NoteReason.Reversa_documento;
                

            }
            catch (Exception ex)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = ex.GetBaseException().Message;
            }

            ViewBag.Line = list;
            return View("newNote", model);
        }

        private NoteDto getTypeFirm(NoteDto model)
        {
            var tenant = _tenantManager.Get(AbpSession.TenantId.GetValueOrDefault());
            model.ValidateHacienda = tenant.ValidateHacienda;
            if (model.ValidateHacienda)
            {
                if (tenant.TipoFirma == FirmType.Todos)
                    model.FirmTypes = EnumHelper.GetSelectListValues(typeof(FirmType)).Take(2).ToList();
                else
                    model.FirmTypes = EnumHelper.GetSelectList(tenant.TipoFirma);

                model.TipoFirma = model.TipoFirma == null ? tenant.FirmaRecurrente : model.TipoFirma;
            }

            return model;
        }

        private IList<line> resumirLines(IList<line> list)
        {
            
            var listAux =
                        (from c in list
                         group c by new
                         {
                             c.Note, c.TaxId, c.Title, c.DiscountPercentage, c.UnitMeasurement, c.UnitMeasurementOthers
                         } into gcs
                         select new line()
                         {
                             Title = gcs.Key.Title,
                             Note = gcs.Key.Note,
                             DiscountPercentage = gcs.Key.DiscountPercentage,
                             UnitMeasurement = gcs.Key.UnitMeasurement,
                             UnitMeasurementOthers = gcs.Key.UnitMeasurementOthers,
                             PricePerUnit = Invoice.GetValor(Invoice.GetValor(gcs.Sum(y => y.Total)) / gcs.Sum(y => y.Quantity)),
                            Quantity = gcs.Sum(y => y.Quantity),
                            SubTotal = gcs.Sum(y => y.SubTotal),
                            TaxAmount = gcs.Sum(y => y.TaxAmount),
                            TaxId = gcs.Key.TaxId,
                            Total = gcs.Sum(y => y.Total),
                            LineTotal = gcs.Sum(y => y.LineTotal)

                        }).ToList();

            return listAux;
        }

        //[HttpPost]
        //public ActionResult ApplyReverse(NoteDto input)
        //{
        //    SearchInvoicesInput model = new SearchInvoicesInput();
        //    model.ClientId = input.ClientId;
        //    try
        //    {
        //        if (input.Total <= 0)
        //        {
        //            model.ErrorCode = ErrorCodeHelper.Error;
        //            model.ErrorDescription = "El monto de la nota no puede ser menor a cero.";
        //        }

        //        if (input.TipoFirma == FirmType.Llave && _invoiceAppClient.isdigitalPendingNote(AbpSession.TenantId.GetValueOrDefault()))
        //        {
        //            model.ErrorCode = ErrorCodeHelper.Error;
        //            model.ErrorDescription = "Existen Notas pendientes por firma digital.";
        //        }            

        //        _invoiceAppClient.ApplyReverse(input);
        //    }
        //    catch (Exception)
        //    {
        //        return Content("-3");
        //    }
        //    return SearchInvoices((Guid)input.ClientId, input.InvoiceId);
        //}

        //[HttpPost]
        //public ActionResult ReverseNote(NoteDto model)
        //{
        //    try
        //    {
        //        var invoice = _invoiceAppClient.Get((Guid)model.InvoiceId);
        //        var originNote = invoice.Notes.Where(n => n.Id == model.DocumentRef.Id).FirstOrDefault();

        //        // model.ClientId = invoice.ClientId.Value;
        //        //model.CanBeReversed = true;
        //        //model.OrigenInvoiceNumber = invoice.Number;
        //        //model.OrigenNoteNumber = originNote.ConsecutiveNumber;
        //        //model.OrigenNoteAmount = originNote.Amount;
        //        //model.OrigenNoteTax = originNote.TaxAmount;
        //        //model.OrigenNoteTotal = originNote.Total;
        //        //model.OrigenNoteType = originNote.NoteType;
        //        //model.CodigoMoneda = (NoteCodigoMoneda)invoice.CodigoMoneda;

        //        if (ModelState.IsValid)
        //        {
        //            ModelState.Clear();

        //            decimal amountAlreadyReversed = _invoiceAppClient.GetNoteAmountAlreadyReversed(model.DocumentRef.ConsecutiveNumber);
        //            if (amountAlreadyReversed >= originNote.Total)
        //            {
        //                /// model.CanBeReversed = false;
        //                model.ErrorCode = ErrorCodeHelper.Error;
        //                model.ErrorDescription = $"No se puede reversar la nota {model.DocumentRef.ConsecutiveNumber}, la misma ya fue totalmente reversada.";
        //                return View("newNote", model);
        //            }
        //            if (model.Total > originNote.Total)
        //            {
        //                model.ErrorCode = ErrorCodeHelper.Error;
        //                model.ErrorDescription = $"El monto máximo permitido es {originNote.Amount - amountAlreadyReversed} ya que la nota {model.DocumentRef.ConsecutiveNumber} tiene uno o mas reversos parciales.";
        //                return View("newNote", model);
        //            }
        //            if (model.TipoFirma == FirmType.Llave && _invoiceAppClient.isdigitalPendingNote(invoice.TenantId))
        //            {
        //                model.ErrorCode = ErrorCodeHelper.Error;
        //                model.ErrorDescription = $"Existen notas pendientes por firma digital.";
        //                return View("newNote", model);
        //            }
        //            var note = _invoiceAppClient.ReverseNote(model);

        //            //model.Amount = note.Amount;
        //            //model.Tax = note.TaxAmount;
        //            //model.Total = note.Total;
        //            //model.Type = note.NoteType;

        //            model.ErrorCode = ErrorCodeHelper.Ok;
        //            model.ErrorDescription = "Nota reversada con éxito!";
        //            return SearchInvoices((Guid)invoice.ClientId, model.InvoiceId);
        //        }
        //        model.ErrorCode = ErrorCodeHelper.Error;
        //        model.ErrorDescription = ModelState.ToErrorMessage();
        //        return View("newNote", model);
        //    }
        //    catch (DbEntityValidationException ex)
        //    {
        //        ModelState.AddRange(ex.GetModelErrors());
        //        model.ErrorDescription = ex.GetModelErrorMessage();
        //        model.ErrorCode = ErrorCodeHelper.Error;
        //        return View("newNote", model);
        //    }
        //    catch (Exception e)
        //    {
        //        model.ErrorDescription = e.Message;
        //        model.ErrorCode = ErrorCodeHelper.Error;
        //        return View("newNote", model);
        //    }
        //}


    }
}