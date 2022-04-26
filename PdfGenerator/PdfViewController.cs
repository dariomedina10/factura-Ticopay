// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdfViewController.cs" company="SemanticArchitecture">
//   http://www.SemanticArchitecture.net pkalkie@gmail.com
// </copyright>
// <summary>
//   Extends the controller with functionality for rendering PDF views
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Mvc;

namespace PdfGenerator
{
    /// <summary>
    /// Extends the controller with functionality for rendering PDF views
    /// </summary>
    public class PdfViewController : Controller
    {
        private readonly HtmlViewRenderer _htmlViewRenderer;
        private readonly StandardPdfRenderer _standardPdfRenderer;

        public PdfViewController()
        {
            _htmlViewRenderer = new HtmlViewRenderer();
            _standardPdfRenderer = new StandardPdfRenderer();
        }

        protected ActionResult ViewPdf(string pageTitle, string linkCss, string viewName, object model, bool horizontal)
        {
            // Render the view html to a string.
            string htmlText = _htmlViewRenderer.RenderViewToString(this, viewName, model);
            htmlText = System.Text.RegularExpressions.Regex.Replace(htmlText, @"\s+", " ");
            htmlText = htmlText.Replace("\n", "").Replace("\r", "").Trim();

            // Let the html be rendered into a PDF document through iTextSharp.
            byte[] buffer = _standardPdfRenderer.Render(htmlText, pageTitle,linkCss, horizontal);

            // Return the PDF as a binary stream to the client.
            return new BinaryContentResult(buffer, "application/pdf");
        }

        //protected byte [] GeneratePdf(string pageTitle, string linkCss, string viewName, object model, bool horizontal)
        //{
        //    // Render the view html to a string.
        //    string htmlText = _htmlViewRenderer.RenderViewToString(this, viewName, model);
        //    htmlText = System.Text.RegularExpressions.Regex.Replace(htmlText, @"\s+", " ");
        //    htmlText = htmlText.Replace("\n", "").Replace("\r", "").Trim();

        //    // Let the html be rendered into a PDF document through iTextSharp.
        //    byte[] buffer = _standardPdfRenderer.Render(htmlText, pageTitle, linkCss, horizontal);

        //    return buffer;
        //}
    }
}