using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.ReportAccountsReceivable.Dto;

namespace TicoPay.ReportAccountsReceivable
{
    public interface IReportCxCAppService : IApplicationService
    {
        IList<Invoice> SearchReportAccountsReceivable(ReportAccountsReceivableInputDto searchInput);
        IList<Client> GetAllClientsList();
    }
}
