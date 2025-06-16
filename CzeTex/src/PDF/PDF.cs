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
        private PdfWriter writer;
        private PdfDocument pdf;
        private Document document;

        private CharacteristicsStack stack;

        private Paragraph? activeParagraph;
        public PDF(string basename)
        {
            writer = new PdfWriter($"output/{basename}.pdf");
            pdf = new PdfDocument(writer);
            document = new Document(pdf);
            document.SetFont(Fonts.defaultFont);

            stack = new CharacteristicsStack(document);
            stack.Push(Fonts.defaultFont);
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

            if (this.stack.Top() is DoNotAddTextCharacteristics) //vyresit nadbytecne boolovske hodnoty u TextCharacteristics
            {
                this.stack.Top().Special(text);
                return;
            }

            if (this.stack.Top() is SpecialTextCharacteristics)
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
            this.stack.Push(Fonts.boldFont);
        }

        public void AddCursiveText()
        {
            this.stack.Push(Fonts.cursiveFont);
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