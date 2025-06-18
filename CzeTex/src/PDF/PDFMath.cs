using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using iText.Layout.Borders;

namespace CzeTex
{
    public partial class PDF
    {
        /// <summary>
        /// Starts math text section.
        /// </summary>
        public void AddMathPart(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.stack.Push(Fonts.mathFont);
        }

        /// <summary>
        /// Adds text in math font.
        /// </summary>
        public void AddMathText(string text)
        {
            this.AddText(new Text(text).SetFont(Fonts.mathFont));
        }

        /// <summary>
        /// Returns text in math font.
        /// </summary>
        public Text GetMathText(string text)
        {
            Text mathText = new Text(text).SetFont(Fonts.mathFont);
            return mathText;
        }

        /// <summary>
        /// Adds math sign and checks calling CzeTex function.
        /// </summary>
        public void AddSign(string sign, List<string> list, int numberOfParameters = 0)
        {
            CallerManager.CorrectParameters(list, numberOfParameters);

            this.AddText(sign);
        }

        /// <summary>
        /// Returns math sign and checks calling CzeTex function.
        /// </summary>
        public Text GetSign(string sign, List<string> list, int numberOfParameters = 0)
        {
            CallerManager.CorrectParameters(list, numberOfParameters);

            return this.GetMathText(sign);
        }

        /// <summary>
        /// Adds mathematical symbolic for power.
        /// </summary>
        public void AddPower(List<string> list)
        {
            CallerManager.CorrectParameters(list, 2);

            this.AddText(list[0]);
            this.AddText(new Text(list[1]).SetFontSize(8).SetTextRise(5));
        }

        //pridat moznost davat mocniny do zlomky, zlomly do zlomlu atd.

        /// <summary>
        /// Adds inline fraction into text.
        /// </summary>
        public void AddInlineFraction(List<string> list)
        {
            CallerManager.CorrectParameters(list, 2);

            this.AddText(new Text(list[0]).SetTextRise(2), false);
            this.AddText("/", false);
            this.AddText(list[1]);
        }

        /// <summary>
        /// Sets properties of numerator or denominator.
        /// And returns respective part.
        /// </summary>
        public Cell SetFractionPart(String text, int repetition = 1)
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
                        //The iText library has autocropping enabled after every word, so it is necessary to use non-printable characters instead of spaces
                        paragraph.Add(new Text("\u00A0")); 
                    }
                }
            }

            cell.Add(paragraph);

            return cell;
        }

        /// <summary>
        /// Adds fraction into text.
        /// </summary>
        public void AddFraction(List<string> list)
        {
            CallerManager.CorrectParameters(list, 2);

            int length = Math.Max(list[0].Length, list[1].Length);

            Table fraction = new Table(1);
            fraction.SetWidth(length);
            fraction.SetMargin(0);

            Cell numerator = this.SetFractionPart(list[0]);
            fraction.AddCell(numerator);

            //Cell fractionLine = this.SetCell("—", length);
            //fraction.AddCell(fractionLine);

            Cell denominator = this.SetFractionPart(list[1]);
            denominator.SetBorderTop(new SolidBorder(1));
            fraction.AddCell(denominator);

            this.activeParagraph!.Add(fraction);
            this.AddText(" ");

            this.stack.Push(new RisedText(this.stack.Font, this.stack.Size, 10));
        }
    }
}