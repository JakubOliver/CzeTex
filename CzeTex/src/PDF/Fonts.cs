using iText.IO.Font;
using iText.Kernel.Font;

namespace CzeTex
{
    public static class Fonts
    {
        public static PdfFont defaultFont = PdfFontFactory.CreateFont("src/open-sans/OpenSans-Regular.ttf", PdfEncodings.IDENTITY_H);
        public static PdfFont boldFont = PdfFontFactory.CreateFont("src/open-sans/OpenSans-Bold.ttf", PdfEncodings.IDENTITY_H);
        public static PdfFont cursiveFont = PdfFontFactory.CreateFont("src/open-sans/OpenSans-Italic.ttf", PdfEncodings.IDENTITY_H);
        public static uint defaultSize = 12;
    }
}