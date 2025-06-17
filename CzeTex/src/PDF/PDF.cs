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
            this.CreateParagraph(list);
            this.stack.Push(20);
        }

        public void AddSubTitle(List<string> list)
        {
            this.CreateParagraph(list);
            this.stack.Push(18);
        }

        public void AddSubSubTitle(List<string> list)
        {
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
            this.stack.Push(Fonts.boldFont);
        }

        public void AddCursiveText(List<string> list)
        {
            this.stack.Push(Fonts.cursiveFont);
        }

        public void AddSlash(List<string> list)
        {
            this.AddText("/");
        }

        public void RemoveFont(List<string> list)
        {
            this.stack.Pop();
        }

        public void AddNewPage(List<string> list)
        {
            this.AddParagraph();
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
        }

        public void AddUnderLineText(List<string> list)
        {
            this.stack.Push(new UnderLineText(stack.Font, stack.Size));
        }

        public void AddLineThroughText(List<string> list)
        {
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
            this.CreateParagraph(list);
            this.stack.Push(new ListText(stack.Font, stack.Size));
        }

        public void AddListItem(List<string> list)
        {
            this.stack.Push(new ListItemText(stack.Font, stack.Size));
        }

        public void AddThis(List<string> list) //testovaci funkce
        {
            this.AddText(list[0]);
        }

        public void AddMathPart(List<string> list)
        {
            this.stack.Push(Fonts.mathFont);
        }

        public void AddMathText(string text)
        {
            this.AddText(new Text(text).SetFont(Fonts.mathFont));
        }

        public Text GetMathText(string text)
        {
            Text mathText = new Text(text).SetFont(Fonts.mathFont);
            return mathText;
        }

        public void AddMultiplicationDot(List<string> list) //pridat speciální font pro matematický text
        {
            this.AddText("\u22C5");
        }

        public void AddMultiplicationSign(List<string> list)
        {
            this.AddText("\u00D7");
        }

        public void AddDivisionSign(List<string> list)
        {
            this.AddText("\u00F7");
        }

        public void AddImplificationSign(List<string> list)
        {
            this.AddText("\u21D2");
        }

        public void AddPower(List<string> list) //pridat u vseho kontrolu poctu argumentu
        {
            this.AddText(list[0]);
            this.AddText(new Text(list[1]).SetFontSize(8).SetTextRise(5));
        }

        public void AddInlineFraction(List<string> list)
        {
            this.AddText(new Text(list[0]).SetTextRise(2), false);
            this.AddText("/", false);
            this.AddText(list[1]);
        }

        public Cell SetCell(String text, int repetition = 1)
        {
            Cell cell = new Cell();
            cell.SetTextAlignment(TextAlignment.CENTER);
            cell.SetBorder(Border.NO_BORDER);
            cell.SetMarginTop(0);
            cell.SetMarginBottom(0.3f);
            cell.SetPadding(0);

            Paragraph paragraph = new Paragraph();
            paragraph.SetMarginBottom(0);
            paragraph.SetMarginBottom(0);
            paragraph.SetPadding(0);
            paragraph.SetMultipliedLeading(0.8f);

            for (int i = 0; i < repetition; i++)
            {
                paragraph.Add(this.GetMathText(text));
            }

            cell.Add(paragraph);

            return cell;
        }

        public void AddFraction(List<string> list)
        {
            int length = Math.Max(list[0].Length, list[1].Length);

            Table fraction = new Table(1);
            fraction.SetWidth(length);
            fraction.SetMargin(0);

            Cell numerator = this.SetCell(list[0]);
            fraction.AddCell(numerator);

            //Cell fractionLine = this.SetCell("—", length);
            //fraction.AddCell(fractionLine);

            Cell denominator = this.SetCell(list[1]);
            denominator.SetBorderTop(new SolidBorder(1));
            fraction.AddCell(denominator);

            document.Add(fraction);
        }
    }
}