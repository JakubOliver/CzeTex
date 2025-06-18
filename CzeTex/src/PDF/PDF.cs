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
using System.Collections.Generic;
using System.Security;
using iText.Layout.Borders;

namespace CzeTex{
    public partial class PDF
    {
        private PdfWriter writer;
        private PdfDocument pdf;
        private Document document;

        private CharacteristicsStack stack;
        private Trie trie;

        private Paragraph? activeParagraph;
        public PDF(string basename, Trie trie)
        {
            writer = new PdfWriter($"output/{basename}.pdf");
            pdf = new PdfDocument(writer);
            document = new Document(pdf);
            document.SetFont(Fonts.defaultFont);

            stack = new CharacteristicsStack(document);
            stack.Push(Fonts.defaultFont);

            this.trie = trie;
        }

        public void Export()
        {
            if (this.activeParagraph != null)
            {
                this.AddParagraph();
            }

            this.document.Close();
        }

        public void CreateParagraph(List<string> list)
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

        public void AddTitle(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.CreateParagraph(list);
            this.stack.Push(20);
        }

        public void AddSubTitle(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.CreateParagraph(list);
            this.stack.Push(18);
        }

        public void AddSubSubTitle(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.CreateParagraph(list);
            this.stack.Push(16);
        }

        public void AddText(string text, bool addWhiteSpace = true)
        {
            this.AddText(new Text(text), addWhiteSpace);
        }

        public void AddText(Text text, bool addWhiteSpace = true)
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

            if (addWhiteSpace)
            {
                this.activeParagraph.Add(this.stack.Top().Define(new Text(" ")));
            }
        }

        public void AddBoldText(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.stack.Push(Fonts.boldFont);
        }

        public void AddCursiveText(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.stack.Push(Fonts.cursiveFont);
        }

        public void AddSlash(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText("/");
        }

        public void RemoveFont(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.stack.Pop();
        }

        public void AddNewPage(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddParagraph();
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
        }

        public void AddUnderLineText(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.stack.Push(new UnderLineText(stack.Font, stack.Size));
        }

        public void AddLineThroughText(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.stack.Push(new LineThroughText(stack.Font, stack.Size));
        }

        /*
        public void AddDottedUnderline()
        {

        }
        */

        //podtrazení, přeškrtnutí atd.

        public void AddList(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.CreateParagraph(list);
            this.stack.Push(new ListText(stack.Font, stack.Size));
        }

        public void AddListItem(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.stack.Push(new ListItemText(stack.Font, stack.Size));
        }

        public void AddThis(List<string> list) //testovaci funkce
        {
            this.AddText(list[0]);
        }
    }
}