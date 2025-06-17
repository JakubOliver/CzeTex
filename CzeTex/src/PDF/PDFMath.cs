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

namespace CzeTex
{
    public partial class PDF
    {
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

        public Text GetMultiplicationDot(List<string> list)
        {
            return this.GetMathText("\u22C5");
        }

        public void AddMultiplicationSign(List<string> list)
        {
            this.AddText("\u00D7");
        }

        public Text GetMultiplicationSign(List<string> list)
        {
            return this.GetMathText("\u00D7");
        }

        public void AddDivisionSign(List<string> list)
        {
            this.AddText("\u00F7");
        }

        public Text GetDivisionSign(List<string> list)
        {
            return this.GetMathText("\u00F7");
        }

        public void AddImplicationSign(List<string> list)
        {
            this.AddText("\u21D2");
        }

        public Text GetImplicationSign(List<string> list)
        {
            return this.GetMathText("\u21D2");
        }

        public void AddPower(List<string> list) //pridat u vseho kontrolu poctu argumentu
        {
            this.AddText(list[0]);
            this.AddText(new Text(list[1]).SetFontSize(8).SetTextRise(5));
        }

        //pridat moznost davat mocniny do zlomky, zlomly do zlomlu atd.

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
                string[] textSplit = text.Split();

                for (int j = 0; j < textSplit.Length; j++)
                {
                    if (StringFunctions.IsFunction(textSplit[j]))
                    {
                        int idx = trie.FindFunction(StringFunctions.GetFunctionName(textSplit[j]));
                        if (trie.getFunctions[idx] == null)
                        {
                            throw new Exception($"Function {StringFunctions.GetFunctionName(textSplit[j])} does not have getFunction");
                        }

                        paragraph.Add(((Func<List<string>, Text>)trie.getFunctions[idx]!)(new List<string>()));
                    }
                    else
                    {
                        paragraph.Add(this.GetMathText(textSplit[j]));
                    }

                    if (j != StringFunctions.LastIndex(textSplit))
                    {
                        paragraph.Add(new Text("\u00A0")); //není způsob jak zabránit tomu, aby se nezalamoval text v tabulce, proto využívám netisknutelných znaků
                    }
                }
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