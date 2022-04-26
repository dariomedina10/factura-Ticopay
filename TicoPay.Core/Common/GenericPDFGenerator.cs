using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Common
{
    public class GenericPDFGenerator
    {
        private readonly Font TitleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
        private readonly Font SubTitleFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
        private readonly Font BoldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
        private readonly Font TableFont = FontFactory.GetFont("Arial", 7, Font.NORMAL);
        private readonly Font EndingMessageFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
        private readonly Font BodyFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);

        public NumberFormatInfo NumberFormatInfo { get; set; }
        public int DecimalDigits { get; set; }
        public string DateTimeFormat { get; set; }
        public string OnDateNullValue { get; set; }

        public GenericPDFGenerator()
        {
            DecimalDigits = 2;
            NumberFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = ".",
                NumberGroupSeparator = ",",
                NumberDecimalDigits = DecimalDigits
            };
            DateTimeFormat = "dd/MM/yyyy";
            OnDateNullValue = "-";
        }

        public Stream CreateGenericReportList<T>(string reportTitle, List<T> items, bool addRowHeaderColumn = false)
        {
            MemoryStream fileStream = new MemoryStream();
            Document doc = new Document(PageSize.LETTER);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, fileStream);
                if (fileStream is MemoryStream)
                {
                    writer.CloseStream = false;
                }
                doc.Open();

                doc.AddTitle(reportTitle);
                doc.AddAuthor("TicoPay");
                doc.Add(BuildGenericListHeader(reportTitle));
                doc.Add(Chunk.NEWLINE);

                var columns = GetColumnsFromType<T>();

                PdfPTable pdfPTable = new PdfPTable(columns.Length);
                pdfPTable.WidthPercentage = 100;
                //pdfPTable.SetWidths(new int[] { 1, 5, 2, 3, 2, 2, 3 });
                pdfPTable.SpacingBefore = 10f;
                pdfPTable.SpacingAfter = 10f;

                if (addRowHeaderColumn)
                {
                    PdfPCell pdfPCell = new PdfPCell(new Phrase("#", BoldTableFont));
                    pdfPCell.BorderWidth = 0;
                    pdfPCell.BorderWidthBottom = 0.75f;
                    pdfPCell.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                    pdfPCell.Padding = 5;
                    pdfPTable.AddCell(pdfPCell);
                }

                foreach (var column in columns)
                {
                    string description = GetPropertyDescription(column);
                    PdfPCell pdfPCell = new PdfPCell(new Phrase(description, BoldTableFont));
                    pdfPCell.BorderWidth = 0;
                    pdfPCell.BorderWidthBottom = 0.75f;
                    pdfPCell.BackgroundColor = new BaseColor(System.Drawing.Color.Silver);
                    pdfPCell.Padding = 5;
                    pdfPTable.AddCell(pdfPCell);
                }

                foreach (var item in items)
                {
                    foreach (var column in columns)
                    {
                        PdfPCell pdfPCell = null;
                        if (column.PropertyType == typeof(decimal) || column.PropertyType == typeof(float) || column.PropertyType == typeof(double))
                        {
                            decimal value = GetPropertyValue<decimal>(column, item);
                            pdfPCell = CreateTextPdfPCell(value.ToString("N" + DecimalDigits, NumberFormatInfo), TableFont, textAlignment: 2, padding: 5);
                        }
                        else if (column.PropertyType == typeof(int))
                        {
                            int value = GetPropertyValue<int>(column, item);
                            pdfPCell = CreateTextPdfPCell(value.ToString("N"), TableFont, textAlignment: 2, padding: 5);
                        }
                        else if (column.PropertyType == typeof(bool))
                        {
                            bool value = GetPropertyValue<bool>(column, item);
                            pdfPCell = CreateTextPdfPCell(value ? "Si" : "No", TableFont, padding: 5);
                        }
                        else if (column.PropertyType == typeof(DateTime))
                        {
                            DateTime value = GetPropertyValue<DateTime>(column, item);
                            string dateValue = value.Year == 1 ? OnDateNullValue : value.ToString(DateTimeFormat);
                            pdfPCell = CreateTextPdfPCell(dateValue, TableFont, padding: 5);
                        }
                        else if (column.PropertyType.IsEnum)
                        {
                            string value = GetPropertyValue<string>(column, item);
                            string enumDescription = GetEnumDescription(column, value);
                            pdfPCell = CreateTextPdfPCell(enumDescription, TableFont, padding: 5);
                        }
                        else
                        {
                            string value = GetPropertyValue<string>(column, item);
                            pdfPCell = CreateTextPdfPCell(value, TableFont, padding: 5);
                        }
                        if (pdfPCell != null)
                        {
                            pdfPTable.AddCell(pdfPCell);
                        }
                    }
                }

                doc.Add(pdfPTable);
            }
            finally
            {
                doc.Close();
            }
            return fileStream;
        }

        private PdfPTable BuildGenericListHeader(string headerText)
        {
            //----------------------------------------------------------------------------------------//
            //                                NOMBRE DEL REPORTE                                      //
            //                           fecha de impresión: ##/##/####                               //
            //----------------------------------------------------------------------------------------//
            PdfPTable header = new PdfPTable(1);
            header.DefaultCell.Border = 0;
            header.WidthPercentage = 100;
            header.SetTotalWidth(new float[] { 100 });

            header.AddCell(CreateTextPdfPCell(headerText, TitleFont));
            header.AddCell(CreateTextPdfPCell($"Fecha de emisión: {DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")}", BodyFont, 2, 2));

            return header;
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

        private PropertyInfo[] GetColumnsFromType<T>()
        {
            PropertyInfo[] properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .ToArray();
            return properties;
        }

        private string GetPropertyDescription(PropertyInfo propertyInfo)
        {
            string description = string.Empty;
            if (propertyInfo != null)
            {
                description = Attribute.IsDefined(propertyInfo, typeof(DescriptionAttribute)) ?
                    (Attribute.GetCustomAttribute(propertyInfo, typeof(DescriptionAttribute)) as DescriptionAttribute).Description :
                    propertyInfo.Name;
            }
            return description;
        }

        private T GetPropertyValue<T>(PropertyInfo propertyInfo, object item)
        {
            T result = default(T);
            try
            {
                object objValue = propertyInfo.GetValue(item);
                result = (T)Convert.ChangeType(objValue, typeof(T));
            }
            catch (Exception)
            {
                //NADA: si no logra convertir el tipo debe retornar el valor por defecto.
            }
            return result;
        }

        private string GetEnumDescription(PropertyInfo fieldInfo, string enumValue)
        {
            string description = string.Empty;
            if (fieldInfo != null)
            {
                var enumField = fieldInfo.PropertyType.GetField(enumValue);
                if (enumField != null)
                {
                    DescriptionAttribute[] attributes = (DescriptionAttribute[])enumField.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attributes.Length > 0)
                    {
                        description = attributes[0].Description;
                    }
                    else
                    {
                        description = enumField.Name.ToString();
                    }
                }
            }
            return description;
        }
    }
}
