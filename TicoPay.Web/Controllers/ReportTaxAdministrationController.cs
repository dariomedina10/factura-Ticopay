using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Authorization.Roles;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.Invoices.Dto;
using TicoPay.MultiTenancy;
using TicoPay.ReportTaxAdministration;
using TicoPay.ReportTaxAdministration.Dto;

namespace TicoPay.Web.Controllers
{
    [Authorize(Roles = StaticRoleNames.Tenants.TaxAdministration)]
    public class ReportTaxAdministrationController : TicoPayControllerBase
    {
        private readonly IReportsTaxAdministrationService _reportsTaxAdministrationService;
        private readonly IInvoiceAppService _invoiceAppClient;
        private readonly IClientAppService _clientAppClient;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ITenantAppService _tenantAppService;

        public ReportTaxAdministrationController(IReportsTaxAdministrationService reportsTaxAdministrationService, IInvoiceAppService invoiceAppClient, IClientAppService clientAppClient, IUnitOfWorkManager unitOfWorkManager, ITenantAppService tenantAppService)
        {
            _reportsTaxAdministrationService = reportsTaxAdministrationService;
            _invoiceAppClient = invoiceAppClient;
            _clientAppClient = clientAppClient;
            _unitOfWorkManager = unitOfWorkManager;
            _tenantAppService = tenantAppService;
        }

        public ActionResult Index()
        {
            var inputDto = new ReportTaxAdministrationSearchInvoicesInput();
            ReportTaxAdministrationSearchInvoicesOutput result = _reportsTaxAdministrationService.Search(inputDto);
            inputDto.Invoices = result.Invoices;
            return View(inputDto);
        }

        public ActionResult Search(ReportTaxAdministrationSearchInvoicesInput inputDto)
        {
            ReportTaxAdministrationSearchInvoicesOutput result = _reportsTaxAdministrationService.Search(inputDto);
            inputDto.Invoices = result.Invoices;
            return PartialView("_listPartial", inputDto);
        }

        public ActionResult ShowEmisorList(int? page, int? pageSize, string q)
        {
            SearchEmisorInput input = new SearchEmisorInput();
            input.Entities = _reportsTaxAdministrationService.GetAllInvoicesSenders(page, pageSize, q);
            return PartialView("_showEmisorPartial", input);
        }

        public ActionResult ShowReceptorList(int? page, int? pageSize, string q)
        {
            SearchReceptorInput input = new SearchReceptorInput();
            input.Entities = _reportsTaxAdministrationService.GetAllInvoiceReceivers(page, pageSize, q);
            return PartialView("_showReceptorPartial", input);
        }
    }
}