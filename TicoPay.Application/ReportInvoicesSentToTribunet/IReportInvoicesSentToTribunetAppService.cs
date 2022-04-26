using Abp.Application.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.ReportInvoicesSentToTribunet.Dto;

namespace TicoPay.ReportInvoicesSentToTribunet
{
    public interface IReportInvoicesSentToTribunetAppService: IApplicationService
    {
        ReportInvoicesSentToTribunetOutput Search(ReportInvoicesSentToTribunetSearchInput inputDto);
        IPagedList<ReportInvoicesSentToTribunetClienteDto> GetAllInvoiceClients(int? page, int? pageSize, string q);
    }
}
