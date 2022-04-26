using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace TicoPay.ReportsSettings.Dto
{
    [AutoMapFrom(typeof(ReportSettings)), AutoMapTo(typeof(ReportSettings))]
    public class ReportSettingsDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }

        [Display(Name = "Nombre del reporte")]
        public string ReportName { get; set; }

        [Display(Name = "Título del reporte")]
        public string ReportTitle { get; set; }

        [Display(Name = "Mostrar fecha de impresión")]
        public bool ShowPrintDate { get; set; }

        [Display(Name = "Mostrar número de página")]
        public bool ShowPageNumber { get; set; }

        #region Watermark
        [Display(Name = "Usa marca de agua")]
        public bool UseWatermark { get; set; }

        [Display(Name = "Marca de agua")]
        public byte[] WatermarkImage { get; set; }

        [Display(Name = "Posición de la marca de agua")]
        public WatermarkPosition WatermarkPosition { get; set; }

        public string WatermarkBase64String { get; set; }

        public HttpPostedFileBase WatermarkFile { get; set; }

        public bool DeleteLogo { get; set; }
        #endregion

        #region Page Size
        public bool IsCustomSize { get; set; }
        public string PageSizeName { get; set; }
        public float LowerLeftX { get; set; }
        public float LowerLeftY { get; set; }
        public float UpperRightX { get; set; }
        public float UpperRightY { get; set; }
        #endregion

        #region Page Margin
        public float MarginLeft { get; set; }
        public float MarginRight { get; set; }
        public float MarginTop { get; set; }
        public float MarginBottom { get; set; }
        #endregion

        #region Fonts
        [Display(Name = "Letra")]
        public string TitleFontName { get; set; }

        [Display(Name = "Letra")]
        public string SubTitleFontName { get; set; }

        [Display(Name = "Letra")]
        public string TableTitleFontName { get; set; }

        [Display(Name = "Letra")]
        public string TableFontName { get; set; }

        [Display(Name = "Letra")]
        public string BodyFontName { get; set; }

        [Display(Name = "Letra")]
        public string InfoMessageFontName { get; set; }

        [Display(Name = "Tamaño")]
        public int TitleFontSize { get; set; }

        [Display(Name = "Tamaño")]
        public int SubTitleFontSize { get; set; }

        [Display(Name = "Tamaño")]
        public int TableTitleFontSize { get; set; }

        [Display(Name = "Tamaño")]
        public int TableFontSize { get; set; }

        [Display(Name = "Tamaño")]
        public int BodyFontSize { get; set; }

        [Display(Name = "Tamaño")]
        public int InfoMessageFontSize { get; set; }

        [Display(Name = "Color")]
        public PdfSupportedColor TitleFontArgbColor { get; set; }

        [Display(Name = "Color")]
        public PdfSupportedColor SubTitleFontArgbColor { get; set; }

        [Display(Name = "Color")]
        public PdfSupportedColor TableTitleFontArgbColor { get; set; }

        [Display(Name = "Color")]
        public PdfSupportedColor TableFontArgbColor { get; set; }

        [Display(Name = "Color")]
        public PdfSupportedColor BodyFontArgbColor { get; set; }

        [Display(Name = "Color")]
        public PdfSupportedColor InfoMessageFontArgbColor { get; set; }

        [Display(Name = "Negritas")]
        public bool IsBoldTitleFont { get; set; }

        [Display(Name = "Negritas")]
        public bool IsBoldSubTitleFont { get; set; }

        [Display(Name = "Negritas")]
        public bool IsBoldTableTitleFont { get; set; }

        [Display(Name = "Negritas")]
        public bool IsBoldTableFont { get; set; }

        [Display(Name = "Negritas")]
        public bool IsBoldBodyFont { get; set; }

        [Display(Name = "Negritas")]
        public bool IsBoldInfoMessageFont { get; set; }

        [Display(Name = "Italica")]
        public bool IsItalicTitleFont { get; set; }

        [Display(Name = "Italica")]
        public bool IsItalicSubTitleFont { get; set; }

        [Display(Name = "Italica")]
        public bool IsItalicTableTitleFont { get; set; }

        [Display(Name = "Italica")]
        public bool IsItalicTableFont { get; set; }

        [Display(Name = "Italica")]
        public bool IsItalicBodyFont { get; set; }

        [Display(Name = "Italica")]
        public bool IsItalicInfoMessageFont { get; set; }
        #endregion

        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public List<FontListSelectItem> Fonts { get; set; }
        public List<int> FontSizes { get; set; }
        public IList<SelectListItem> FontColors
        {
            get
            {
                return EnumHelper.GetSelectList(typeof(PdfSupportedColor));
            }
        }
    }

    public class FontListSelectItem : SelectListItem
    {
        public string FontFamily { get; set; } = "Arial";

        public FontListSelectItem(string font)
        {
            Value = font;
            Text = font;
            FontFamily = font;
        }
    }
}
