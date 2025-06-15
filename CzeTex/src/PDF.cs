using iText;
using iText.Forms.Form.Element;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.IO;

namespace CzeTex{
    public class PDF
    {
        PdfWriter writer;
        PdfDocument pdf;
        Document document;

        PdfFont font = PdfFontFactory.CreateFont("src/open-sans/OpenSans-Regular.ttf", PdfEncodings.IDENTITY_H);
        PdfFont boldFont = PdfFontFactory.CreateFont("src/open-sans/OpenSans-Bold.ttf", PdfEncodings.IDENTITY_H);
        PdfFont cursiveFont = PdfFontFactory.CreateFont("src/open-sans/OpenSans-Italic.ttf", PdfEncodings.IDENTITY_H);

        CharacteristicsStack stack;

        Paragraph? activeParagraph;
        public PDF(string basename)
        {
            writer = new PdfWriter($"output/{basename}.pdf");
            pdf = new PdfDocument(writer);
            document = new Document(pdf);
            document.SetFont(font);

            stack = new CharacteristicsStack(document);
            stack.Push(font);
        }

        public void Export()
        {
            if (this.activeParagraph != null)
            {
                this.AddParagraph();
            }

            this.document.Close();
        }

        public void CreateParagraph()
        {
            if (this.activeParagraph != null)
            {
                this.AddParagraph();
            }

            this.activeParagraph = new Paragraph();
        }

        public void AddParagraph()
        {
            this.document.Add(this.activeParagraph);
        }

        public void AddTitle()
        {
            this.CreateParagraph();
            this.stack.Push(20);
        }

        public void AddText(string text)
        {
            this.AddText(new Text(text));
        }

        public void AddText(Text text)
        {
            if (this.activeParagraph == null)
            {
                throw new Exception("No active paragraph. Create a paragraph first.");
            }

            if (this.stack.Top().DoNotAdd())
            {
                this.stack.Top().Special(text);
                return;
            }

            if (this.stack.IsSpecial())
            {
                this.activeParagraph.Add(this.stack.Top().Special(text));
            }
            else
            {
                this.activeParagraph.Add(this.stack.Top().Define(text));
            }

            this.activeParagraph.Add(this.stack.Top().Define(new Text(" ")));
        }

        public void AddBoldText()
        {
            this.stack.Push(this.boldFont);
        }

        public void AddCursiveText()
        {
            this.stack.Push(this.cursiveFont);
        }

        public void AddSlash()
        {
            this.AddText("/");
        }

        public void RemoveFont()
        {
            this.stack.Pop();
        }

        public void AddNewPage()
        {
            this.AddParagraph();
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
        }

        public void AddUnderLineText()
        {
            this.stack.Push(new UnderLineText(stack.Font, stack.Size));
        }

        public void AddLineThroughText()
        {
            this.stack.Push(new LineThroughText(stack.Font, stack.Size));
        }

        /*
        public void AddDottedUnderline()
        {

        }
        */

        //podtrazení, přeškrtnutí atd.

        public void AddList()
        {
            this.CreateParagraph();
            this.stack.Push(new ListText(stack.Font, stack.Size));
        }

        public void AddListItem()
        {
            this.stack.Push(new ListItemText(stack.Font, stack.Size));
        }
    }
}