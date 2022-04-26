using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Clients;
using TicoPay.ReportInvoicesSentToTribunet;
using TicoPay.ReportInvoicesSentToTribunet.Dto;

namespace TicoPay.Web.Controllers
{
    [Authorize]
    public class ReportInvoicesSentToTribunetController : TicoPayControllerBase
    {
        private readonly IReportInvoicesSentToTribunetAppService _reportInvoicesSentToTribunetAppService;
        private readonly IClientAppService _clientAppClient;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ReportInvoicesSentToTribunetController(IReportInvoicesSentToTribunetAppService reportInvoicesSentToTribunetAppService, IClientAppService clientAppClient, IUnitOfWorkManager unitOfWorkManager)
        {
            _reportInvoicesSentToTribunetAppService = reportInvoicesSentToTribunetAppService;
            _clientAppClient = clientAppClient;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public ActionResult Index()
        {
            var inputDto = new ReportInvoicesSentToTribunetSearchInput();
            ReportInvoicesSentToTribunetOutput result = _reportInvoicesSentToTribunetAppService.Search(inputDto);
            inputDto.Invoices = result.Invoices;
            return View(inputDto);
        }

        public ActionResult Search(ReportInvoicesSentToTribunetSearchInput inputDto)
        {
            ReportInvoicesSentToTribunetOutput result = _reportInvoicesSentToTribunetAppService.Search(inputDto);
            inputDto.Invoices = result.Invoices;
            return PartialView("_listPartial", inputDto);
        }

        public ActionResult ShowClienteList(int? page, int? pageSize, string q)
        {
            SearchClienteInput input = new SearchClienteInput();
            input.Entities = _reportInvoicesSentToTribunetAppService.GetAllInvoiceClients(page, pageSize, q);
            return PartialView("_showClientePartial", input);
        }

    }
}