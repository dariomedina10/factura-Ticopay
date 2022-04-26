using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.Reports
{
    public class ConnectorReportSettings
    {
        public int TenantId { get; set; }

        public string ReportName { get; set; }
        public string ReportTitle { get; set; }
        public bool ShowPrintDate { get; set; }
        public bool ShowPageNumber { get; set; }

        #region Watermark
        public bool UseWatermark { get; set; }
        public byte[] WatermarkImage { get; set; }
        public ConnectorWatermarkPosition WatermarkPosition { get; set; }
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

        public ConnectorPdfSupportedColor TitleFontArgbColor { get; set; }
        public ConnectorPdfSupportedColor SubTitleFontArgbColor { get; set; }
        public ConnectorPdfSupportedColor TableTitleFontArgbColor { get; set; }
        public ConnectorPdfSupportedColor TableFontArgbColor { get; set; }
        public ConnectorPdfSupportedColor BodyFontArgbColor { get; set; }
        public ConnectorPdfSupportedColor InfoMessageFontArgbColor { get; set; }

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

    public enum ConnectorWatermarkPosition
    {
        Fill,
        Center,
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom
    }

    public enum ConnectorPdfSupportedColor
    {
        BLACK,        
        BLUE,        
        CYAN,        
        MAGENTA,        
        GREEN,        
        ORANGE,        
        YELLOW,        
        RED,        
        WHITE,        
        DARK_GRAY,        
        GRAY,        
        LIGHT_GRAY,        
        PINK,
    }
}
