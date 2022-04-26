using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicoPay.BranchOffices;
using TicoPay.Clients;
using TicoPay.Common;
using TicoPay.Core;
using TicoPay.Core.Common;
using TicoPay.MultiTenancy;
using TicoPay.ReportsSettings;
using TicoPay.Services;

namespace TicoPay.Invoices.XSD
{
    public class GeneratePDF
    {
        // tipos de letra
        private readonly Font TitleFont;
        private readonly Font SubTitleFont;
        private readonly Font TableTitleFont;
        private readonly Font TableFont;
        private readonly Font InfoMessageFont;
        private readonly Font BodyFont;
        private readonly ReportSettingsHelper _reportSettingsHelper;

        public GeneratePDF(ReportSettings reportSettings)
        {
            if (reportSettings == null)
            {
                reportSettings = new ReportSettings();
            }
            _reportSettingsHelper = ReportSettingsHelper.Create(reportSettings);

            TitleFont = _reportSettingsHelper.GetTitleFont();
            SubTitleFont = _reportSettingsHelper.GetSubTitleFont();
            TableTitleFont = _reportSettingsHelper.GetTableTitleFont();
            TableFont = _reportSettingsHelper.GetTableFont();
            InfoMessageFont = _reportSettingsHelper.GetInfoMessageFont();
            BodyFont = _reportSettingsHelper.GetBodyFont();
        }


        public Stream CreatePDFAsStream(Invoice invoice, Client client, Tenant tenant, List<PaymentInvoice> listInfoPayment, List<BranchOffice> infoBranchOffice)
        {
            MemoryStream pdfMemoryStream = new MemoryStream();

            CreatePDF(invoice, client, tenant, null, listInfoPayment, infoBranchOffice, pdfMemoryStream);
            pdfMemoryStream.Position = 0;
            return pdfMemoryStream;
        }

        public Stream CreatePDFNoteAsStream(Invoice invoice, Client client, Tenant tenant, Note note, Taxes.Tax tax)
        {
            MemoryStream pdfMemoryStream = new MemoryStream();
            CreatePDFNote(invoice, client, tenant, note,  pdfMemoryStream);
            pdfMemoryStream.Position = 0;
            return pdfMemoryStream;
        }


        public void CreatePDF(Invoice invoice, Client client, Tenant tenant, List<FacturaElectronicaMedioPago> mediopago, List<PaymentInvoice> listInfoPago, List<BranchOffice> infoBranchOffice)
        {
            CreatePDF(invoice, client, tenant, mediopago, listInfoPago, infoBranchOffice,  new FileStream(Path.Combine(WorkPaths.GetPdfPath(), invoice.VoucherKey + ".pdf"), FileMode.Create));
        }

        public void CreatePDF(Invoice invoice, Client client, Tenant tenant, List<FacturaElectronicaMedioPago> mediopago, List<PaymentInvoice> listInfoPago , List<BranchOffice> infoBranchOffice, Stream fileStream)
        {
            Document doc = _reportSettingsHelper.CreateDocument();

            try
            {
                // Indicamos donde vamos a guardar el documento
                PdfWriter writer = PdfWriter.GetInstance(doc, fileStream);
                if (fileStream is MemoryStream)
                {
                    writer.CloseStream = false;
                }
                writer.PageEvent = _reportSettingsHelper.GetPageEventHandler();
                doc.Open();

                // Adding meta info 
                doc.AddTitle(_reportSettingsHelper.Settings.ReportTitle); // "Factura Electrónica TicoPay"
                doc.AddAuthor("TicoPay");
                doc.Add(BuildHeader(invoice, tenant, mediopago, listInfoPago, infoBranchOffice));
                doc.Add(BuildClientInfo(client,invoice));
                doc.Add(Chunk.NEWLINE);

                //Datos Factura
                PdfPTable tblPrueba = new PdfPTable(8);
                tblPrueba.WidthPercentage = 100;
                tblPrueba.SetWidths(new int[] { 1, 6, 2, 3, 2, 2, 3, 4 });
                tblPrueba.SpacingBefore = 2f;
                tblPrueba.SpacingAfter = 10f;

                // Configuramos el título de las columnas de la tabla
                PdfPCell clNum = new PdfPCell(new Phrase("#", TableTitleFont));
                clNum.BorderWidth = 0;
                clNum.BorderWidthBottom = 0.5f;
                clNum.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clNum.Padding = 4;


                PdfPCell clNombre = new PdfPCell(new Phrase("Descripción / Código", TableTitleFont));
                clNombre.BorderWidth = 0;
                clNombre.BorderWidthBottom = 0.5f;
                clNombre.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clNombre.Padding = 4;

                PdfPCell clcant = new PdfPCell(new Phrase("Cant.", TableTitleFont));
                clcant.BorderWidth = 0;
                clcant.BorderWidthBottom = 0.5f;
                clcant.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clcant.Padding = 4;
                clcant.HorizontalAlignment = 2;

                PdfPCell clPrecio = new PdfPCell(new Phrase("Precio", TableTitleFont));
                clPrecio.BorderWidth = 0;
                clPrecio.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clPrecio.BorderWidthBottom = 0.5f;
                clPrecio.Padding = 4;
                clPrecio.HorizontalAlignment = 2;

                PdfPCell clUnidMedida = new PdfPCell(new Phrase("Unidad", TableTitleFont));
                clUnidMedida.BorderWidth = 0;
                clUnidMedida.Padding = 4;
                clUnidMedida.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clUnidMedida.BorderWidthBottom = 0.5f;
                clUnidMedida.HorizontalAlignment = 2;

                PdfPCell clDescuento = new PdfPCell(new Phrase("%Desc.", TableTitleFont));
                clDescuento.BorderWidth = 0;
                clDescuento.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clDescuento.BorderWidthBottom = 0.5f;
                clDescuento.Padding = 4;
                clDescuento.HorizontalAlignment = 2;

                PdfPCell clImp = new PdfPCell(new Phrase("Impuesto", TableTitleFont));
                clImp.BorderWidth = 0;
                clImp.BorderWidthBottom = 0.5f;
                clImp.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clImp.Padding = 4;
                clImp.HorizontalAlignment = 2;

                PdfPCell clTotal = new PdfPCell(new Phrase("Total", TableTitleFont));
                clTotal.BorderWidth = 0;
                clTotal.BorderWidthBottom = 0.5f;
                clTotal.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clTotal.Padding = 4;
                clTotal.HorizontalAlignment = 2;

                // Añadimos las celdas a la tabla
                tblPrueba.AddCell(clNum);
                tblPrueba.AddCell(clNombre);
                tblPrueba.AddCell(clcant);
                tblPrueba.AddCell(clPrecio);
                tblPrueba.AddCell(clUnidMedida);
                tblPrueba.AddCell(clDescuento);
                tblPrueba.AddCell(clImp);
                tblPrueba.AddCell(clTotal);

                decimal rate = invoice.ChangeType == 1 ? invoice.GetRate() : invoice.ChangeType;
                string moneda = Invoice.RunConversion(tenant,invoice) ? invoice.CodigoMoneda.ToString() : "";
                

                // Llenamos la tabla con información
                foreach (var line in invoice.InvoiceLines.OrderBy(x => x.LineNumber))
                {
                    clNum = new PdfPCell(new Phrase(line.LineNumber.ToString(), TableFont));
                    clNum.BorderWidth = 0;
                    clNum.Padding = 4;

                    string codigo = line.ObtenerCodigo(line);
                    string description = line.Title + "\n";
                    string note = String.Empty;
                    if (line.Note != null)
                        note = line.Note + "\n";
                    string descriptionDiscount = line.DescriptionDiscount;
                    string service = "";
                    if (tenant.ShowServiceCodePdf)
                    {
                        service = description + codigo.ToUpper() + note + descriptionDiscount;
                    }
                    else
                    {
                        service = description + note + descriptionDiscount;
                    }
                    clNombre = new PdfPCell(new Phrase(service, TableFont));
                    clNombre.BorderWidth = 0;
                    clNombre.Padding = 4;

                    clcant = new PdfPCell(new Phrase(line.Quantity.ToString("N", new CultureInfo("en-US")), TableFont));
                    clcant.BorderWidth = 0;
                    clcant.Padding = 4;
                    clcant.HorizontalAlignment = 2;
                    
                    clPrecio = new PdfPCell(new Phrase(Invoice.ConvertCRCToUSD(line.PricePerUnit, 1, tenant, invoice, null), TableFont));
                    clPrecio.BorderWidth = 0;
                    clPrecio.Padding = 4;
                    clPrecio.HorizontalAlignment = 2;

                    string unidadMedida = line.UnitMeasurement.Equals(UnidadMedidaType.Otros) ? line.UnitMeasurement.ToString() + ((line.UnitMeasurementOthers == null) ? "" : ("/" + line.UnitMeasurementOthers.ToString())) : line.UnitMeasurement.ToString();
                    clUnidMedida = new PdfPCell(new Phrase(unidadMedida, TableFont));
                    clUnidMedida.BorderWidth = 0;
                    clUnidMedida.Padding = 4;
                    clUnidMedida.HorizontalAlignment = 2;

                    clDescuento = new PdfPCell(new Phrase(Invoice.ConvertCRCToUSD(line.DiscountPercentage, 1, tenant, invoice, null), TableFont));
                    clDescuento.BorderWidth = 0;
                    clDescuento.Padding = 4;
                    clDescuento.HorizontalAlignment = 2;

                    clImp = new PdfPCell(new Phrase(Invoice.ConvertCRCToUSD(line.TaxAmount, 1, tenant, invoice, null), TableFont));
                    clImp.BorderWidth = 0;
                    clImp.Padding = 4;
                    clImp.HorizontalAlignment = 2;

                    clTotal = new PdfPCell(new Phrase(Invoice.ConvertCRCToUSD(line.LineTotal, 1, tenant, invoice, null), TableFont));
                    clTotal.BorderWidth = 0;
                    clTotal.Padding = 4;
                    clTotal.HorizontalAlignment = 2;

                    // Añadimos las celdas a la tabla
                    tblPrueba.AddCell(clNum);
                    tblPrueba.AddCell(clNombre);
                    tblPrueba.AddCell(clcant);
                    tblPrueba.AddCell(clPrecio);
                    tblPrueba.AddCell(clUnidMedida);
                    tblPrueba.AddCell(clDescuento);
                    tblPrueba.AddCell(clImp);
                    tblPrueba.AddCell(clTotal);
                }

                doc.Add(Chunk.NEWLINE);
                // totales
                //Total gravado
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

                clImp = new PdfPCell(new Phrase("Total Gravado: ", TableTitleFont));
                clImp.BorderWidth = 0;
                clImp.Colspan = 2;
                clImp.Padding = 4;
                clImp.HorizontalAlignment = 2;

                string gravado = Invoice.ConvertCRCToUSD(invoice.TotalGravado, 1, tenant, invoice, null);
                clTotal = new PdfPCell(CreateLabeledTextPdfPCell(moneda + " ", $"{gravado}", BodyFont, TableFont, 2, 1, 0.5f, 5));
                clTotal.BorderWidth = 0;
                clTotal.Padding = 4;
                clTotal.HorizontalAlignment = 2;

                tblPrueba.AddCell(clImp);
                tblPrueba.AddCell(clTotal);

                //total Exento
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });

