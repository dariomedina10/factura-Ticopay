// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandardPdfRenderer.cs" company="SemanticArchitecture">
//   http://www.SemanticArchitecture.net
// </copyright>
// <summary>
//   This class is responsible for rendering a html text string to a PDF document
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;

namespace PdfGenerator
{
    /// <summary>
    /// This class is responsible for rendering a html text string to a PDF document using the html renderer of iTextSharp.
    /// </summary>
    public class StandardPdfRenderer
    {
        private const int HorizontalMargin = 10;
        private const int VerticalMargin = 10;

        public byte[] Render(string htmlText, string pageTitle, string linkCss, bool horizontal)
        {
            byte[] renderedBuffer;
            using (var outputMemoryStream = new MemoryStream())
            {
                using (var pdfDocument = new Document(PageSize.A4, HorizontalMargin, HorizontalMargin, VerticalMargin, VerticalMargin))
                {
                    if (horizontal)
                        pdfDocument.SetPageSize(PageSize.A4.Rotate());
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDocument, outputMemoryStream);
                    pdfWriter.CloseStream = false;
                    pdfWriter.PageEvent = new PrintHeaderFooter { Title = pageTitle };
                    pdfDocument.Open();

                    // register all fonts in current computer
                    FontFactory.RegisterDirectories();

                    // Set factories
                    var htmlContext = new HtmlPipelineContext(null);
                    htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
                    // Set css
                    ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
                    cssResolver.AddCssFile(HttpContext.Current.Server.MapPath(linkCss), true);
                    cssResolver.AddCss(".shadow {background-color: #ebdddd; }", true);

                    //Export
                    IPipeline pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(pdfDocument, pdfWriter)));

                    using (var xmlString = new StringReader(htmlText))
                    {
                        var worker = new XMLWorker(pipeline, true);
                        var xmlParse = new XMLParser(true, worker);
                        xmlParse.Parse(xmlString);
                        xmlParse.Flush();
                    }
                }

                renderedBuffer = new byte[outputMemoryStream.Position];
                outputMemoryStream.Position = 0;
                outputMemoryStream.Read(renderedBuffer, 0, renderedBuffer.Length);
            }
            return renderedBuffer;
        }
    }
}