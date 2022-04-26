using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.ReportsSettings.Dto;

namespace TicoPay.ReportsSettings
{
    public interface IReportSettingsAppService : IApplicationService
    {
        List<ReportSettingsDto> GetReportSettingsList();
        ReportSettingsDto GetReportSettings(int id);
        void Update(ReportSettingsDto dto);
        ReportSettings GetReportSettingsByReportName(int tenantId, TicoPayReports factura,string remitente);
        List<FontListSelectItem> GetSupportedFonts();
        void CreateDefaultConfigurationIfNone(int tenantId);
    }
}
