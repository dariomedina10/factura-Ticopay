using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using TicoPay.ReportsSettings;
using TicopayUniversalConnectorService.Entities;

namespace TicopayUniversalConnectorService.Reports
{
    public class PdfReportGenerator
    {
        // tipos de letra
        private readonly Font TitleFont;
        private readonly Font SubTitleFont;
        private readonly Font TableTitleFont;
        private readonly Font TableFont;
        private readonly Font InfoMessageFont;
        private readonly Font BodyFont;
        private readonly ConnectorPdfSettingsHelper _reportSettingsHelper;

        public PdfReportGenerator()
        {
            ConnectorReportSettings reportSettings = new ConnectorReportSettings();
            _reportSettingsHelper = ConnectorPdfSettingsHelper.Create(reportSettings);

            TitleFont = _reportSettingsHelper.GetTitleFont();
            SubTitleFont = _reportSettingsHelper.GetSubTitleFont();
            TableTitleFont = _reportSettingsHelper.GetTableTitleFont();
            TableFont = _reportSettingsHelper.GetTableFont();
            InfoMessageFont = _reportSettingsHelper.GetInfoMessageFont();
            BodyFont = _reportSettingsHelper.GetBodyFont();
        }

        public void GenerateOperationsSincronizedReport(List<Operacion> Operaciones, Configuracion Conector,string FileLocation)
        {
            Document doc = _reportSettingsHelper.CreateDocument();            
            try
            {
                // Indicamos donde vamos a guardar el documento
                PdfWriter writer = PdfWriter.GetInstance(doc,
                           new FileStream(FileLocation, FileMode.Create));
                writer.PageEvent = _reportSettingsHelper.GetPageEventHandler();
                doc.Open();

                // Adding meta info 
                _reportSettingsHelper.Settings.ReportTitle = "Reporte de Operaciones Procesadas";
                doc.AddTitle(_reportSettingsHelper.Settings.ReportTitle);
                doc.AddAuthor("Conector TicoPay");
                doc.Add(BuildHeader(Conector, "Reporte de Operaciones Procesadas por el Conector de Ticopay"));
                doc.Add(Chunk.NEWLINE);

                //Datos Factura
                PdfPTable tblPrueba = new PdfPTable(6);
                tblPrueba.WidthPercentage = 100;
                tblPrueba.SetWidths(new int[] { 2, 2, 3, 4, 5, 2 });
                tblPrueba.SpacingBefore = 10f;
                tblPrueba.SpacingAfter = 10f;

                // Configuramos el título de las columnas de la tabla
                PdfPCell clTipoOperacion = new PdfPCell(new Phrase("Tipo Ope", TableTitleFont));
                clTipoOperacion.BorderWidth = 0;
                clTipoOperacion.BorderWidthBottom = 0.75f;
                clTipoOperacion.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clTipoOperacion.Padding = 5;


                PdfPCell clEstatusEnvio = new PdfPCell(new Phrase("Estatus envío", TableTitleFont));
                clEstatusEnvio.BorderWidth = 0;
                clEstatusEnvio.BorderWidthBottom = 0.75f;
                clEstatusEnvio.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clEstatusEnvio.Padding = 5;

                PdfPCell clNumeroControl = new PdfPCell(new Phrase("Numero Control Op", TableTitleFont));
                clNumeroControl.BorderWidth = 0;
                clNumeroControl.BorderWidthBottom = 0.75f;
                clNumeroControl.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clNumeroControl.Padding = 5;
                clNumeroControl.HorizontalAlignment = 2;

                PdfPCell clConsecutivoTicopay = new PdfPCell(new Phrase("Consecutivo en Ticopay", TableTitleFont));
                clConsecutivoTicopay.BorderWidth = 0;
                clConsecutivoTicopay.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clConsecutivoTicopay.BorderWidthBottom = 0.75f;
                clConsecutivoTicopay.Padding = 5;
                clConsecutivoTicopay.HorizontalAlignment = 2;

                PdfPCell clVoucher = new PdfPCell(new Phrase("Voucher key", TableTitleFont));
                clVoucher.BorderWidth = 0;
                clVoucher.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clVoucher.BorderWidthBottom = 0.75f;
                clVoucher.Padding = 5;
                clVoucher.HorizontalAlignment = 2;

                PdfPCell clFechaEnvio = new PdfPCell(new Phrase("Fecha/Hora Envió", TableTitleFont));
                clFechaEnvio.BorderWidth = 0;
                clFechaEnvio.BorderWidthBottom = 0.75f;
                clFechaEnvio.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clFechaEnvio.Padding = 5;
                clFechaEnvio.HorizontalAlignment = 2;                

                // Añadimos las celdas a la tabla
                tblPrueba.AddCell(clTipoOperacion);
                tblPrueba.AddCell(clEstatusEnvio);
                tblPrueba.AddCell(clNumeroControl);
                tblPrueba.AddCell(clConsecutivoTicopay);
                tblPrueba.AddCell(clVoucher);
                tblPrueba.AddCell(clFechaEnvio);

                // Llenamos la tabla con información
                foreach (Operacion line in Operaciones.OrderBy(x => x.TipoDeOperacion))
                {
                    clTipoOperacion = new PdfPCell(new Phrase(line.TipoDeOperacion.ToString(), TableFont));
                    clTipoOperacion.BorderWidth = 0;
                    clTipoOperacion.Padding = 5;

                    clEstatusEnvio = new PdfPCell(new Phrase(line.EstadoOperacion.ToString(), TableFont));
                    clEstatusEnvio.BorderWidth = 0;
                    clEstatusEnvio.Padding = 5;
                                        
                    clNumeroControl = new PdfPCell(new Phrase(line.ConsecutivoConector, TableFont));
                    clNumeroControl.BorderWidth = 0;
                    clNumeroControl.Padding = 5;
                    clNumeroControl.HorizontalAlignment = 2;

                    if (line.EstadoOperacion == Estado.Procesado)
                    {
                        clConsecutivoTicopay = new PdfPCell(new Phrase(line.ConsecutivoTicopay, TableFont));
                    }                    
                    else
                    {
                        clConsecutivoTicopay = new PdfPCell(new Phrase("N/A", TableFont));
                    }                        
                    clConsecutivoTicopay.BorderWidth = 0;
                    clConsecutivoTicopay.Padding = 5;
                    clConsecutivoTicopay.HorizontalAlignment = 2;


                    if (line.EstadoOperacion == Estado.Procesado)
                    {
                        clVoucher = new PdfPCell(new Phrase(line.VoucherTicopay, TableFont));
                    }
                    else
                    {
                        clVoucher = new PdfPCell(new Phrase("N/A", TableFont));                        
                    }
                    clVoucher.BorderWidth = 0;
                    clVoucher.Padding = 5;
                    clVoucher.HorizontalAlignment = 2;

                    if (line.EstadoOperacion != Estado.NoProcesado)
                    {
                        clFechaEnvio = new PdfPCell(new Phrase(line.EnviadoEl.ToString("dd/MM/yyyy HH:mm"), TableFont));
                    }
                    else
                    {
                        clFechaEnvio = new PdfPCell(new Phrase("N/A", TableFont));
                    }                        
                    clFechaEnvio.BorderWidth = 0;
                    clFechaEnvio.Padding = 5;
                    clFechaEnvio.HorizontalAlignment = 2;

                    // Añadimos las celdas a la tabla
                    tblPrueba.AddCell(clTipoOperacion);
                    tblPrueba.AddCell(clEstatusEnvio);
                    tblPrueba.AddCell(clNumeroControl);
                    tblPrueba.AddCell(clConsecutivoTicopay);
                    tblPrueba.AddCell(clVoucher);
                    tblPrueba.AddCell(clFechaEnvio);
                }

                // totales
                //Total gravado
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

                clTipoOperacion = new PdfPCell(new Phrase("Total de Operaciones: " + Operaciones.Count.ToString(), TableTitleFont));
                clTipoOperacion.BorderWidth = 0;
                clTipoOperacion.Colspan = 2;
                clTipoOperacion.Padding = 5;
                clTipoOperacion.HorizontalAlignment = 2;

                tblPrueba.AddCell(clTipoOperacion);                

                doc.Add(tblPrueba);

                doc.Add(Chunk.NEWLINE);

                doc.Add(BuildFooter());
            }
            catch (Exception ex)
            {
            }
            finally
            {
                doc.Close();
            }
        }

