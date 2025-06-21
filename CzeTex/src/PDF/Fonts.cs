using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace CzeTex
{
    /// <summary>
    /// Class containing constants for text generation.
    /// </summary>
    public static class Fonts
    {
        public const bool usingSans = true;
        public const int sizeOfTabulator = 4;

        public static readonly PdfFont sansDefaultFont =
            PdfFontFactory.CreateFont(
                "src/fonts/sans/OpenSans-Regular.ttf",
                PdfEncodings.IDENTITY_H
            );
        public static readonly PdfFont sansBoldFont =
            PdfFontFactory.CreateFont(
                "src/fonts/sans/OpenSans-Bold.ttf",
                PdfEncodings.IDENTITY_H
            );
        public static readonly PdfFont sansCursiveFont =
            PdfFontFactory.CreateFont(
                "src/fonts/sans/OpenSans-Italic.ttf",
                PdfEncodings.IDENTITY_H
            );
        public static readonly PdfFont sansBoldCursiveFont =
            PdfFontFactory.CreateFont(
                "src/fonts/sans/OpenSans-BoldItalic.ttf",
                PdfEncodings.IDENTITY_H
            );
        public static readonly PdfFont serifDefaultFont =
            PdfFontFactory.CreateFont(
                "src/fonts/serif/LibertinusSerif-Regular.ttf",
                PdfEncodings.IDENTITY_H
            );
        public static readonly PdfFont serifBoldFont =
            PdfFontFactory.CreateFont(
                "src/fonts/serif/LibertinusSerif-Bold.ttf",
                PdfEncodings.IDENTITY_H
            );
        public static readonly PdfFont serifCursiveFont =
            PdfFontFactory.CreateFont(
                "src/fonts/serif/LibertinusSerif-Italic.ttf",
                PdfEncodings.IDENTITY_H
            );
        public static readonly PdfFont serifBoldCursiveFont =
            PdfFontFactory.CreateFont(
                "src/fonts/serif/LibertinusSerif-BoldItalic.ttf",
                PdfEncodings.IDENTITY_H
            );
        public static readonly PdfFont mathFont =
            PdfFontFactory.CreateFont(
                "src/fonts/math/LibertinusMath-Regular.ttf",
                PdfEncodings.IDENTITY_H
            );

        public static readonly Color linkColor = ColorConstants.BLUE;

        public const uint defaultSize = 11;
        public const uint defaultTitleSizeDifference = 2;
        public const uint defaultTitleSize = 20;
        public const uint defaultSubTitleSize =
            defaultTitleSize - defaultTitleSizeDifference;
        public const uint defaultSubSubTitleSize =
            defaultTitleSize - 2 * defaultTitleSizeDifference;

        public const float defaultSpacing = 1.25f;
        
        public const TextAlignment alignmentLeft = TextAlignment.LEFT;
        public const TextAlignment alignmentCenter = TextAlignment.CENTER;
        public const TextAlignment alignmentRight = TextAlignment.RIGHT;
        public const TextAlignment alignmentJustified = TextAlignment.JUSTIFIED;
        public const TextAlignment defaultAlignment = alignmentLeft;
    }
}