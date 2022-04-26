using Abp.Application.Services;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.Authorization;
using TicoPay.Authorization.Roles;
using TicoPay.Common;
using TicoPay.Core;
using TicoPay.ReportsSettings.Dto;
using TicoPay.Roles;

namespace TicoPay.ReportsSettings
{
    public class ReportSettingsAppService : ApplicationService, IReportSettingsAppService
    {
        private readonly IRepository<ReportSettings> _repository;

        public ReportSettingsAppService(IRepository<ReportSettings> repository)
        {
            _repository = repository;
        }

        public ReportSettingsDto GetReportSettings(int id)
        {
            var model = _repository.Get(id);
            if (model != null)
            {
                return model.MapTo<ReportSettingsDto>();
            }
            return null;
        }

        public ReportSettings GetReportSettingsByReportName(int tenantId, TicoPayReports ticoPayReport,string remitente)
        {
            var report = _repository.GetAll().Where(r => r.TenantId == tenantId && r.ReportName == ticoPayReport.ToString()).FirstOrDefault();
            return report;
        }

        public List<ReportSettingsDto> GetReportSettingsList()
        {
            return _repository.GetAll()
                .Where(i => i.TenantId == AbpSession.TenantId)
                .ToList()
                .MapTo<List<ReportSettingsDto>>();
        }

        public void Update(ReportSettingsDto dto)
        {
            ReportSettings model = dto.MapTo<ReportSettings>();
            if (dto.WatermarkFile != null && dto.WatermarkFile.InputStream != null)
            {
                model.WatermarkImage = dto.WatermarkFile.InputStream.ToByteArray();
            }
            else
            {
                model.WatermarkImage = null;
            }
            _repository.Update(model);
        }

        /// <summary>
        /// Retorna la lista de fonts soportadas por iTextSharp
        /// </summary>
        /// <returns></returns>
        public List<FontListSelectItem> GetSupportedFonts()
        {
            List<FontListSelectItem> fonts = new List<FontListSelectItem>();
            fonts.Add(new FontListSelectItem("Arial"));
            fonts.Add(new FontListSelectItem("Courier"));
            fonts.Add(new FontListSelectItem("ZapfDingbats"));
            fonts.Add(new FontListSelectItem("Times-BoldItalic"));
            fonts.Add(new FontListSelectItem("Times-Italic"));
            fonts.Add(new FontListSelectItem("Times-Bold"));
            fonts.Add(new FontListSelectItem("Times-Roman"));
            fonts.Add(new FontListSelectItem("Symbol"));
            fonts.Add(new FontListSelectItem("Times"));
            fonts.Add(new FontListSelectItem("Helvetica-Oblique"));
            fonts.Add(new FontListSelectItem("Helvetica-Bold"));
            fonts.Add(new FontListSelectItem("Helvetica"));
            fonts.Add(new FontListSelectItem("Courier-BoldOblique"));
            fonts.Add(new FontListSelectItem("Courier-Oblique"));
            fonts.Add(new FontListSelectItem("Courier-Bold"));
            fonts.Add(new FontListSelectItem("Helvetica-BoldOblique"));
            return fonts;
        }

        public void CreateDefaultConfigurationIfNone(int tenantId)
        {
            var reports = Enum.GetValues(typeof(TicoPayReports));
            foreach (var report in reports)
            {
                //TODO: Se debe quitar esta condicion cuando se agrege las notas a la lista
                if (report.ToString() == "Nota")
                {
                    continue;
                }

                var exist = _repository.GetAll().Any(r => r.TenantId == tenantId && r.ReportName == report.ToString());
                if (!exist && tenantId > 0)
                {
                    var reportSettings = new ReportSettings
                    {
                        TenantId = tenantId,
                        ReportName = report.ToString(),
                        ReportTitle = ((TicoPayReports)report).GetDescription()
                    };
                    _repository.Insert(reportSettings);
                }
            }

            CurrentUnitOfWork.SaveChanges();
        }
    }
}