        public void GenerateOperationsErrorReport(List<Operacion> Operaciones, Configuracion Conector, string FileLocation)
        {
            Document doc = _reportSettingsHelper.CreateDocument();
            try
            {
                // Indicamos donde vamos a guardar el documento
                PdfWriter writer = PdfWriter.GetInstance(doc,
                           new FileStream(FileLocation, FileMode.Create));
                writer.PageEvent = _reportSettingsHelper.GetPageEventHandler();
                doc.Open();

                // Adding meta info 
                _reportSettingsHelper.Settings.ReportTitle = "Reporte de Operaciones con Error";
                doc.AddTitle(_reportSettingsHelper.Settings.ReportTitle);
                doc.AddAuthor("Conector TicoPays");
                doc.Add(BuildHeader(Conector, "Reporte de Operaciones con estado de Error del Conector de Ticopays"));
                doc.Add(Chunk.NEWLINE);

                //Datos Factura
                PdfPTable tblPrueba = new PdfPTable(4);
                tblPrueba.WidthPercentage = 100;
                tblPrueba.SetWidths(new int[] { 2,2,2,12 });
                tblPrueba.SpacingBefore = 10f;
                tblPrueba.SpacingAfter = 10f;

                // Configuramos el título de las columnas de la tabla
                PdfPCell clTipoOperacion = new PdfPCell(new Phrase("Tipo Ope", TableTitleFont));
                clTipoOperacion.BorderWidth = 0;
                clTipoOperacion.BorderWidthBottom = 0.75f;
                clTipoOperacion.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clTipoOperacion.Padding = 5;                

                PdfPCell clNumeroControl = new PdfPCell(new Phrase("Numero Control Op", TableTitleFont));
                clNumeroControl.BorderWidth = 0;
                clNumeroControl.BorderWidthBottom = 0.75f;
                clNumeroControl.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clNumeroControl.Padding = 5;
                clNumeroControl.HorizontalAlignment = 2;
                
                PdfPCell clFechaEnvio = new PdfPCell(new Phrase("Fecha/Hora Envió", TableTitleFont));
                clFechaEnvio.BorderWidth = 0;
                clFechaEnvio.BorderWidthBottom = 0.75f;
                clFechaEnvio.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clFechaEnvio.Padding = 5;
                clFechaEnvio.HorizontalAlignment = 2;

                PdfPCell clError = new PdfPCell(new Phrase("Error", TableTitleFont));
                clError.BorderWidth = 0;
                clError.BorderWidthBottom = 0.75f;
                clError.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                clError.Padding = 5;
                clError.HorizontalAlignment = 2;

                // Añadimos las celdas a la tabla
                tblPrueba.AddCell(clTipoOperacion);
                tblPrueba.AddCell(clNumeroControl);
                tblPrueba.AddCell(clFechaEnvio);
                tblPrueba.AddCell(clError);

                // Llenamos la tabla con información
                foreach (Operacion line in Operaciones.OrderBy(x => x.TipoDeOperacion))
                {
                    clTipoOperacion = new PdfPCell(new Phrase(line.TipoDeOperacion.ToString(), TableFont));
                    clTipoOperacion.BorderWidth = 0;
                    clTipoOperacion.Padding = 5;
                                        
                    clNumeroControl = new PdfPCell(new Phrase(line.ConsecutivoConector, TableFont));
                    clNumeroControl.BorderWidth = 0;
                    clNumeroControl.Padding = 5;
                    clNumeroControl.HorizontalAlignment = 2;
                                        
                    clFechaEnvio = new PdfPCell(new Phrase(line.EnviadoEl.ToString("dd/MM/yyyy HH:mm"), TableFont));
                    clFechaEnvio.BorderWidth = 0;
                    clFechaEnvio.Padding = 5;
                    clFechaEnvio.HorizontalAlignment = 2;

                    clError = new PdfPCell(new Phrase(line.Errores, TableFont));
                    clError.BorderWidth = 0;
                    clError.Padding = 5;
                    clError.HorizontalAlignment = 2;

                    // Añadimos las celdas a la tabla
                    tblPrueba.AddCell(clTipoOperacion);
                    tblPrueba.AddCell(clNumeroControl);
                    tblPrueba.AddCell(clFechaEnvio);
                    tblPrueba.AddCell(clError);
                }

                // totales
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });
                tblPrueba.AddCell(new PdfPCell(new Phrase()) { BorderWidth = 0, Padding = 5 });

