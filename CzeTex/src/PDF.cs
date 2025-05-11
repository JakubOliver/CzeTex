using iText;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

using System;
using System.IO;

namespace CzeTex{
    public class PDF{
        PdfWriter writer;
        PdfDocument pdf;
        Document document;
    
        PdfFont font = PdfFontFactory.CreateFont("src/open-sans/OpenSans-Regular.ttf",PdfEncodings.IDENTITY_H);
        PdfFont boldFont = PdfFontFactory.CreateFont("src/open-sans/OpenSans-Bold.ttf",PdfEncodings.IDENTITY_H);
        PdfFont cursiveFont = PdfFontFactory.CreateFont("src/open-sans/OpenSans-Italic.ttf",PdfEncodings.IDENTITY_H);

        Paragraph? activeParagraph;
        public PDF(){
            writer = new PdfWriter("output/output.pdf");
            pdf = new PdfDocument(writer);
            document = new Document(pdf);
            document.SetFont(font);
        }

        public void Export(){
            if (this.activeParagraph != null){
                this.AddParagraph();
            }

            this.document.Close();
        }

        public void AddTitle(string title){
            this.document.Add(new Paragraph(title).SetFontSize(20));
        }

        public void CreateParagraph(){
            if (this.activeParagraph != null){
                this.AddParagraph();
            }

            this.activeParagraph = new Paragraph();
        }

        public void AddParagraph(){
            this.document.Add(this.activeParagraph);
        }

        public void AddText(string text){
            this.AddText(new Text(text));
        }

        public void AddText(Text text, bool Custom = false){
            if (this.activeParagraph == null){
                throw new Exception("No active paragraph. Create a paragraph first.");
            }

            if (Custom){
                this.activeParagraph.Add(text);
                return;
            } else {
                this.activeParagraph.Add(text).SetFont(this.font);
            }

            this.activeParagraph.Add(" ");
        }

        public void AddBoldText(string text){
            this.AddText(new Text(text).SetFont(this.boldFont));
        }

        public void AddCursiveText(string text){
            this.AddText(new Text(text).SetFont(this.cursiveFont));
        }
    }
}