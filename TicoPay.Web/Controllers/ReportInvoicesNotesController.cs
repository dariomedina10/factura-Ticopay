using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.ReportStatusInvoices;
using TicoPay.ReportStatusInvoices.Dto;
using TicoPay.Web.Infrastructure;
using TicoPay.Clients;
using System.Text.RegularExpressions;
using TicoPay.Application.Helpers;
using TicoPay.Invoices;
using TicoPay.Common;
using TicoPay.Drawers;

namespace TicoPay.Web.Controllers
{
    public class ReportInvoicesNotesController : Controller
    {
        private readonly IReportStatusInvoicesAppService _reportStatusInvoicesAppService;
        private readonly IClientManager _clientManager;
        private readonly IDrawersAppService _drawersAppService;

        public ReportInvoicesNotesController(IReportStatusInvoicesAppService reportStatusInvoicesAppService, IClientManager clientManager, IDrawersAppService drawersAppService)
        {
            _reportStatusInvoicesAppService = reportStatusInvoicesAppService;
            _clientManager = clientManager;
            _drawersAppService = drawersAppService;

        }
        // GET: ReportInvoicesNotes
        public ActionResult Index()
        {
            ReportStatusInvoicesInputDto<ReportInvoicesNotes> model = new ReportStatusInvoicesInputDto<ReportInvoicesNotes>();
            try
            {
                model.Query = "";
                DateTime tempDate = DateTime.Now;

                model.InitialDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 00, 00, 00);
                model.FinalDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 23, 59, 00);
                //model.listStatus = EnumHelper.GetSelectList(typeof(Status));              
                model.InvoicesList = _reportStatusInvoicesAppService.SearchReportInvoicesNotes(model);
                model.Groups = _clientManager.GetAllListGroups();
                model.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(null);
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                model.Control = "ReportInvoicesnotes";
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
        public ViewResultBase Search(ReportStatusInvoicesInputDto<ReportInvoicesNotes> model)
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
               
                //model.listStatus = EnumHelper.GetSelectList(typeof(Status));
                model.InvoicesList = _reportStatusInvoicesAppService.SearchReportInvoicesNotes(model);
                model.Groups = _clientManager.GetAllListGroups();
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