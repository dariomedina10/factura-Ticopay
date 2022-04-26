using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using TicoPay.Common;
using TicoPay.Invoices;
using TicoPay.ReportStatusInvoices;
using TicoPay.ReportStatusInvoices.Dto;

namespace TicoPay.Web.Controllers
{
    public class ExportCSVExcelController : Controller
    {
        private readonly IReportStatusInvoicesAppService _reportStatusInvoicesAppService;

        public ExportCSVExcelController(IReportStatusInvoicesAppService reportStatusInvoicesAppService)
        {
            _reportStatusInvoicesAppService = reportStatusInvoicesAppService;

        }

        // GET: ImportsCSVExcel
        [Authorize]
        public FileResult DownloadExcel(DateTime? initialDate, DateTime? finalDate, Guid? clientId, string consecutive, Status? statusPay,
                                        NoteCodigoMoneda? moneda, TypeDocument? type, IntegrationZohoSVConta fileName)
        {
            var nameDocument = "";
            ReportStatusInvoicesInputDto<IntegracionZohoSVConta> model = new ReportStatusInvoicesInputDto<IntegracionZohoSVConta>()
            {
                InitialDate = initialDate,
                FinalDate = finalDate,
                ClientId = clientId,
                NumberInvoice = consecutive,
                StatusPay = statusPay,
                CodigoMoneda = moneda,
                Type = type
            };

            if (type == null)
            {
                if (fileName.Equals(IntegrationZohoSVConta.SVConta))
                {
                    nameDocument = "SVContaExcel";
                }
            }
            else
                nameDocument = Application.Helpers.EnumHelper.GetDescription(type) + Application.Helpers.EnumHelper.GetDescription(fileName) + "Excel";

            IWorkbook workbook = new HSSFWorkbook();

            var document="";
            if (fileName.Equals(IntegrationZohoSVConta.SVConta))
            {
                document = nameDocument;
                    }
            else
            {
                document = Application.Helpers.EnumHelper.GetDescription(type);
                }
                var dt = _reportStatusInvoicesAppService.DataTableZohoCVConta(model, fileName);
                LibroExcel(workbook, document, dt);

            MemoryStream m = new MemoryStream();
            workbook.Write(m);

            return File(m.GetBuffer(), "application/vnd.ms-excel", nameDocument + ".xls");
        }


        [Authorize]
        public FileResult DownloadCSV(DateTime? initialDate, DateTime? finalDate, Guid? clientId, string consecutive, Status? statusPay,
                                        NoteCodigoMoneda? moneda, TypeDocument type, IntegrationZohoSVConta fileName)
        {
            var nameDocument = "";

            ReportStatusInvoicesInputDto<IntegracionZohoSVConta> model = new ReportStatusInvoicesInputDto<IntegracionZohoSVConta>()
            {
                InitialDate = initialDate,
                FinalDate = finalDate,
                ClientId = clientId,
                NumberInvoice = consecutive,
                StatusPay = statusPay,
                CodigoMoneda = moneda,
                Type = type
            };

            if (type == null)
                if (fileName.Equals(IntegrationZohoSVConta.SVConta))
                {
                    nameDocument = "SVContaCSV";
                }
                else
                    nameDocument = "FacturaNotaCreditoNotaDebito" + Application.Helpers.EnumHelper.GetDescription(fileName) + "CSV";
            else
                nameDocument = Application.Helpers.EnumHelper.GetDescription(type) + Application.Helpers.EnumHelper.GetDescription(fileName) + "CSV";

            var dt = _reportStatusInvoicesAppService.DataTableZohoCVConta(model, fileName);

            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }
            var encoding = Encoding.GetEncoding("iso-8859-1");

            return File(encoding.GetBytes(sb.ToString()), "application/csv", nameDocument + ".csv");
        }

        private void LibroExcel(IWorkbook workbook, string name, DataTable dt)
        {
            ISheet sheet = workbook.CreateSheet(name);

            IRow row = sheet.CreateRow(0);

            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
            }

            //loops through data
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                row = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                }
            }
        }

    }
}