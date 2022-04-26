using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using iTextSharp.text;

namespace TicoPay.ReportsSettings
{
    public class ReportSettings : AuditedEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public string ReportName { get; set; }
        public string ReportTitle { get; set; }
        public bool ShowPrintDate { get; set; }
        public bool ShowPageNumber { get; set; }

        #region Watermark
        public bool UseWatermark { get; set; }
        public byte[] WatermarkImage { get; set; }
        public WatermarkPosition WatermarkPosition { get; set; }
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
        public string TitleFontName { get; set; }
        public string SubTitleFontName { get; set; }
        public string TableTitleFontName { get; set; }
        public string TableFontName { get; set; }
        public string BodyFontName { get; set; }
        public string InfoMessageFontName { get; set; }

        public int TitleFontSize { get; set; }
        public int SubTitleFontSize { get; set; }
        public int TableTitleFontSize { get; set; }
        public int TableFontSize { get; set; }
        public int BodyFontSize { get; set; }
        public int InfoMessageFontSize { get; set; }

        public PdfSupportedColor TitleFontArgbColor { get; set; }
        public PdfSupportedColor SubTitleFontArgbColor { get; set; }
        public PdfSupportedColor TableTitleFontArgbColor { get; set; }
        public PdfSupportedColor TableFontArgbColor { get; set; }
        public PdfSupportedColor BodyFontArgbColor { get; set; }
        public PdfSupportedColor InfoMessageFontArgbColor { get; set; }

        public bool IsBoldTitleFont { get; set; }
        public bool IsBoldSubTitleFont { get; set; }
        public bool IsBoldTableTitleFont { get; set; }
        public bool IsBoldTableFont { get; set; }
        public bool IsBoldBodyFont { get; set; }
        public bool IsBoldInfoMessageFont { get; set; }

        public bool IsItalicTitleFont { get; set; }
        public bool IsItalicSubTitleFont { get; set; }
        public bool IsItalicTableTitleFont { get; set; }
        public bool IsItalicTableFont { get; set; }
        public bool IsItalicBodyFont { get; set; }
        public bool IsItalicInfoMessageFont { get; set; }
        #endregion
    }

    public enum WatermarkPosition
    {
        Fill,
        Center,
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom
    }
}