                clImp = new PdfPCell(new Phrase("Total Exento: ", TableTitleFont));
                clImp.BorderWidth = 0;
                clImp.Colspan = 2;
                clImp.Padding = 4;
                clImp.HorizontalAlignment = 2;

                string exento = Invoice.ConvertCRCToUSD(invoice.TotalExento, 1, tenant, invoice, null);
                clTotal = new PdfPCell(CreateLabeledTextPdfPCell(moneda + " ", $"{exento}", BodyFont, TableFont, 2, 0, 0.5f, 5));
                clTotal.BorderWidth = 0;
                clTotal.Padding = 4;
                clTotal.HorizontalAlignment = 2;

                tblPrueba.AddCell(clImp);
                tblPrueba.AddCell(clTotal);

                //Total desc
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

                clImp = new PdfPCell(new Phrase("Desc. Total: ", TableTitleFont));
                clImp.BorderWidth = 0;
                clImp.Colspan = 2;
                clImp.Padding = 4;
                clImp.HorizontalAlignment = 2;

                string descuento = Invoice.ConvertCRCToUSD(invoice.DiscountAmount, 1, tenant, invoice, null);
                clTotal = new PdfPCell(CreateLabeledTextPdfPCell(moneda + " ", $"{descuento}", BodyFont, TableFont, 2, 0, 0.5f, 5));
                clTotal.BorderWidth = 0;
                clTotal.Padding = 4;
                clTotal.HorizontalAlignment = 2;

                tblPrueba.AddCell(clImp);
                tblPrueba.AddCell(clTotal);

