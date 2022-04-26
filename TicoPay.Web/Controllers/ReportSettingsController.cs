using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Invoices;
using TicoPay.Invoices.XSD;
using TicoPay.MultiTenancy;
using TicoPay.ReportsSettings;
using TicoPay.ReportsSettings.Dto;
using TicoPay.Web.Infrastructure;
using ZXing;

namespace TicoPay.Web.Controllers
{
    public class ReportSettingsController : TicoPayControllerBase
    {
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IReportSettingsAppService _reportSettingsAppService;

        public ReportSettingsController(IRepository<Tenant, int> tenantRepository, IReportSettingsAppService reportSettingsAppService)
        {
            _tenantRepository = tenantRepository;
            _reportSettingsAppService = reportSettingsAppService;
        }

        public ActionResult Index()
        {
            ReportSettingsListDto dto = new ReportSettingsListDto();
            dto.ReportsSettings = _reportSettingsAppService.GetReportSettingsList();
            return View(dto);
        }

        public ActionResult Edit(int id)
        {
            ReportSettingsDto dto = _reportSettingsAppService.GetReportSettings(id);
            dto.Fonts = _reportSettingsAppService.GetSupportedFonts();
            dto.FontSizes = new List<int> { 8, 10, 11, 12, 14, 16, 18, 24 };
            dto.WatermarkBase64String = dto.WatermarkImage == null ? string.Empty : "data:image/png;base64," + Convert.ToBase64String(dto.WatermarkImage, 0, dto.WatermarkImage.Length);

            return PartialView("_editPartial", dto);
        }

        [HttpPost]
        public ActionResult Edit(ReportSettingsDto dto)
        {
            try
            {
                dto.Fonts = _reportSettingsAppService.GetSupportedFonts();
                dto.FontSizes = new List<int> { 8, 10, 11, 12, 14, 16, 18, 24 };
                dto.WatermarkBase64String = dto.WatermarkImage == null ? string.Empty : "data:image/png;base64," + Convert.ToBase64String(dto.WatermarkImage, 0, dto.WatermarkImage.Length);

                if (ModelState.IsValid)
                {
                    _reportSettingsAppService.Update(dto);
                    dto.ErrorCode = ErrorCodeHelper.Ok;
                    dto.ErrorDescription = "¡Configuración guardada exitosamente!";
                    return PartialView("_editPartial", dto);
                }
                dto.ErrorCode = ErrorCodeHelper.Error;
                dto.ErrorDescription = ModelState.ToErrorMessage();
                return PartialView("_editPartial", dto);
            }
            catch (DbEntityValidationException ex)
            {
                ModelState.AddRange(ex.GetModelErrors());
                dto.ErrorDescription = ex.GetModelErrorMessage();
                dto.ErrorCode = ErrorCodeHelper.Error;
                return PartialView("_createPartial", dto);
            }
            catch (Exception e)
            {
                dto.ErrorDescription = e.Message;
                dto.ErrorCode = ErrorCodeHelper.Error;
                return PartialView("_editPartial", dto);
            }
        }

        public ActionResult GetWatermark(int id)
        {
            var current = _reportSettingsAppService.GetReportSettings(id);
            if (current != null && current.WatermarkImage != null && current.WatermarkImage.Length > 0)
            {
                string logoBase64String = "data:image/png;base64," + Convert.ToBase64String(current.WatermarkImage, 0, current.WatermarkImage.Length);
                return Json(logoBase64String, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Genera un pdf con datos falsos para preview en reportsettings
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPdfPreview(ReportSettingsDto dto)
        {
            var tenant = _tenantRepository.Get(AbpSession.TenantId.Value);
            var client = Client.Create(AbpSession.TenantId.Value, "Nombre Cliente", "Apillido Cliente", Gender.Male, new DateTime(1980, 1, 1), "111222333", IdentificacionTypeTipo.Cedula_Fisica, "123-00000000", "123-00000000", "",
                "cliente@servidor.com", "", 1, "", "CR", "A 300 metros de la estación de servicio.", null, null, "", "", "", "", "", "Nombre Cliente", "", 0,1);
            var invoice = Invoice.Create(AbpSession.TenantId.Value, "", DateTime.Now, null, tenant, FacturaElectronicaCondicionVenta.Contado, FacturaElectronicaResumenFacturaCodigoMoneda.CRC);
            invoice.ConsecutiveNumber = "00100001010000000001";
            invoice.AssignInvoiceLine(AbpSession.TenantId.Value, 1500, 15, 0, "", "", "Servicio", 1, LineType.Service, null, null, invoice, 1, null, null,UnidadMedidaType.Servicios_Profesionales, "");

            invoice.ClientName = client.Name + " " + (client.IdentificationType != IdentificacionTypeTipo.Cedula_Juridica ? client.LastName : string.Empty);
            invoice.ClientAddress = (client.Barrio != null ? client.Barrio.NombreBarrio + ", " + client.Barrio.Distrito.NombreDistrito + ", " + client.Address : string.Empty);
            invoice.ClientEmail = client.Email;
            invoice.ClientMobilNumber = client.MobilNumber;
            invoice.ClientIdentificationType = client.IdentificationType;
            invoice.ClientIdentification = client.IdentificationType == IdentificacionTypeTipo.NoAsiganda ? client.IdentificacionExtranjero : client.Identification;
            invoice.ClientPhoneNumber = client.PhoneNumber;


            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                barcodeWriter.Write(invoice.ConsecutiveNumber).Save(memoryStream, ImageFormat.Png);
                invoice.QRCode = memoryStream.ToArray();
            }

            ReportSettings reportSettings = dto.MapTo<ReportSettings>();
            if (dto.WatermarkFile != null && dto.WatermarkFile.InputStream != null)
            {
                reportSettings.WatermarkImage = dto.WatermarkFile.InputStream.ToByteArray();
            }
            else
            {
                reportSettings.WatermarkImage = null;
            }
            GeneratePDF generatePDF = new GeneratePDF(reportSettings);
            Stream pdfStream = generatePDF.CreatePDFAsStream(invoice, client, tenant, null, null);
            var data = pdfStream.ToByteArray();
            var pdfBase64 = Convert.ToBase64String(data, 0, data.Length);
            return Json(new { PdfPreview = pdfBase64 }, JsonRequestBehavior.AllowGet);
        }
    }
}