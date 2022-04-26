using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Drawers;
using TicoPay.MultiTenancy;
using TicoPay.ReportAccountsReceivable;
using TicoPay.ReportAccountsReceivable.Dto;
using TicoPay.Web.Infrastructure;

namespace TicoPay.Web.Controllers
{
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    [Authorize]
    public class ReportCxCController : TicoPayControllerBase
    {
        private readonly IReportCxCAppService _reportAccountsReceivableAppService;
        private readonly IClientManager _clientManager;
        private readonly IDrawersAppService _drawersAppService;
        private readonly TenantManager _tenantManager;

        public ReportCxCController(IReportCxCAppService reportAccountsReceivableAppService, IClientManager clientManager, IDrawersAppService drawersAppService, TenantManager tenantManager)
        {
            _reportAccountsReceivableAppService = reportAccountsReceivableAppService;
            _clientManager = clientManager;
            _drawersAppService = drawersAppService;
            _tenantManager = tenantManager;
        }

        public ActionResult Index()
        {
            ReportAccountsReceivableInputDto model = new ReportAccountsReceivableInputDto();
            try
            {
                model.Query = "";
                DateTime tempDate = DateTime.Now;
                model.InitialDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 00, 00, 00);
                model.FinalDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 23, 59, 00);
                model.InvoicesList = _reportAccountsReceivableAppService.SearchReportAccountsReceivable(model);
                model.ClientList = _reportAccountsReceivableAppService.GetAllClientsList();
                model.MonedaTenant = _tenantManager.Get(AbpSession.TenantId.Value).CodigoMoneda;
                model.Groups = _clientManager.GetAllListGroups();
                model.BranchOffice = _drawersAppService.getUserbranch().ToList();
                var drawerUser = _drawersAppService.getUserDrawers(null);
                model.DrawerUser = (from d in drawerUser select new SelectListItem { Value = d.Drawer.Id.ToString(), Text = d.Drawer.Code }).ToList();
                model.Control = "ReportCxC";
                model.Action = "Search";
                model.ErrorCode = ErrorCodeHelper.Ok;
                model.ErrorDescription = "";
            }
            catch (Exception)
            {
                model.ErrorCode = ErrorCodeHelper.Error;
                model.ErrorDescription = "Error al buscar las Existencias";
            }
            return View(model);

        }

        [HttpPost]
        public ViewResultBase Search(ReportAccountsReceivableInputDto model)
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
                var entities = _reportAccountsReceivableAppService.SearchReportAccountsReceivable(model);
                model.MonedaTenant = _tenantManager.Get(AbpSession.TenantId.Value).CodigoMoneda;
                model.ClientList = _reportAccountsReceivableAppService.GetAllClientsList();
                model.InvoicesList = entities;
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
                model.ErrorDescription = "Error al buscar las Existencias";
            }
            return View("Index", model);
        }


    }
}