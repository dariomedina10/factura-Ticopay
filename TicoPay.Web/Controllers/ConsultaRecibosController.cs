using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Common;
using TicoPay.ConsultaRecibos;
using TicoPay.ConsultaRecibos.Dto;
using TicoPay.Invoices;
using TicoPay.Invoices.Dto;
using TicoPay.Web.Infrastructure;

namespace TicoPay.Web.Controllers
{
    [Deny(Roles = Authorization.Roles.StaticRoleNames.Tenants.TaxAdministration)]
    public class ConsultaRecibosController : TicoPayControllerBase
    {
        private readonly IConsultaReciboAppService _consultaRecibosAppService;
        private readonly IInvoiceAppService _invoiceAppService;

        public ConsultaRecibosController(IConsultaReciboAppService consultaRecibosAppService, IInvoiceAppService invoiceAppService)
        {
            _consultaRecibosAppService = consultaRecibosAppService;
            _invoiceAppService = invoiceAppService;
        }
        // GET: ConsultaRecibos
        public ActionResult Index()
        {
            SearchConsultaRecibosInput viewModel = new SearchConsultaRecibosInput();
            viewModel.Action = "Search";
            viewModel.Control = "ConsultaRecibos";
            viewModel.Tenants = _consultaRecibosAppService.GetAllTenants();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Search(string identification, long tenantId)
        {
            SearchConsultaRecibosInput tempView = new SearchConsultaRecibosInput();
            tempView.Identification = identification;
            tempView.TenantId = tenantId;
            try
            {
                var entities = _invoiceAppService.SearchInvoicesPending(tempView.Identification, tempView.TenantId);
                tempView.Entities = entities;
                if (entities != null && entities.Count > 0)
                    tempView.ClientInfo = _invoiceAppService.GetClient(entities.FirstOrDefault().ClientId.Value);
                else
                    tempView.ClientInfo = _invoiceAppService.GetClient(tempView.Identification, tempView.TenantId);
                
                foreach (var item in tempView.Entities)
                {
                    IList<ListItems> newList = new List<ListItems>();
                    var list = new ListItems();
                    list.InvoiceLines = _invoiceAppService.GetAllListInvoiceLines(item.Id);
                    list.Notes = item.Notes;
                    list.Status = item.Status;
                    newList.Add(list);
                    item.ListItems = newList;
                }
            }
            catch (Exception e)
            {
                tempView.ErrorCode = ErrorCodeHelper.Error;
                tempView.ErrorDescription = e.Message;
                
            }

           return PartialView("_listPartial", tempView);
        }
    }
}