using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;

namespace CzeTex
{
    /// <summary>
    /// Class containing constants for text generation.
    /// </summary>
    public static class Fonts
    {
        public static bool usingSans = true;

        public static PdfFont sansDefaultFont =
            PdfFontFactory.CreateFont("src/fonts/sans/OpenSans-Regular.ttf", PdfEncodings.IDENTITY_H);
        public static PdfFont sansBoldFont =
            PdfFontFactory.CreateFont("src/fonts/sans/OpenSans-Bold.ttf", PdfEncodings.IDENTITY_H);
        public static PdfFont sansCursiveFont =
            PdfFontFactory.CreateFont("src/fonts/sans/OpenSans-Italic.ttf", PdfEncodings.IDENTITY_H);
        public static PdfFont sansBoldCursiveFont =
            PdfFontFactory.CreateFont("src/fonts/sans/OpenSans-BoldItalic.ttf", PdfEncodings.IDENTITY_H);
        public static PdfFont serifDefaultFont =
            PdfFontFactory.CreateFont("src/fonts/serif/LibertinusSerif-Regular.ttf", PdfEncodings.IDENTITY_H);
        public static PdfFont serifBoldFont =
            PdfFontFactory.CreateFont("src/fonts/serif/LibertinusSerif-Bold.ttf", PdfEncodings.IDENTITY_H);
        public static PdfFont serifCursiveFont =
            PdfFontFactory.CreateFont("src/fonts/serif/LibertinusSerif-Italic.ttf", PdfEncodings.IDENTITY_H);
        public static PdfFont serifBoldCursiveFont =
            PdfFontFactory.CreateFont("src/fonts/serif/LibertinusSerif-BoldItalic.ttf", PdfEncodings.IDENTITY_H);
        public static PdfFont mathFont =
            PdfFontFactory.CreateFont("src/fonts/math/LibertinusMath-Regular.ttf", PdfEncodings.IDENTITY_H);
        public static Color linkColor = ColorConstants.BLUE;
        public static uint defaultSize = 11;
        public static uint defaultTitleSizeDifference = 2;
        public static uint defaultTitleSize = 20;
        public static uint defaultSubTitleSize =
            defaultTitleSize - defaultTitleSizeDifference;
        public static uint defaultSubSubTitleSize =
            defaultTitleSize - 2 * defaultTitleSizeDifference;
        public static float defaultSpacing = 1.25f;
    }
}