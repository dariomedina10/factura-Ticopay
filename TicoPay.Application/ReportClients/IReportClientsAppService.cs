using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using TicoPay.Clients;
using TicoPay.ReportClients.Dto;

namespace TicoPay.ReportClients
{
    public interface IReportClientsAppService : IApplicationService
    {
        IList<Client> SearchReportClients(ReportClientsInputDto searchInput);
    }
}