                //Total impuesto
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });

                clImp = new PdfPCell(new Phrase("Total Impuesto: ", TableTitleFont));
                clImp.BorderWidth = 0;
                clImp.Colspan = 2;
                clImp.Padding = 4;
                clImp.HorizontalAlignment = 2;

                string impuesto = Invoice.ConvertCRCToUSD(invoice.TotalTax, 1, tenant, invoice, null);
                clTotal = new PdfPCell(CreateLabeledTextPdfPCell(moneda + " ", $"{impuesto}", BodyFont, TableFont, 2, 0, 0.5f, 5));
                clTotal.BorderWidth = 0;
                clTotal.Padding = 4;
                clTotal.HorizontalAlignment = 2;

                tblPrueba.AddCell(clImp);
                tblPrueba.AddCell(clTotal);


                //Total Factura
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });

                string titleConvert = invoice.CodigoMoneda.Equals(FacturaElectronicaResumenFacturaCodigoMoneda.USD) ? " en Dólares" : " en Colones";
                string titleTotal = invoice.TypeDocument == TypeDocumentInvoice.Ticket ? "Total Tiquete" + (tenant.IsConvertUSD ? titleConvert : "") + ": " : "Total Factura" + (tenant.IsConvertUSD ? titleConvert : "") + ": ";

                clImp = new PdfPCell(new Phrase(titleTotal, TableTitleFont));
                clImp.BorderWidth = 0;
                clImp.Colspan = 3;
                clImp.Padding = 4;
                clImp.HorizontalAlignment = 2;

                string total = Invoice.ConvertCRCToUSD(invoice.Total, 1, tenant, invoice, null);
                clTotal = new PdfPCell(CreateLabeledTextPdfPCell(moneda + " ", $"{total}", BodyFont, TableFont, 2, 0, 0.5f, 5));
                clTotal.BorderWidth = 0;
                clTotal.Padding = 4;
                clTotal.HorizontalAlignment = 2;

                tblPrueba.AddCell(clImp);
                tblPrueba.AddCell(clTotal);

                //Tasa de Día
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 4 });
                                
                if (Invoice.RunConversion(tenant,invoice) && rate != 0)
                {
                    string codigoMoneda = "";
                    if (invoice.CodigoMoneda.Equals(FacturaElectronicaResumenFacturaCodigoMoneda.USD))
                    {
                        total = Invoice.ConvertCRCToUSD(invoice.Total, rate, tenant, invoice, FacturaElectronicaResumenFacturaCodigoMoneda.USD);
                        codigoMoneda = FacturaElectronicaResumenFacturaCodigoMoneda.CRC.ToString();
                        titleConvert = "en Colones";
                    }
                    else if (invoice.CodigoMoneda.Equals(FacturaElectronicaResumenFacturaCodigoMoneda.CRC)) {
                        total = Invoice.ConvertCRCToUSD(invoice.Total, rate, tenant, invoice, FacturaElectronicaResumenFacturaCodigoMoneda.CRC);
                        codigoMoneda = FacturaElectronicaResumenFacturaCodigoMoneda.USD.ToString();
                        titleConvert = "en Dólares";
                    }
                    titleTotal = invoice.TypeDocument == TypeDocumentInvoice.Ticket ? "Total Tiquete " + titleConvert + " ( USD 1 = CRC " + rate.ToString("N", new CultureInfo("en-US")) + " ) : " : "Total Factura " + titleConvert + " ( USD 1 = CRC " + rate.ToString("N", new CultureInfo("en-US")) + " ) : ";
                    clImp = new PdfPCell(new Phrase(titleTotal, TableTitleFont));
                    clImp.BorderWidth = 0;
                    clImp.Colspan = 4;
                    clImp.Padding = 4;
                    clImp.HorizontalAlignment = 2;

                    clTotal = new PdfPCell(CreateLabeledTextPdfPCell(codigoMoneda + " ", $"{total}", BodyFont, TableFont, 2, 0, 0.5f, 5));
                    clTotal.BorderWidth = 0;
                    clTotal.Padding = 4;
                    clTotal.HorizontalAlignment = 2;

                    tblPrueba.AddCell(clImp);
                    tblPrueba.AddCell(clTotal);

                }

                doc.Add(tblPrueba);
                if (invoice.IsContingency)
                {
                    doc.Add(BuildContingency(invoice.ConsecutiveNumberContingency, invoice.DateContingency.Value, invoice.ReasonContingency));
                }

                doc.Add(Chunk.NEWLINE);

                PdfPTable tblContent = new PdfPTable(2);
                tblContent.WidthPercentage = 100;
                tblContent.DefaultCell.BorderWidth = 0;
                tblContent.DefaultCell.Border = 0;
                tblContent.SetWidths(new int[] { 80, 20 });
                tblContent.SpacingBefore = 3f;
                tblContent.SpacingAfter = 3f;

                // Seccion para agregar Observaciones
                tblContent.AddCell(BuildObservaciones(invoice));

                //Codigo QR

                var qr = Image.GetInstance(System.Drawing.Image.FromStream(new MemoryStream(invoice.QRCode)), ImageFormat.Png);
                qr.Border = Rectangle.BOX;
                qr.BorderWidth = 0;
                tblContent.AddCell(new PdfPCell(qr) { BorderWidth = 0, Padding = 20 });
                doc.Add(tblContent);

                doc.Add(Chunk.NEWLINE);
                // inserta  nota
                if (tenant.ValidateHacienda == true)
                {
                    doc.Add(new Paragraph("Incluido en el Registro de Facturación Electrónica, según normativa " + ConfigurationManager.AppSettings["XML.NumeroResolucion"], InfoMessageFont));
                }
                else
                {
                    doc.Add(new Paragraph("Autorizado mediante resolución 1197 de la D.G.T.D", InfoMessageFont));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                doc.Close();
            }
        }

        public void CreatePDFNote(Invoice invoice, Client client, Tenant tenant, Note note)
        {
            CreatePDFNote(invoice, client, tenant, note,  new FileStream(Path.Combine(WorkPaths.GetPdfPath(), "note_" + note.VoucherKey + ".pdf"), FileMode.Create));
        }

        public void CreatePDFNote(Invoice invoice, Client client, Tenant tenant, Note note,  Stream fileStream)
        {
            Document doc = new Document(PageSize.LETTER);

            try
            {
                // Indicamos donde vamos a guardar el documento
                PdfWriter writer = PdfWriter.GetInstance(doc, fileStream);
                if (fileStream is MemoryStream)
                {
                    writer.CloseStream = false;
                }
                doc.Open();

                // Adding meta info 
                doc.AddTitle("Documento Electrónico TicoPay");
                doc.AddAuthor("TicoPay");

                doc.Add(BuildHeaderNote(note, tenant));

                // informacion del cliente
                doc.Add(BuildClientInfo(client, invoice));
                doc.Add(Chunk.NEWLINE);

                //Datos Factura
                PdfPTable tblPrueba = new PdfPTable(8);
                tblPrueba.WidthPercentage = 100;
                tblPrueba.SetWidths(new int[] { 1, 6, 2, 3, 2, 2, 3, 4 });
                tblPrueba.SpacingBefore = 10f;
                tblPrueba.SpacingAfter = 10f;

                // Configuramos el título de las columnas de la tabla
                PdfPCell clNum = new PdfPCell(new Phrase("#", TableTitleFont));
                clNum.BorderWidth = 0;
                clNum.BorderWidthBottom = 0.75f;
                clNum.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clNum.Padding = 5;

                PdfPCell clNombre = new PdfPCell(new Phrase("Descripción", TableTitleFont));
                clNombre.BorderWidth = 0;
                clNombre.BorderWidthBottom = 0.75f;
                clNombre.Padding = 5;
                clNombre.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);

                PdfPCell clcant = new PdfPCell(new Phrase("Cant.", TableTitleFont));
                clcant.BorderWidth = 0;
                clcant.BorderWidthBottom = 0.75f;
                clcant.Padding = 5;
                clcant.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clcant.HorizontalAlignment = 2;

                PdfPCell clPrecio = new PdfPCell(new Phrase("Precio", TableTitleFont));
                clPrecio.BorderWidth = 0;
                clPrecio.Padding = 5;
                clPrecio.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clPrecio.BorderWidthBottom = 0.75f;
                clPrecio.HorizontalAlignment = 2;

                PdfPCell clUnidMedida = new PdfPCell(new Phrase("Unid Med.", TableTitleFont));
                clUnidMedida.BorderWidth = 0;
                clUnidMedida.Padding = 5;
                clUnidMedida.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clUnidMedida.BorderWidthBottom = 0.75f;
                clUnidMedida.HorizontalAlignment = 2;

                PdfPCell clDescuento = new PdfPCell(new Phrase("%Desc.", TableTitleFont));
                clDescuento.BorderWidth = 0;
                clDescuento.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clDescuento.BorderWidthBottom = 0.75f;
                clDescuento.Padding = 5;
                clDescuento.HorizontalAlignment = 2;


                PdfPCell clImp = new PdfPCell(new Phrase("Impuesto", TableTitleFont));
                clImp.BorderWidth = 0;
                clImp.Padding = 5;
                clImp.BorderWidthBottom = 0.75f;
                clImp.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clImp.HorizontalAlignment = 2;

                PdfPCell clTotal = new PdfPCell(new Phrase("Total", TableTitleFont));
                clTotal.BorderWidth = 0;
                clTotal.Padding = 5;
                clTotal.BorderWidthBottom = 0.75f;
                clTotal.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clTotal.HorizontalAlignment = 2;

                // Añadimos las celdas a la tabla
                tblPrueba.AddCell(clNum);
                tblPrueba.AddCell(clNombre);
                tblPrueba.AddCell(clcant);
                tblPrueba.AddCell(clPrecio);
                tblPrueba.AddCell(clUnidMedida);
                tblPrueba.AddCell(clDescuento);
                tblPrueba.AddCell(clImp);
                tblPrueba.AddCell(clTotal);

                decimal rate = note.ChangeType == 1 ? note.GetRate() : note.ChangeType;
                string moneda = Note.RunConversion(tenant, note) ? note.CodigoMoneda.ToString() : "";

                // Llenamos la tabla con información
                foreach (var line in note.NotesLines.OrderBy(x => x.LineNumber))
                {
                    clNum = new PdfPCell(new Phrase(line.LineNumber.ToString(), TableFont));
                    clNum.BorderWidth = 0;
                    clNum.Padding = 5;


                    var service = line.Title + "\n" + line.Notes + "\n" + line.DescriptionDiscount;
                    clNombre = new PdfPCell(new Phrase(service, TableFont));
                    clNombre.BorderWidth = 0;
                    clNombre.Padding = 5;

                    clcant = new PdfPCell(new Phrase(line.Quantity.ToString("N", new CultureInfo("en-US")), TableFont));
                    clcant.BorderWidth = 0;
                    clcant.Padding = 5;
                    clcant.HorizontalAlignment = 2;

                    clPrecio = new PdfPCell(new Phrase(Note.ConvertUSDToCRC(line.PricePerUnit, 1, tenant, note, null), TableFont));
                    clPrecio.BorderWidth = 0;
                    clPrecio.Padding = 5;
                    clPrecio.HorizontalAlignment = 2;

                    clUnidMedida = new PdfPCell(new Phrase(line.UnitMeasurement.ToString(), TableFont));
                    clUnidMedida.BorderWidth = 0;
                    clUnidMedida.Padding = 5;
                    clUnidMedida.HorizontalAlignment = 2;

                    clDescuento = new PdfPCell(new Phrase(Note.ConvertUSDToCRC(line.DiscountPercentage, 1, tenant, note, null), TableFont));
                    clDescuento.BorderWidth = 0;
                    clDescuento.Padding = 5;
                    clDescuento.HorizontalAlignment = 2;

                    clImp = new PdfPCell(new Phrase(Note.ConvertUSDToCRC(line.TaxAmount, 1, tenant, note, null), TableFont));
                    clImp.BorderWidth = 0;
                    clImp.Padding = 5;
                    clImp.HorizontalAlignment = 2;

                    clTotal = new PdfPCell(new Phrase(Note.ConvertUSDToCRC(line.LineTotal, 1, tenant, note, null), TableFont));
                    clTotal.BorderWidth = 0;
                    clTotal.Padding = 5;
                    clTotal.HorizontalAlignment = 2;

                    // Añadimos las celdas a la tabla
                    tblPrueba.AddCell(clNum);
                    tblPrueba.AddCell(clNombre);
                    tblPrueba.AddCell(clcant);
                    tblPrueba.AddCell(clPrecio);
                    tblPrueba.AddCell(clUnidMedida);
                    tblPrueba.AddCell(clDescuento);
                    tblPrueba.AddCell(clImp);
                    tblPrueba.AddCell(clTotal);
                }

                // totales
                //Total gravado
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

                clImp = new PdfPCell(new Phrase("Total Gravado: ", TableTitleFont));
                clImp.BorderWidth = 0;
                clImp.Colspan = 2;
                clImp.Padding = 5;
                clImp.HorizontalAlignment = 2;

                decimal totalgravado = 0;
                if (note.TotalGravado != 0)
                    totalgravado = note.TotalGravado;

                string gravado = Note.ConvertUSDToCRC(totalgravado, 1, tenant, note, null);
                clTotal = new PdfPCell(CreateLabeledTextPdfPCell(moneda + " ", $"{gravado}", BodyFont, TableFont, 2, 0, 0.5f, 5));
                clTotal.BorderWidth = 0;
                clTotal.Padding = 5;
                clTotal.HorizontalAlignment = 2;

                tblPrueba.AddCell(clImp);
                tblPrueba.AddCell(clTotal);

                //total Exento
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

                clImp = new PdfPCell(new Phrase("Total Exento: ", TableTitleFont));
                clImp.BorderWidth = 0;
                clImp.Colspan = 2;
                clImp.Padding = 5;
                clImp.HorizontalAlignment = 2;

                decimal totalexento = 0;
                if (note.TotalExento != 0)
                    totalexento = note.TotalExento;

                string exento = Note.ConvertUSDToCRC(totalexento, 1, tenant, note, null);
                clTotal = new PdfPCell(CreateLabeledTextPdfPCell(moneda + " ", $"{exento}", BodyFont, TableFont, 2, 0, 0.5f, 5));
                clTotal.BorderWidth = 0;
                clTotal.Padding = 5;
                clTotal.HorizontalAlignment = 2;

                tblPrueba.AddCell(clImp);
                tblPrueba.AddCell(clTotal);

                //Total desc
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

                clImp = new PdfPCell(new Phrase("Desc. Total: ", TableTitleFont));
                clImp.BorderWidth = 0;
                clImp.Colspan = 2;
                clImp.Padding = 5;
                clImp.HorizontalAlignment = 2;

                string descuento = Note.ConvertUSDToCRC(note.DiscountAmount, 1, tenant, note, null);
                clTotal = new PdfPCell(CreateLabeledTextPdfPCell(moneda + " ", $"{descuento}", BodyFont, TableFont, 2, 0, 0.5f, 5));
                clTotal.BorderWidth = 0;
                clTotal.Padding = 5;
                clTotal.HorizontalAlignment = 2;

                tblPrueba.AddCell(clImp);
                tblPrueba.AddCell(clTotal);

                //Total impuesto
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });


                clImp = new PdfPCell(new Phrase("Total Impuesto: ", TableTitleFont));
                clImp.BorderWidth = 0;
                clImp.Colspan = 2;
                clImp.Padding = 5;
                clImp.HorizontalAlignment = 2;

                string impuesto = Note.ConvertUSDToCRC(note.TaxAmount, 1, tenant, note, null);
                clTotal = new PdfPCell(CreateLabeledTextPdfPCell(moneda + " ", $"{impuesto}", BodyFont, TableFont, 2, 0, 0.5f, 5));
                clTotal.BorderWidth = 0;
                clTotal.Padding = 5;
                clTotal.HorizontalAlignment = 2;

                tblPrueba.AddCell(clImp);
                tblPrueba.AddCell(clTotal);

                //Total Factura
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

                string titleConvert = note.CodigoMoneda.Equals(NoteCodigoMoneda.USD) ? " en Dólares" : " en Colones";      

                clImp = new PdfPCell(new Phrase("Total Nota " + (tenant.IsConvertUSD ? titleConvert : "") + ": ", TableTitleFont));
                clImp.BorderWidth = 0;
                clImp.Colspan = 3;
                clImp.Padding = 5;
                clImp.HorizontalAlignment = 2;

                string total = Note.ConvertUSDToCRC(note.Total, 1, tenant, note, null);
                clTotal = new PdfPCell(CreateLabeledTextPdfPCell(moneda + " ", $"{total}", BodyFont, TableFont, 2, 0, 0.5f, 5));
                clTotal.BorderWidth = 0;
                clTotal.Padding = 5;
                clTotal.HorizontalAlignment = 2;

                tblPrueba.AddCell(clImp);
                tblPrueba.AddCell(clTotal);

                //Tasa de Día
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

                if (note.IsContingency)
                {
                    doc.Add(BuildContingency(note.ConsecutiveNumberContingency, note.DateContingency.Value, note.ReasonContingency));
                }

                doc.Add(Chunk.NEWLINE);

                if (Note.RunConversion(tenant,note))
                {
                    string codigoMoneda = "";
                    if (note.CodigoMoneda.Equals(NoteCodigoMoneda.USD))
                    {
                        total = Note.ConvertUSDToCRC(note.Total, rate, tenant, note, NoteCodigoMoneda.USD);
                        codigoMoneda = NoteCodigoMoneda.CRC.ToString();
                        titleConvert = "en Colones";
                }
                    else if (note.CodigoMoneda.Equals(NoteCodigoMoneda.CRC))
                {
                        total = Note.ConvertUSDToCRC(note.Total, rate, tenant, note, NoteCodigoMoneda.CRC);
                        codigoMoneda = NoteCodigoMoneda.USD.ToString();
                        titleConvert = "en Dólares";
                }

                    string titleTotal = "Total Nota " + titleConvert + " (USD 1 = CRC " + rate.ToString("N", new CultureInfo("en-US")) + " ):";
                    clImp = new PdfPCell(new Phrase(titleTotal, TableTitleFont));
                    clImp.BorderWidth = 0;
                    clImp.Colspan = 4;
                    clImp.Padding = 5;
                    clImp.HorizontalAlignment = 2;

                    clTotal = new PdfPCell(CreateLabeledTextPdfPCell( codigoMoneda + " ", $"{total}", BodyFont, TableFont, 2, 0, 0.5f, 5));
                    clTotal.BorderWidth = 0;
                    clTotal.Padding = 5;
                    clTotal.HorizontalAlignment = 2;

                    tblPrueba.AddCell(clImp);
                    tblPrueba.AddCell(clTotal);
                }

                doc.Add(tblPrueba);
                doc.Add(Chunk.NEWLINE);

                PdfPTable tblContent = new PdfPTable(2);
                tblContent.WidthPercentage = 100;
                tblContent.SetWidths(new int[] { 83, 17 });
                tblContent.SpacingBefore = 10f;
                tblContent.SpacingAfter = 10f;

                tblContent.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 0 });

                //Codigo QR

                note.byteArrayToImage(note.QRCode, note.VoucherKey);
                var qr = Image.GetInstance(Path.Combine(WorkPaths.GetQRPath(), "note_" + note.VoucherKey + ".png"));
                qr.BorderWidth = 0;
                tblContent.AddCell(new PdfPCell(qr) { BorderWidth = 0, Padding = 0 });
                doc.Add(tblContent);

                doc.Add(Chunk.NEWLINE);

                if (tenant.ValidateHacienda == true)
                {
                    doc.Add(new Paragraph("Incluido en el Registro de Facturación Electrónica, según normativa " + ConfigurationManager.AppSettings["XML.NumeroResolucion"], InfoMessageFont));
            }
                else
                {
                    doc.Add(new Paragraph("Autorizado mediante resolución 1197 de la D.G.T.D", InfoMessageFont));
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                doc.Close();
            }
        }

        private PdfPTable BuildHeader(Invoice invoice, Tenant tenant, List<FacturaElectronicaMedioPago> mediopago, List<PaymentInvoice> listInfoPago, List<BranchOffice> infoBranchOffice)
        {
            //----------------------------------------------------------------------------------------//
            //                                                        Factura #: 00100001010000000080 //
            //                    Clave Numérica #:50606061800310174178800100001010000003502196437526 // 
            //                                                   Codigo de referencia interna #: 1234 // ---- Lo agrega si existe
            //----------------------------------------------------------------------------------------//
            //               |  TENANT NOMBRE COMERCIAL                                               //
            //               |  Razón Social: XXXXXXXXXXXXX                                          //
            //               |  Cedula <tipo>: XXXXXXXXXXXXX                                          //
            //     Logo      |  Telef.: 123-00000000, Fax: 123-000000000                              //
            //               |  Email: cuenta@servidor.com                                            //
            //               |  Dirección:                                                            //
            //----------------------------------------------------------------------------------------//
            //                                                           Fecha de emision: 02/11/2017 //
            //                                                                        Moneda: USD      //
            //                                                            Condición de venta: Contado //
            //                                                                 Medio de pago: Efectivo//
            //                                                                  Banco:(si es deposito)//
            //                                                                  Emisor:(si es tarjeta)//
            //                                                                Estado factura: Pagada  //
            //----------------------------------------------------------------------------------------//

            PdfPTable header = new PdfPTable(2);
            header.DefaultCell.Border = 0;
            header.WidthPercentage = 100;
            header.SetTotalWidth(new float[] { 15, 100 });

            int tenantDataColspan = 2;
            string invoiceNumber = tenant.ValidateHacienda ? invoice.ConsecutiveNumber : Convert.ToInt64(invoice.ConsecutiveNumber).ToString();
            string typedocumet = invoice.TypeDocument == TypeDocumentInvoice.Ticket ? $"Tiquete Electrónico #: {invoiceNumber}" : $"Factura Electrónica #: {invoiceNumber}";
            header.AddCell(CreateTextPdfPCell(typedocumet, SubTitleFont, 2, 2));
            header.AddCell(CreateTextPdfPCell($"Clave Numérica #: {invoice.VoucherKey}", SubTitleFont, 2, 2));

            //Add the External reference number if exists
            if (!string.IsNullOrEmpty(invoice.ExternalReferenceNumber))
            {
                header.AddCell(CreateTextPdfPCell($"Número de referencia interna: {invoice.ExternalReferenceNumber}", SubTitleFont, 2, 2));
            }

            header.AddCell(CreateEmptyLine(2));

            if (tenant.Logo != null && tenant.Logo.Count() > 0)
            {
                try
                {
                var logo = Image.GetInstance(System.Drawing.Image.FromStream(new MemoryStream(tenant.Logo)), ImageFormat.Png);
                logo.Border = Rectangle.BOX;
                logo.BorderColor = BaseColor.WHITE;
                logo.BorderWidth = 3f;
                header.AddCell(logo);
                tenantDataColspan = 1;
                }
                catch (System.ArgumentException ex)
                {
                    header.AddCell(CreateTextPdfPCell("Formato de logo no compatible.", SubTitleFont));
                    tenantDataColspan = 1;
                }
                catch (Exception)
                {
                    header.AddCell(CreateTextPdfPCell("No se pudo cargar el logo.", SubTitleFont));
                    tenantDataColspan = 1;
                }
            }

            PdfPTable tenantData = new PdfPTable(1);
            tenantData.AddCell(CreateTextPdfPCell(tenant.BussinesName, TitleFont));
            tenantData.AddCell(CreateLabeledTextPdfPCell("Razón Social:", tenant.Name, SubTitleFont, BodyFont));
            tenantData.AddCell(CreateLabeledTextPdfPCell($"{tenant.IdentificationTypeToString()}:", tenant.IdentificationNumber, SubTitleFont, BodyFont));
            tenantData.AddCell(CreateLabeledTextPdfPCell("Teléf.:", $"{tenant.PhoneNumber}, Móvil: {tenant.Fax}", SubTitleFont, BodyFont));
            tenantData.AddCell(CreateLabeledTextPdfPCell("Email:", tenant.Email, SubTitleFont, BodyFont));
            if (tenant.IsAddressShort)
                tenantData.AddCell(CreateLabeledTextPdfPCell("Dirección:", $"{tenant.AddressShort}", SubTitleFont, BodyFont));
            else
                tenantData.AddCell(CreateLabeledTextPdfPCell("Dirección:", $"{tenant.Barrio.NombreBarrio}, {tenant.Barrio.Distrito.NombreDistrito}, {tenant.Barrio.Distrito.Canton.NombreCanton}, {tenant.Barrio.Distrito.Canton.Provincia.NombreProvincia}, {tenant.Address}", SubTitleFont, BodyFont));

            if (infoBranchOffice != null)
            {
                string suc = "";
                string sucDir = "";
                foreach (var list in infoBranchOffice)
                {
                    suc = list.Name;
                    sucDir = list.Location;
                }
                if (suc != "")
                    tenantData.AddCell(CreateLabeledTextPdfPCell("Sucursal:", suc, SubTitleFont, BodyFont));
                if (sucDir != "")
                    tenantData.AddCell(CreateLabeledTextPdfPCell("Dirección Sucursal:", sucDir, SubTitleFont, BodyFont));

            }
            header.AddCell(new PdfPCell(tenantData) { BorderWidth = 0, Colspan = tenantDataColspan, PaddingLeft = 3f });



            header.AddCell(CreateEmptyLine(2));
            string fechaEmision, horaEmision;
            if (invoice.IsContingency)
            {
                fechaEmision = invoice.DateContingency.Value.ToString("dd/MM/yyyy");
                horaEmision = invoice.DateContingency.Value.ToString("hh:mm:ss");
            }
            else
            {
                fechaEmision = invoice.DueDate.ToString("dd/MM/yyyy");
                horaEmision = invoice.DueDate.ToString("hh:mm:ss");
            }
            header.AddCell(CreateLabeledTextPdfPCell("Fecha de emisión:", fechaEmision, SubTitleFont, BodyFont, 2, 2));
            header.AddCell(CreateLabeledTextPdfPCell("Hora de emisión:", horaEmision, SubTitleFont, BodyFont, 2, 2));


            if (invoice.ConditionSaleType == FacturaElectronicaCondicionVenta.Credito)
                header.AddCell(CreateLabeledTextPdfPCell("Fecha de vencimiento:", invoice.ExpirationDate.Value.ToString("dd/MM/yyyy"), SubTitleFont, BodyFont, 2, 2));
            header.AddCell(CreateLabeledTextPdfPCell("Moneda:", invoice.CodigoMoneda.ToString(), SubTitleFont, BodyFont, 2, 2));
            header.AddCell(CreateLabeledTextPdfPCell("Condición de venta:", invoice.ConditionSaleTypeToString(), SubTitleFont, BodyFont, 2, 2));
            if (invoice.Status == Status.Completed && mediopago != null)
            {
                string medioPago = "";
                int cont = 0;
                foreach (FacturaElectronicaMedioPago pago in mediopago)
                {
                    if (cont != 0 && cont < mediopago.Count())
                        medioPago = medioPago + " / ";

                    if (pago == FacturaElectronicaMedioPago.Transferencia_Deposito_Bancario)
                        medioPago = medioPago + "Depósito/Transferencia";
                    else
                        medioPago = medioPago + pago.ToString();

                    cont = cont + 1;
                }

                header.AddCell(CreateLabeledTextPdfPCell("Medio de pago:", medioPago, SubTitleFont, BodyFont, 2, 2));
            }
            else if (invoice.Status == Status.Completed && mediopago == null)
            {
                StringBuilder medioPago = new StringBuilder();
                int y = 0;
                foreach (var payment in invoice.InvoicePaymentTypes)
                {
                    medioPago.Append(payment.Payment.PaymetnMethodType.GetDescription() + ((y < invoice.InvoicePaymentTypes.Count - 1) ? "/" : ""));
                    y++;
                }

                header.AddCell(CreateLabeledTextPdfPCell("Medio de pago:", medioPago.ToString(), SubTitleFont, BodyFont, 2, 2));
            }

            var userC = "";
            var bankS = "";
            if (listInfoPago != null)
            {
                foreach (var pay in listInfoPago)
                {
                    if (pay.PaymetnMethodType == PaymetnMethodType.Card)
                        userC = pay.UserCard;

                    if (pay.PaymetnMethodType == PaymetnMethodType.Deposit)
                        bankS = pay.UserCard;
                }
            if (userC != "")
            header.AddCell(CreateLabeledTextPdfPCell("Emisor:", userC, SubTitleFont, BodyFont, 2, 2));
            if (bankS != "")
                header.AddCell(CreateLabeledTextPdfPCell("Banco:", bankS, SubTitleFont, BodyFont, 2, 2));
            }

            string type = invoice.TypeDocument == TypeDocumentInvoice.Ticket ? "Estado tiquete:" : "Estado factura:";
            header.AddCell(CreateLabeledTextPdfPCell(type, invoice.Status.GetDescription(), SubTitleFont, BodyFont, 2, 2));

            return header;
        }

        private PdfPTable BuildClientInfo(Client client, Invoice invoice)
        {
            //----------------------------------------------------------------------------------------//
            // SEÑOR(ES):                                                                             //
            //----------------------------------------------------------------------------------------//
            // Identificacion:                                   | Codigo Cliente:                    //
            //----------------------------------------------------------------------------------------//
            // Telefono:                                         | Email:                             //
            //----------------------------------------------------------------------------------------//
            // Direccion:                                                                             //
            //----------------------------------------------------------------------------------------//
            string tituloid = "Identificación: ";
            string identificacion = string.Empty;

          //  string codeClient = string.Empty;

            if (!string.IsNullOrWhiteSpace(invoice.ClientIdentification))
            {
                tituloid = string.Format("Identificación{0}: ", (invoice.ClientIdentificationType == IdentificacionTypeTipo.NoAsiganda ? " Extranjera" : string.Empty));
                identificacion = (invoice.ClientIdentificationType == IdentificacionTypeTipo.NoAsiganda ? invoice.ClientIdentification : ((IdentificacionTypeTipo)invoice.ClientIdentificationType).GetDescription() + " " + invoice.ClientIdentification);
            }

            string nameclient = (!string.IsNullOrWhiteSpace(invoice.ClientName)) ? invoice.ClientName : string.Empty;
            string codeClient = string.Empty;
            string phone = (!string.IsNullOrWhiteSpace(invoice.ClientPhoneNumber)) ? invoice.ClientPhoneNumber : string.Empty;
            string Mobilphone = (!string.IsNullOrWhiteSpace(invoice.ClientMobilNumber)) ? invoice.ClientMobilNumber : string.Empty;
            string email = (!string.IsNullOrWhiteSpace(invoice.ClientEmail)) ? invoice.ClientEmail : string.Empty;
            string address = "N/D";

            if ((client != null) && (invoice.TypeDocument == TypeDocumentInvoice.Invoice))
            {
                tituloid = string.Format("Identificación{0}: ", (client.IdentificationType == IdentificacionTypeTipo.NoAsiganda ? " Extranjera" : string.Empty));
                identificacion = (client.IdentificationType == IdentificacionTypeTipo.NoAsiganda ? client.IdentificacionExtranjero : ((IdentificacionTypeTipo)client.IdentificationType).GetDescription() + " " + client.Identification);
                nameclient = client.Name + " " + (client.IdentificationType == XSD.IdentificacionTypeTipo.Cedula_Juridica ? string.Empty : client.LastName);
                 codeClient = client.Code.ToString();
                 phone = client.PhoneNumber;
                Mobilphone = client.MobilNumber;
                 email = client.Email;
                if (client.Barrio != null)
                {
                    address = client.Barrio.NombreBarrio + ", " + client.Barrio.Distrito.NombreDistrito + ", " + client.Address;
                }

            }

            PdfPTable clientInfo = new PdfPTable(2);
            clientInfo.SpacingBefore = 10;
            clientInfo.SpacingAfter = 5;
            clientInfo.WidthPercentage = 100;
            clientInfo.SetWidths(new int[] { 60, 40 });
            clientInfo.AddCell(CreateLabeledTextPdfPCell("Señor(es): ", $"{nameclient}", TableTitleFont, BodyFont, 2, borderWidth: 0.5f, padding: 4));
            clientInfo.AddCell(CreateLabeledTextPdfPCell(tituloid, $"{identificacion}", TableTitleFont, BodyFont, borderWidth: 0.5f, padding: 4));
            clientInfo.AddCell(CreateLabeledTextPdfPCell("Código Cliente: ", $"{codeClient}", TableTitleFont, BodyFont, borderWidth: 0.5f, padding: 4));
            clientInfo.AddCell(CreateLabeledTextPdfPCell("Móvil.: ", $"{Mobilphone}, Teléf.: {phone}", TableTitleFont, BodyFont, borderWidth: 0.5f, padding: 4));
            clientInfo.AddCell(CreateLabeledTextPdfPCell("Email: ", $"{email}", TableTitleFont, BodyFont, borderWidth: 0.5f, padding: 4));
            clientInfo.AddCell(CreateLabeledTextPdfPCell("Dirección: ", $"{address}", TableTitleFont, BodyFont, 2, 0, 0.5f, 4));

            return clientInfo;
        }

        private PdfPTable BuildContingency(string number, DateTime date, string reason)
        {
            //----------------------------------------------------------------------------------------//
            // Datos Comprobante provisional de contingencia                                           //
            //----------------------------------------------------------------------------------------//
            // Número Consecutivo:                               | Fecha:                             //
            //----------------------------------------------------------------------------------------//         
            // Motivo:                                                                                // 
            //                                                                                        //
            //----------------------------------------------------------------------------------------//

            PdfPTable contingency = new PdfPTable(2);
            contingency.SpacingBefore = 10;
            contingency.SpacingAfter = 5;
            contingency.WidthPercentage = 100;
            contingency.SetWidths(new int[] { 60, 40 });
            contingency.AddCell(CreateLabeledTextPdfPCell("Datos Comprobante provisional de contingencia", "", TableTitleFont, BodyFont, 2, borderWidth: 0.5f, padding: 4));
            contingency.AddCell(CreateLabeledTextPdfPCell("Número Consecutivo:", $"{number}", TableTitleFont, BodyFont, borderWidth: 0.5f, padding: 4));
            contingency.AddCell(CreateLabeledTextPdfPCell("Fecha: ", $"{date.ToString("dd/MM/yyyy")}", TableTitleFont, BodyFont, borderWidth: 0.5f, padding: 4));            
            contingency.AddCell(CreateLabeledTextPdfPCell("Motivo: ", $"{reason}", TableTitleFont, BodyFont, 2, 0, 0.5f, 4));

            return contingency;
        }

        private PdfPTable BuildFooter()
        {
            //----------------------------------------------------------------------------------------//
            //                                         TicoPay                                        //
            //                                  https://ticopays.com                                  //
            //   email: soporte @ticopays.com                                telf.: 123 - 87046985    //
            //----------------------------------------------------------------------------------------//

            PdfPTable footer = new PdfPTable(2);
            footer.DefaultCell.Border = 0;
            footer.WidthPercentage = 100;
            footer.AddCell(CreateTextPdfPCell("TicoPay", InfoMessageFont, 2, 1));
            footer.AddCell(CreateTextPdfPCell("https://ticopays.com", InfoMessageFont, 2, 1));
            footer.AddCell(CreateTextPdfPCell("email: soporte @ticopays.com", InfoMessageFont, 1, 0));
            footer.AddCell(CreateTextPdfPCell("telf.: 123-87046985", InfoMessageFont, 1, 2));

            return footer;
        }

        private PdfPTable BuildObservaciones(Invoice invoice)
        {
            PdfPTable clObservaciones = new PdfPTable(2);
            clObservaciones.WidthPercentage = 100;
            clObservaciones.DefaultCell.Border = 0;
            clObservaciones.SetWidths(new int[] { 40, 40 });
            clObservaciones.DefaultCell.BorderWidth = 0;
            clObservaciones.SetTotalWidth(new float[] { 50, 50 }); 
            

            if (invoice.TenantId == 2)
            {
                if (invoice.GeneralObservation != null || invoice.GeneralObservation != "")
                {
                    clObservaciones.AddCell(CreateLabeledTextPdfPCell(invoice.GeneralObservation, "", TableTitleFont, BodyFont, 2, borderWidth: 0, padding: 4));
                }
                clObservaciones.AddCell(CreateEmptyLine(2));
                clObservaciones.AddCell(CreateLabeledTextPdfPCell("CANALES DEL BANCO NACIONAL","", TableTitleFont, BodyFont,2, borderWidth: 0, padding: 4));
                clObservaciones.AddCell(CreateLabeledTextPdfPCell("Puede Pagar sus facturas a través de todos los canales del Banco Nacional. Desde la opción “Pagos” selecciona la opción “Facturación”, luego “Ticopay”. Para consultar sus facturas pendientes digite el Código del Cliente, que aparece en su Factura Electrónica, en el espacio “Valor a consultar”.", "", TableTitleFont, BodyFont, 2, borderWidth: 0, padding: 4));
                clObservaciones.AddCell(CreateLabeledTextPdfPCell("DEPÓSITOS O TRANSFERENCIAS (Banco Costa Rica – Cuenta Corriente):","", TableTitleFont, BodyFont, 2, borderWidth: 0, padding: 4));
                clObservaciones.AddCell(CreateLabeledTextPdfPCell("Cuenta en Colones (CRC)","", SubTitleFont, BodyFont, borderWidth: 0.5f, padding: 4));
                clObservaciones.AddCell(CreateLabeledTextPdfPCell("Cuenta en Dólares (USD)", "", SubTitleFont, BodyFont, borderWidth: 0.5f, padding: 4));
                clObservaciones.AddCell(CreateLabeledTextPdfPCell("001-0463071-8","", BodyFont, BodyFont, borderWidth: 0.5f, padding: 4));
                clObservaciones.AddCell(CreateLabeledTextPdfPCell("001-0463074-2", "", BodyFont, BodyFont, borderWidth: 0.5f, padding: 4));
                clObservaciones.AddCell(CreateLabeledTextPdfPCell("Cuenta Cliente  15201001046307189","", BodyFont, BodyFont, borderWidth: 0.5f, padding: 4));
                clObservaciones.AddCell(CreateLabeledTextPdfPCell("Cuenta Cliente 15201001046307427","", BodyFont, BodyFont, borderWidth: 0.5f, padding: 4));
            }
            else
            {
                if (invoice.GeneralObservation != null || invoice.GeneralObservation != "")
                {
                    clObservaciones.AddCell(CreateTextPdfPCell(invoice.GeneralObservation, SubTitleFont, 2));
                }
                clObservaciones.AddCell(CreateEmptyLine(1));
            }


            //if (ObservationMensaje == null || ObservationMensaje == "")
            //{
            //    clObservaciones = new PdfPCell() { BorderWidth = 0, Padding = 0 };
            //}

            //    clObservaciones = new PdfPCell(new Phrase(ObservationMensaje, TableTitleFont))
            //    {
            //        BorderWidth = 0,
            //        BackgroundColor = new BaseColor(224,224,224),
            //        Padding = 5,
            //        HorizontalAlignment = 0,
            //        VerticalAlignment = Element.ALIGN_MIDDLE
            //    };

            return clObservaciones;
        }

        private PdfPCell CreateLabeledTextPdfPCell(string label, string text, Font labelFont, Font textFont, int colspan = 1, int textAlignment = 0, float borderWidth = 0, float padding = 1)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Add(new Chunk(label, labelFont));
            paragraph.Add(new Chunk(text, textFont));

            return new PdfPCell(paragraph)
            {
                Colspan = colspan,
                HorizontalAlignment = textAlignment,
                BorderWidth = borderWidth,
                Padding = padding
            };
        }

        private PdfPCell CreateTextPdfPCell(string text, Font font, int colspan = 1, int textAlignment = 0, float borderWidth = 0, float padding = 1)
        {
            return new PdfPCell(new Paragraph(text, font))
            {
                Colspan = colspan,
                HorizontalAlignment = textAlignment,
                BorderWidth = borderWidth,
                Padding = padding
            };
        }

        private PdfPCell CreateEmptyLine(int colspan = 1, int borderWidth = 0)
        {
            return new PdfPCell(new Paragraph(Environment.NewLine))
            {
                Colspan = colspan,
                BorderWidth = borderWidth
            };
        }

        public void CreatePDFNoteCorregida(Note noteoriginal, Client client, Tenant tenant, Note note, Service service)
        {
            Document doc = new Document(PageSize.LETTER);

            // Indicamos donde vamos a guardar el documento
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Path.Combine(WorkPaths.GetPdfPath(), "note_" + note.VoucherKey + ".pdf"), FileMode.Create));

            // tipos de letra
            var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
            var subTitleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
            var boldTableFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
            var TableFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);
            var endingMessageFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
            var bodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);

            doc.Open();

            // Adding meta info 
            doc.AddTitle("Documento Electrónico TicoPay");
            doc.AddAuthor("TicoPay");

            PdfPTable encabezado = new PdfPTable(2);
            encabezado.DefaultCell.Border = 0;
            encabezado.WidthPercentage = 100;
            encabezado.SetWidths(new int[] { 10, 4 });

            PdfPCell cell11 = new PdfPCell();
            cell11.Border = 0;
            cell11.BorderWidth = 0;

            // Escribimos el encabezado de la compañia
            cell11.AddElement(new Paragraph(tenant.Name, subTitleFont));
            cell11.AddElement(new Paragraph(tenant.IdentificationType.ToString() + " " + tenant.IdentificationNumber, bodyFont));
            cell11.AddElement(new Paragraph(tenant.Barrio.NombreBarrio + ", " + tenant.Barrio.Distrito.NombreDistrito
                + ", " + tenant.Barrio.Distrito.Canton.NombreCanton + ", " + tenant.Barrio.Distrito.Canton.Provincia.NombreProvincia + ", " + tenant.Address, bodyFont));
            cell11.AddElement(new Paragraph("Telf: " + tenant.PhoneNumber + ", Fax: " + tenant.Fax, bodyFont));
            cell11.AddElement(new Paragraph("Email: " + tenant.Email, bodyFont));

            cell11.VerticalAlignment = Element.ALIGN_LEFT;

            PdfPCell cell12 = new PdfPCell();
            cell12.Border = 0;
            cell12.BorderWidth = 0;

            cell12.AddElement(Chunk.NEWLINE);
            cell12.AddElement(new Paragraph("Fecha: " + note.CreationTime.ToShortDateString(), bodyFont));

            if (note.NoteType == NoteType.Credito)
                cell12.AddElement(new Paragraph("#Nota de Crédito Electrónica: " + note.ConsecutiveNumber, boldTableFont));
            else
                cell12.AddElement(new Paragraph("#Nota de Débito Electrónica: " + note.ConsecutiveNumber, boldTableFont));

            cell12.VerticalAlignment = Element.ALIGN_RIGHT;


            encabezado.AddCell(cell11);
            encabezado.AddCell(cell12);

            doc.Add(encabezado);

            // informacion del cliente
            var orderInfoTable = new PdfPTable(2);
            orderInfoTable.HorizontalAlignment = 0;
            orderInfoTable.SpacingBefore = 40;
            orderInfoTable.SpacingAfter = 10;
            orderInfoTable.DefaultCell.Border = 0;
            orderInfoTable.SetWidths(new int[] { 1, 4 });

            var tituloid = string.Format("Identificación{0}: ", (client.IdentificationType == IdentificacionTypeTipo.NoAsiganda ? " Extranjera" : string.Empty));
            var identificacion = (client.IdentificationType == IdentificacionTypeTipo.NoAsiganda ? client.IdentificacionExtranjero : client.IdentificationTypeToString() + " " + client.Identification);


            orderInfoTable.AddCell(new Phrase(tituloid, boldTableFont));
            orderInfoTable.AddCell(identificacion);
            orderInfoTable.AddCell(new Phrase("Código Cliente:", boldTableFont));
            orderInfoTable.AddCell(client.Code.ToString());
            orderInfoTable.AddCell(new Phrase("Cliente:", boldTableFont));
            orderInfoTable.AddCell(client.Name);
            orderInfoTable.AddCell(new Phrase("Dirección:", boldTableFont));
            orderInfoTable.AddCell(client.Barrio.NombreBarrio + ", " + client.Barrio.Distrito.NombreDistrito + ", " + client.Address);
            orderInfoTable.AddCell(new Phrase("Email:", boldTableFont));
            orderInfoTable.AddCell(client.Email);
            orderInfoTable.AddCell(new Phrase("Móvil:", boldTableFont));
            orderInfoTable.AddCell(client.MobilNumber);
            orderInfoTable.AddCell(new Phrase("Teléfono:", boldTableFont));
            orderInfoTable.AddCell(client.PhoneNumber);

            doc.Add(orderInfoTable);
            doc.Add(Chunk.NEWLINE);

            //Datos Factura
            PdfPTable tblPrueba = new PdfPTable(5);
            tblPrueba.WidthPercentage = 100;
            tblPrueba.SetWidths(new int[] { 7, 1, 2, 2, 2 });
            tblPrueba.SpacingBefore = 10f;
            tblPrueba.SpacingAfter = 10f;

            // Configuramos el título de las columnas de la tabla
            PdfPCell clNombre = new PdfPCell(new Phrase("Descripción", boldTableFont));
            clNombre.BorderWidth = 0;
            clNombre.BorderWidthBottom = 0.75f;
            clNombre.Padding = 5;
            clNombre.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);

            PdfPCell clcant = new PdfPCell(new Phrase("Cant.", boldTableFont));
            clcant.BorderWidth = 0;
            clcant.BorderWidthBottom = 0.75f;
            clcant.Padding = 5;
            clcant.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);

            PdfPCell clPrecio = new PdfPCell(new Phrase("Precio", boldTableFont));
            clPrecio.BorderWidth = 0;
            clPrecio.Padding = 5;
            clPrecio.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
            clPrecio.BorderWidthBottom = 0.75f;

            PdfPCell clImp = new PdfPCell(new Phrase("Impuesto", boldTableFont));
            clImp.BorderWidth = 0;
            clImp.Padding = 5;
            clImp.BorderWidthBottom = 0.75f;
            clImp.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);

            PdfPCell clTotal = new PdfPCell(new Phrase("Total", boldTableFont));
            clTotal.BorderWidth = 0;
            clTotal.Padding = 5;
            clTotal.BorderWidthBottom = 0.75f;
            clTotal.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);

            // Añadimos las celdas a la tabla
            tblPrueba.AddCell(clNombre);
            tblPrueba.AddCell(clcant);
            tblPrueba.AddCell(clPrecio);
            tblPrueba.AddCell(clImp);
            tblPrueba.AddCell(clTotal);

            // Llenamos la tabla con información
            if (note.NoteReasons == NoteReason.Reversa_documento)
                clNombre = new PdfPCell(new Phrase("Reversar Nota de Crédito #" + noteoriginal.ConsecutiveNumber, TableFont));
            else
                clNombre = new PdfPCell(new Phrase("Corregir Monto a Factura #" + noteoriginal.ConsecutiveNumber, TableFont));

            clNombre.BorderWidth = 0;
            clNombre.Padding = 5;

            clcant = new PdfPCell(new Phrase("1", TableFont));
            clcant.BorderWidth = 0;
            clcant.Padding = 5;

            clPrecio = new PdfPCell(new Phrase(note.Amount.ToString("N", new CultureInfo("en-US")), TableFont));
            clPrecio.BorderWidth = 0;
            clPrecio.Padding = 5;

            clImp = new PdfPCell(new Phrase(note.TaxAmount.ToString("N", new CultureInfo("en-US")), TableFont));
            clImp.BorderWidth = 0;
            clImp.Padding = 5;

            clTotal = new PdfPCell(new Phrase(note.Total.ToString("N", new CultureInfo("en-US")), TableFont));
            clTotal.BorderWidth = 0;
            clTotal.Padding = 5;

            // Añadimos las celdas a la tabla
            tblPrueba.AddCell(clNombre);
            tblPrueba.AddCell(clcant);
            tblPrueba.AddCell(clPrecio);
            tblPrueba.AddCell(clImp);
            tblPrueba.AddCell(clTotal);


            // totales
            //Total gravado
            tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
            tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

            clImp = new PdfPCell(new Phrase("Total Gravado: ", boldTableFont));
            clImp.BorderWidth = 0;
            clImp.Colspan = 2;
            clImp.Padding = 5;

            decimal totalgravado = 0;
            if (noteoriginal.TaxAmount != 0)
                totalgravado = note.Amount;

            clTotal = new PdfPCell(new Phrase(totalgravado.ToString("N", new CultureInfo("en-US")), TableFont));
            clTotal.BorderWidth = 0;
            clTotal.Padding = 5;

            tblPrueba.AddCell(clImp);
            tblPrueba.AddCell(clTotal);

            //total Exento
            tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
            tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

            clImp = new PdfPCell(new Phrase("Total Exento: ", boldTableFont));
            clImp.BorderWidth = 0;
            clImp.Colspan = 2;
            clImp.Padding = 5;

            decimal totalexento = 0;
            if (noteoriginal.TaxAmount == 0)
                totalexento = note.Amount;

            clTotal = new PdfPCell(new Phrase(totalexento.ToString("N", new CultureInfo("en-US")), TableFont));
            clTotal.BorderWidth = 0;
            clTotal.Padding = 5;

            tblPrueba.AddCell(clImp);
            tblPrueba.AddCell(clTotal);

            //Total impuesto
            tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
            tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

            clImp = new PdfPCell(new Phrase("Total Impuesto: ", boldTableFont));
            clImp.BorderWidth = 0;
            clImp.Colspan = 2;
            clImp.Padding = 5;

            clTotal = new PdfPCell(new Phrase(note.TaxAmount.ToString("N", new CultureInfo("en-US")), TableFont));
            clTotal.BorderWidth = 0;
            clTotal.Padding = 5;

            tblPrueba.AddCell(clImp);
            tblPrueba.AddCell(clTotal);

            //Total Factura
            tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
            tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

            clImp = new PdfPCell(new Phrase("Total Nota: ", boldTableFont));
            clImp.BorderWidth = 0;
            clImp.Colspan = 2;
            clImp.Padding = 5;

            clTotal = new PdfPCell(new Phrase(note.Total.ToString("N", new CultureInfo("en-US")), TableFont));
            clTotal.BorderWidth = 0;
            clTotal.Padding = 5;

            tblPrueba.AddCell(clImp);
            tblPrueba.AddCell(clTotal);

            doc.Add(tblPrueba);

            doc.Add(Chunk.NEWLINE);

            // inserta el codigo QR y una nota
            //doc.Add(new Paragraph("* Consulte sus recibos pendientes en ticopayproduccion.cloudapp.net/ConsultaRecibos", endingMessageFont));

            note.byteArrayToImage(note.QRCode, note.VoucherKey);
            Image qr = Image.GetInstance(Path.Combine(WorkPaths.GetQRPath(), "note_" + note.VoucherKey + ".png"));
            qr.BorderWidth = 0;
            qr.Alignment = Element.ALIGN_CENTER;

            doc.Add(qr);

            doc.Close();
        }

        private PdfPTable BuildHeaderNote(Note note, Tenant tenant)
        {
            //----------------------------------------------------------------------------------------//
            //                                                        Factura #: 00100001010000000080 //
            //----------------------------------------------------------------------------------------//
            //               |  TENANT NOMBRE COMERCIAL                                               //
            //               |  Razón Social: XXXXXXXXXXXXX                                          //
            //               |  Cedula <tipo>: XXXXXXXXXXXXX                                          //
            //     Logo      |  Telef.: 123-00000000, Fax: 123-000000000                              //
            //               |  Email: cuenta@servidor.com                                            //
            //               |  Dirección:                                                            //
            //----------------------------------------------------------------------------------------//
            //                                                           Fecha de emision: 02/11/2017 //
            //                                                       Tipo Documento Referido: Factura //
            //                                                            Documento Referido: Pagada  //
            //                                                                        Moneda: USD      //
            //                                                            Condición de venta: Contado //
            //                                                                 Medio de pago: Efectivo //
            //                                                                Estado factura: Pagada  //

            //----------------------------------------------------------------------------------------//

            PdfPTable header = new PdfPTable(2);
            header.DefaultCell.Border = 0;
            header.WidthPercentage = 100;
            header.SetTotalWidth(new float[] { 15, 100 });

            int tenantDataColspan = 2;
            string invoiceNumber = tenant.IdentificationType.ToString() + " " + tenant.IdentificationNumber.ToString();
            if (note.NoteType == NoteType.Credito)
                header.AddCell(CreateTextPdfPCell($"Nota de Crédito Electrónica #: {note.ConsecutiveNumber}", SubTitleFont, 2, 2));
            else
                header.AddCell(CreateTextPdfPCell($"Nota de Débito Electrónica #: {note.ConsecutiveNumber}", SubTitleFont, 2, 2));
            header.AddCell(CreateTextPdfPCell($"Clave Numérica #: {note.VoucherKey}", SubTitleFont, 2, 2));
            header.AddCell(CreateEmptyLine(2));

            if (tenant.Logo != null && tenant.Logo.Count() > 0)
            {
                var logo = Image.GetInstance(System.Drawing.Image.FromStream(new MemoryStream(tenant.Logo)), ImageFormat.Png);
                logo.Border = Rectangle.BOX;
                logo.BorderColor = BaseColor.WHITE;
                logo.BorderWidth = 5f;
                header.AddCell(logo);
                tenantDataColspan = 1;
            }

            PdfPTable tenantData = new PdfPTable(1);
            tenantData.AddCell(CreateTextPdfPCell(tenant.BussinesName, TitleFont));
            tenantData.AddCell(CreateLabeledTextPdfPCell("Razón Social:", tenant.Name, SubTitleFont, BodyFont));
            tenantData.AddCell(CreateLabeledTextPdfPCell($"{tenant.IdentificationTypeToString()}:", tenant.IdentificationNumber, SubTitleFont, BodyFont));
            tenantData.AddCell(CreateLabeledTextPdfPCell("Teléf.:", $"{tenant.PhoneNumber}, Fax: {tenant.Fax}", SubTitleFont, BodyFont));
            tenantData.AddCell(CreateLabeledTextPdfPCell("Email:", tenant.Email, SubTitleFont, BodyFont));
            if (tenant.IsAddressShort)
                tenantData.AddCell(CreateLabeledTextPdfPCell("Dirección:", $"{tenant.AddressShort}", SubTitleFont, BodyFont));
            else
                tenantData.AddCell(CreateLabeledTextPdfPCell("Dirección:", $"{tenant.Barrio.NombreBarrio}, {tenant.Barrio.Distrito.NombreDistrito}, {tenant.Barrio.Distrito.Canton.NombreCanton}, {tenant.Barrio.Distrito.Canton.Provincia.NombreProvincia}, {tenant.Address}", SubTitleFont, BodyFont));
            header.AddCell(new PdfPCell(tenantData) { BorderWidth = 0, Colspan = tenantDataColspan, PaddingLeft = 5f });
            header.AddCell(CreateEmptyLine(2));
            header.AddCell(CreateLabeledTextPdfPCell("Fecha de emisión:", note.CreationTime.ToString("dd/MM/yyyy"), SubTitleFont, BodyFont, 2, 2));
            header.AddCell(CreateLabeledTextPdfPCell("Hora de emisión:", note.CreationTime.ToString("hh:mm:ss"), SubTitleFont, BodyFont, 2, 2));


            if (note.ConsecutiveNumberReference != null)
            {
                var tipo = "Nota de " + (note.NoteType == NoteType.Credito ? NoteType.Debito.GetDescription() : NoteType.Credito.GetDescription());
                header.AddCell(CreateLabeledTextPdfPCell("Tipo Documento Referido: ", tipo, SubTitleFont, BodyFont, 2, 2));
                header.AddCell(CreateLabeledTextPdfPCell("Documento Referido: ", note.ConsecutiveNumberReference, SubTitleFont, BodyFont, 2, 2));
            }
            else
            {
                header.AddCell(CreateLabeledTextPdfPCell("Tipo Documento Referido: ", note.Invoice.TypeDocument.GetDescription(), SubTitleFont, BodyFont, 2, 2));
                header.AddCell(CreateLabeledTextPdfPCell("Documento Referido: ", note.Invoice.ConsecutiveNumber, SubTitleFont, BodyFont, 2, 2));
            }
            header.AddCell(CreateLabeledTextPdfPCell("Moneda: ", note.CodigoMoneda.ToString(), SubTitleFont, BodyFont, 2, 2));
            return header;
        }
                
    }
}
