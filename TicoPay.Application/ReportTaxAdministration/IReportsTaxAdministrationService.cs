using Abp.Application.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Clients;
using TicoPay.MultiTenancy;
using TicoPay.ReportTaxAdministration.Dto;

namespace TicoPay.ReportTaxAdministration
{
    public interface IReportsTaxAdministrationService : IApplicationService
    {
        ReportTaxAdministrationSearchInvoicesOutput Search(ReportTaxAdministrationSearchInvoicesInput inputDto);
        IPagedList<EmisorDto> GetAllInvoicesSenders(int? pageIndex, int? pageSize, string q);
        IPagedList<ReceptorDto> GetAllInvoiceReceivers(int? pageIndex, int? pageSize, string q);
    }
}
