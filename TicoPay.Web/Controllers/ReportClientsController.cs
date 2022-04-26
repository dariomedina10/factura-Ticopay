using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Common;
using TicoPay.ReportAccountsReceivable.Dto;
using TicoPay.ReportClients;
using TicoPay.ReportClients.Dto;
using TicoPay.Web.Infrastructure;

namespace TicoPay.Web.Controllers
{
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    [Authorize]
    public class ReportClientsController : TicoPayControllerBase
    {
        private readonly IReportClientsAppService _reportClientsAppService;
        // GET: ReportAccountsReceivable

        public ReportClientsController(IReportClientsAppService reportClientsAppService)
        {
            _reportClientsAppService = reportClientsAppService;
        }

        public ActionResult Index()
        {
            ReportClientsInputDto model = new ReportClientsInputDto();
            try
            {
                model.Query = "";
                model.ClientsList = _reportClientsAppService.SearchReportClients(model);
                //model.CurrentUserName = _currentUser.UserName;
                //model.CurrentDateTime = _date.Now;
                model.Control = "ReportClients";
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
        public ViewResultBase Search(ReportClientsInputDto model)
        {
            try
            {
                var entities = _reportClientsAppService.SearchReportClients(model);
                model.ClientsList = entities;
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