                clTipoOperacion = new PdfPCell(new Phrase("Total de Operaciones Fallidas: " + Operaciones.Count.ToString(), TableTitleFont));
                clTipoOperacion.BorderWidth = 0;
                clTipoOperacion.Colspan = 2;
                clTipoOperacion.Padding = 5;
                clTipoOperacion.HorizontalAlignment = 2;

                tblPrueba.AddCell(clTipoOperacion);

                doc.Add(tblPrueba);

                doc.Add(Chunk.NEWLINE);

                doc.Add(BuildFooter());
            }
            catch (Exception ex)
            {
            }
            finally
            {
                doc.Close();
            }
        }

        private PdfPTable BuildHeader(Configuracion job, string title)
        {
            //----------------------------------------------------------------------------------------//
            //                                               Reporte de Documentos enviados a Ticopay //
            //                          Tipo de Conector : Contapyme / Tenant de Ticopay : Quickbooks //   
            //----------------------------------------------------------------------------------------//
            //                                                           Fecha de emision: 02/11/2017 //
            //----------------------------------------------------------------------------------------//

            PdfPTable header = new PdfPTable(2);
            header.DefaultCell.Border = 0;
            header.WidthPercentage = 100;
            header.SetTotalWidth(new float[] { 15, 100 });
            header.AddCell(CreateTextPdfPCell(title, SubTitleFont, 2, 2));
            header.AddCell(CreateTextPdfPCell("Tipo de Conector : " + job.TipoConector.ToString() + " / Tenant de Ticopay : " + job.SubDominioTicopay, SubTitleFont, 2, 2));
            header.AddCell(CreateEmptyLine(2));
            
            header.AddCell(CreateLabeledTextPdfPCell("Fecha de emisión:", DateTime.Now.ToString("dd/MM/yyyy HH:mm"), SubTitleFont, BodyFont, 2, 2));            

            return header;
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
            footer.AddCell(CreateTextPdfPCell("Conector TicoPay", InfoMessageFont, 2, 1));
            footer.AddCell(CreateTextPdfPCell("https://ticopays.com", InfoMessageFont, 2, 1));
            footer.AddCell(CreateTextPdfPCell("email: soporte @ticopays.com", InfoMessageFont, 1, 0));
            footer.AddCell(CreateTextPdfPCell("telf.: 123-87046985", InfoMessageFont, 1, 2));

            return footer;
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
    }
}
