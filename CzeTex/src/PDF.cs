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
    
        PdfFont font = PdfFontFactory.CreateFont("src/OpenSans-Regular.ttf",PdfEncodings.IDENTITY_H);

        Paragraph? activeParagraph;
        public PDF(){
            writer = new PdfWriter("output/output.pdf");
            pdf = new PdfDocument(writer);
            document = new Document(pdf);
            document.SetFont(font);
        }

        public void Export(){
            this.document.Close();
        }

        public void CreateParagraph(){
            this.activeParagraph = new Paragraph();
        }

        public void AddParagraph(){
            this.document.Add(this.activeParagraph);
        }

        public void AddText(string text){
            this.AddText(new Text(text));
        }

        public void AddText(Text text){
            if (this.activeParagraph == null){
                throw new Exception("No active paragraph. Create a paragraph first.");
            }

            this.activeParagraph.Add(text).SetFont(this.font);
        }
    }
}