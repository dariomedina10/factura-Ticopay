using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.ReportStatusInvoices.Dto;

namespace TicoPay.ReportStatusInvoices
{
    public interface IReportStatusInvoicesAppService: IApplicationService
    {
        IList<ReportStatusInvoicesDto> SearchReportStatusInvoices(ReportStatusInvoicesInputDto<ReportStatusInvoicesDto> searchInput);
        IList<ReportInvoicesNotes> SearchReportInvoicesNotes(ReportStatusInvoicesInputDto<ReportInvoicesNotes> searchInput);

        IList<IntegracionZohoSVConta> SearchIntegrationZoho(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput);

        IList<IntegracionZohoSVConta> SearchIntegrationSVConta(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput);

        DataTable DataTableZohoCVConta(ReportStatusInvoicesInputDto<IntegracionZohoSVConta> searchInput,IntegrationZohoSVConta integrationZohoSVConta);
    }
}
