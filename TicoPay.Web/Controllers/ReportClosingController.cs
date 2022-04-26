using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Authorization.Roles;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Drawers;
using TicoPay.Invoices;
using TicoPay.ReportClosing;
using TicoPay.ReportClosing.Dto;
using TicoPay.Web.Infrastructure;

namespace TicoPay.Web.Controllers
{
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    [Authorize]
    public class ReportClosingController : TicoPayControllerBase
    {
        private readonly IReportClosingAppService _reportClosingAppService;
        private readonly IClientManager _clientManager;
        private readonly IDrawersAppService _drawersAppService;
        private readonly RoleManager _roleManager;

        public ReportClosingController(IReportClosingAppService reportClosingAppService, IClientManager clientManager, IDrawersAppService drawersAppService,
            RoleManager roleManager)
        {
            _reportClosingAppService = reportClosingAppService;            
            _clientManager = clientManager;
            _drawersAppService = drawersAppService;
            _roleManager = roleManager;
        }

        public ActionResult Index()
        {
            ReportClosingInputDto<ReportClosingDto> model = new ReportClosingInputDto<ReportClosingDto>();
            try
            {
                model.Query = "";
                DateTime tempDate = DateTime.Now;
                model.InitialDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 00, 00, 00);
                model.FinalDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 23, 59, 00);
                var entities = _reportClosingAppService.SearchReportClosing(model);
                model.InvoicesList = entities;
                model.ClientList = _reportClosingAppService.GetAllClientsList();
                model.Groups = _clientManager.GetAllListGroups();
                model.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var rol = _roleManager.GetRoleByUser(AbpSession.UserId.Value);
                var role = _roleManager.FindNameRol(rol);
                model.UserRol = role.Name;
                model.Usuarios = _reportClosingAppService.GetUserByRole().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(null);
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                model.Control = "ReportClosing";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las facturas";
            }
            return View(model);

        }

      

        [HttpPost]
        public ViewResultBase Search(ReportClosingInputDto<ReportClosingDto> model)
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
                var entities = _reportClosingAppService.SearchReportClosing(model);
                model.InvoicesList = entities;
                model.BranchOffice= _drawersAppService.getUserbranch().ToList();
                model.Usuarios = _reportClosingAppService.GetUserByRole().ToList();
                var rol = _roleManager.GetRoleByUser(AbpSession.UserId.Value);
                var role = _roleManager.FindNameRol(rol);
                model.UserRol = role.Name;
                var drawerUser = _drawersAppService.getUserDrawers(model.BranchOfficeId);
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                model.ClientList = _reportClosingAppService.GetAllClientsList();
                model.Groups = _clientManager.GetAllListGroups();
                model.Control = "ReportClosing";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las facturas";
            }
            return View("Index", model);
        }

        [ActionName("GetDrawer")]
        public ActionResult GetDrawer(Guid? id)
        {
            var drawer = _drawersAppService.getUserDrawers(id);

            var resp = drawer.Select(x => new SelectListItem()
            {
                Value = x.Drawer.Id.ToString(),
                Text = x.Drawer.Code
            }).ToList();
           // resp.Insert(0, new SelectListItem() { Value = "", Text = "Todos" });

            return Json(resp, JsonRequestBehavior.AllowGet);

        }
    }
}