using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.Reports
{
    public class ConnectorPdfWatermarlWriter : IPdfPageEvent
    {
        private readonly byte[] _watermarkImage = null;
        private readonly Rectangle _watermarkRectange;

        public ConnectorPdfWatermarlWriter(byte[] watermarkImage, Rectangle watermarkRectange)
        {
            _watermarkImage = watermarkImage;
            _watermarkRectange = watermarkRectange;
        }

        public void OnOpenDocument(PdfWriter writer, Document document) { }

        public void OnCloseDocument(PdfWriter writer, Document document) { }

        public void OnStartPage(PdfWriter writer, Document document)
        {
            if (_watermarkImage == null || _watermarkImage.Length == 0)
            {
                return;
            }
            try
            {
                Image img = Image.GetInstance(ByteArrayToImage(_watermarkImage), System.Drawing.Imaging.ImageFormat.Jpeg);
                img.ScaleAbsolute(_watermarkRectange);
                img.SetAbsolutePosition(0, 0);

                PdfContentByte under = writer.DirectContentUnder;
                under.AddImage(img);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public void OnEndPage(PdfWriter writer, Document document) { }

        public void OnParagraph(PdfWriter writer, Document document, float paragraphPosition) { }

        public void OnParagraphEnd(PdfWriter writer, Document document, float paragraphPosition) { }

        public void OnChapter(PdfWriter writer, Document document, float paragraphPosition, Paragraph title) { }

        public void OnChapterEnd(PdfWriter writer, Document document, float paragraphPosition) { }

        public void OnSection(PdfWriter writer, Document document, float paragraphPosition, int depth, Paragraph title) { }

        public void OnSectionEnd(PdfWriter writer, Document document, float paragraphPosition) { }

        public void OnGenericTag(PdfWriter writer, Document document, Rectangle rect, String text) { }

        public System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }
    }
}
