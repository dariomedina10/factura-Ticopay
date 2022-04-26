using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TicoPay.Common;
using TicoPay.Drawers;
using TicoPay.Invoices;
using TicoPay.ReportStatusInvoices;
using TicoPay.ReportStatusInvoices.Dto;

namespace TicoPay.Web.Controllers
{
    public class IntegrationZohoController : Controller
    {

        private readonly IReportStatusInvoicesAppService _reportStatusInvoicesAppService;
        private readonly IDrawersAppService _drawersAppService;

        public IntegrationZohoController(IReportStatusInvoicesAppService reportStatusInvoicesAppService, IDrawersAppService drawersAppService)
        {
            _reportStatusInvoicesAppService = reportStatusInvoicesAppService;
            _drawersAppService = drawersAppService;

        }

        // GET: IntegrationZoho
        public ActionResult Index()
        {
            ReportStatusInvoicesInputDto<IntegracionZohoSVConta> model = new ReportStatusInvoicesInputDto<IntegracionZohoSVConta>();
            try
            {
                model.Query = "";
                DateTime tempDate = DateTime.Now;
                model.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(null);
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                model.Control = "IntegrationZoho";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar los documentos";
            }
            return View(model);
        }

        [HttpPost]
        public ViewResultBase Search(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> model)
        {
            try
            {
                    DateTime tempInitial;
                    DateTime tempFinal;
                    if (model.InitialDate != null)
                    {
                        tempInitial = model.InitialDate.Value;
                        model.InitialDate = new DateTime(tempInitial.Year, tempInitial.Month, tempInitial.Day, 00, 00, 00);
                    }

                    if (model.FinalDate != null)
                    {
                        tempFinal = model.FinalDate.Value;
                        model.FinalDate = new DateTime(tempFinal.Year, tempFinal.Month, tempFinal.Day, 23, 59, 59);
                    }
                model.InvoicesList = _reportStatusInvoicesAppService.SearchIntegrationZoho(model);
                model.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(model.BranchOfficeId);
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                    model.ErrorCode = ErrorCodeHelper.Ok;
                    model.ErrorDescription = "";
                }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Facturas";
            }
            return View("Index", model);
        }

       
    }
}