using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService.Reports
{
    public class ConnectorPdfSettingsHelper
    {
        public const float OneInch = 25.4f;

        public ConnectorReportSettings Settings { get; private set; }

        private ConnectorPdfSettingsHelper()
        {
        }

        public static ConnectorPdfSettingsHelper Create(ConnectorReportSettings reportSettings)
        {
            if (reportSettings == null)
            {
                throw new ArgumentNullException(nameof(reportSettings));
            }

            return new ConnectorPdfSettingsHelper
            {
                Settings = reportSettings
            };
        }

        public Font GetTitleFont()
        {
            if (string.IsNullOrWhiteSpace(Settings.TitleFontName) || Settings.TitleFontSize == 0)
            {
                return FontFactory.GetFont("Arial", 12, Font.BOLD, BaseColor.BLACK);
            }
            int fontStyle = Font.BOLD;
            if (Settings.IsBoldTitleFont && Settings.IsItalicTitleFont)
            {
                fontStyle = Font.BOLDITALIC;
            }
            else if (Settings.IsBoldTitleFont)
            {
                fontStyle = Font.BOLD;
            }
            else if (Settings.IsItalicTitleFont)
            {
                fontStyle = Font.ITALIC;
            }
            BaseColor color = GetSupportedColor(Settings.TitleFontArgbColor);
            return FontFactory.GetFont(Settings.TitleFontName, Settings.TitleFontSize, fontStyle, color);
        }

        public Font GetSubTitleFont()
        {
            if (string.IsNullOrWhiteSpace(Settings.SubTitleFontName) || Settings.SubTitleFontSize == 0)
            {
                return FontFactory.GetFont("Arial", 10, Font.BOLD);
            }
            int fontStyle = Font.BOLD;
            if (Settings.IsBoldSubTitleFont && Settings.IsItalicSubTitleFont)
            {
                fontStyle = Font.BOLDITALIC;
            }
            else if (Settings.IsBoldSubTitleFont)
            {
                fontStyle = Font.BOLD;
            }
            else if (Settings.IsItalicSubTitleFont)
            {
                fontStyle = Font.ITALIC;
            }
            BaseColor color = GetSupportedColor(Settings.SubTitleFontArgbColor);
            return FontFactory.GetFont(Settings.SubTitleFontName, Settings.SubTitleFontSize, fontStyle, color);
        }

        public Font GetTableTitleFont()
        {
            if (string.IsNullOrWhiteSpace(Settings.TableTitleFontName) || Settings.TableTitleFontSize == 0)
            {
                return FontFactory.GetFont("Arial", 10, Font.BOLD);
            }
            int fontStyle = Font.BOLD;
            if (Settings.IsBoldTableTitleFont && Settings.IsItalicTableTitleFont)
            {
                fontStyle = Font.BOLDITALIC;
            }
            else if (Settings.IsBoldTableTitleFont)
            {
                fontStyle = Font.BOLD;
            }
            else if (Settings.IsItalicTableTitleFont)
            {
                fontStyle = Font.ITALIC;
            }
            BaseColor color = GetSupportedColor(Settings.TableTitleFontArgbColor);
            return FontFactory.GetFont(Settings.TableTitleFontName, Settings.TableTitleFontSize, fontStyle, color);
        }

        public Font GetTableFont()
        {
            if (string.IsNullOrWhiteSpace(Settings.TableFontName) || Settings.TableFontSize == 0)
            {
                return FontFactory.GetFont("Arial", 10, Font.NORMAL);
            }
            int fontStyle = Font.NORMAL;
            if (Settings.IsBoldTableFont && Settings.IsItalicTableFont)
            {
                fontStyle = Font.BOLDITALIC;
            }
            else if (Settings.IsBoldTableFont)
            {
                fontStyle = Font.BOLD;
            }
            else if (Settings.IsItalicTableFont)
            {
                fontStyle = Font.ITALIC;
            }
            BaseColor color = GetSupportedColor(Settings.TableFontArgbColor);
            return FontFactory.GetFont(Settings.TableFontName, Settings.TableFontSize, fontStyle, color);
        }

        public Font GetInfoMessageFont()
        {
            if (string.IsNullOrWhiteSpace(Settings.InfoMessageFontName) || Settings.InfoMessageFontSize == 0)
            {
                return FontFactory.GetFont("Arial", 10, Font.ITALIC);
            }

            int fontStyle = Font.NORMAL;
            if (Settings.IsBoldInfoMessageFont && Settings.IsItalicInfoMessageFont)
            {
                fontStyle = Font.BOLDITALIC;
            }
            else if (Settings.IsBoldInfoMessageFont)
            {
                fontStyle = Font.BOLD;
            }
            else if (Settings.IsItalicInfoMessageFont)
            {
                fontStyle = Font.ITALIC;
            }
            BaseColor color = GetSupportedColor(Settings.InfoMessageFontArgbColor);
            return FontFactory.GetFont(Settings.InfoMessageFontName, Settings.InfoMessageFontSize, fontStyle, color);
        }

        public Font GetBodyFont()
        {
            if (string.IsNullOrWhiteSpace(Settings.BodyFontName) || Settings.BodyFontSize == 0)
            {
                return FontFactory.GetFont("Arial", 8, Font.NORMAL);
            }
            int fontStyle = Font.NORMAL;
            if (Settings.IsBoldBodyFont && Settings.IsItalicBodyFont)
            {
                fontStyle = Font.BOLDITALIC;
            }
            else if (Settings.IsBoldBodyFont)
            {
                fontStyle = Font.BOLD;
            }
            else if (Settings.IsItalicBodyFont)
            {
                fontStyle = Font.ITALIC;
            }
            BaseColor color = GetSupportedColor(Settings.BodyFontArgbColor);
            return FontFactory.GetFont(Settings.BodyFontName, Settings.BodyFontSize, fontStyle, color);
        }

        public Rectangle GetPageSize()
        {
            if (Settings.IsCustomSize)
            {
                return new Rectangle(Settings.LowerLeftX, Settings.LowerLeftY, Settings.UpperRightX, Settings.UpperRightY).Rotate();
            }
            else if (!string.IsNullOrWhiteSpace(Settings.PageSizeName))
            {
                return PageSize.GetRectangle(Settings.PageSizeName).Rotate();
            }
            else
            {
                return PageSize.LETTER.Rotate();
            }
        }

        public string GetPageSizeName()
        {
            return string.IsNullOrWhiteSpace(Settings.PageSizeName) ? "LETTER" : Settings.PageSizeName;
        }

        public float GetMarginLeft()
        {
            return Settings.MarginLeft == 0 ? OneInch : Settings.MarginLeft * OneInch;
        }

        public float GetMarginRight()
        {
            return Settings.MarginRight == 0 ? OneInch : Settings.MarginRight * OneInch;
        }

        public float GetMarginTop()
        {
            return Settings.MarginTop == 0 ? OneInch : Settings.MarginTop * OneInch;
        }

        public float GetMarginBottom()
        {
            return Settings.MarginBottom == 0 ? OneInch : Settings.MarginBottom * OneInch;
        }

        public byte[] GetWatermarkImage()
        {
            /////TEST must be deleted
            //if (Settings.WatermarkImage == null || Settings.WatermarkImage.Length == 0)
            //{
            //    System.Drawing.Image img = System.Drawing.Image.FromFile(@"C:\Users\yanez\Google Drive\TicoPay\image_placeholder_02.jpg");
            //    byte[] bytes;
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //        bytes = ms.ToArray();
            //    }
            //    return bytes;
            //}
            /////END TEST
            return Settings.WatermarkImage;
        }

        public Document CreateDocument()
        {
            return new Document(GetPageSize(), GetMarginLeft(), GetMarginRight(), GetMarginTop(), GetMarginBottom());
        }

        public IPdfPageEvent GetPageEventHandler()
        {
            return new ConnectorPdfWatermarlWriter(GetWatermarkImage(), GetPageSize());
        }

        private static BaseColor GetSupportedColor(ConnectorPdfSupportedColor supportedColor)
        {
            switch (supportedColor)
            {
                case ConnectorPdfSupportedColor.WHITE:
                    return BaseColor.WHITE;
                case ConnectorPdfSupportedColor.BLUE:
                    return BaseColor.BLUE;
                case ConnectorPdfSupportedColor.CYAN:
                    return BaseColor.CYAN;
                case ConnectorPdfSupportedColor.MAGENTA:
                    return BaseColor.MAGENTA;
                case ConnectorPdfSupportedColor.GREEN:
                    return BaseColor.GREEN;
                case ConnectorPdfSupportedColor.ORANGE:
                    return BaseColor.ORANGE;
                case ConnectorPdfSupportedColor.YELLOW:
                    return BaseColor.YELLOW;
                case ConnectorPdfSupportedColor.RED:
                    return BaseColor.RED;
                case ConnectorPdfSupportedColor.BLACK:
                    return BaseColor.BLACK;
                case ConnectorPdfSupportedColor.DARK_GRAY:
                    return BaseColor.DARK_GRAY;
                case ConnectorPdfSupportedColor.GRAY:
                    return BaseColor.GRAY;
                case ConnectorPdfSupportedColor.LIGHT_GRAY:
                    return BaseColor.LIGHT_GRAY;
                case ConnectorPdfSupportedColor.PINK:
                    return BaseColor.PINK;
                default:
                    return BaseColor.BLACK;
            }
        }
    }
}
