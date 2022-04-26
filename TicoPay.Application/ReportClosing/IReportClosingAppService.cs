using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using TicoPay.Clients;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;
using TicoPay.ReportClosing.Dto;
using TicoPay.Users;

namespace TicoPay.ReportClosing
{
    public interface IReportClosingAppService : IApplicationService
    {
        IList<ReportClosingDto> SearchReportClosing(ReportClosingInputDto<ReportClosingDto> searchInput);
        IList<Client> GetAllClientsList();
        int GetCountClientsActive();
        int GetCountUsersActive();
        IList<User> GetUserByRole();

        bool GetUSDOfCRC(FacturaElectronicaResumenFacturaCodigoMoneda moneda);

        IList<ResultTotalDto> GetInvoices(DateTime fechai, DateTime fechaf, FacturaElectronicaResumenFacturaCodigoMoneda moneda, Guid? BranchOfficeId, Guid? DrawerId);
        IList<ResultTotalDto> GetNoteCredit(DateTime fechai, DateTime fechaf, FacturaElectronicaResumenFacturaCodigoMoneda moneda, Guid? BranchOfficeId, Guid? DrawerId);
        IList<ResultTotalDto> GetNoteDebit(DateTime fechai, DateTime fechaf, FacturaElectronicaResumenFacturaCodigoMoneda moneda, Guid? BranchOfficeId, Guid? DrawerId);
        IList<ResultTotalDto> GetPayments(DateTime fechai, DateTime fechaf, FacturaElectronicaResumenFacturaCodigoMoneda moneda, Guid? BranchOfficeId, Guid? DrawerId);
    }
}
