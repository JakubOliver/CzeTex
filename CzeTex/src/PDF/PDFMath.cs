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
            CallerManager.CorrectParameters(list, 0);

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
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.multiplicationDot);
        }

        public Text GetMultiplicationDot(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.multiplicationDot);
        }

        public void AddMultiplicationSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);
            
            this.AddText(Signs.multiplicationSign);
        }

        public Text GetMultiplicationSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);
            
            return this.GetMathText(Signs.multiplicationSign);
        }

        public void AddDivisionSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.divisionSign);
        }

        public Text GetDivisionSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.divisionSign);
        }

        public void AddImplicationSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.implicationSign);
        }

        public Text GetImplicationSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.implicationSign);
        }

        public void AddNegationSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.negationSign);
        }

        public Text GetNegationSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.negationSign);
        }

        public void AddElementOfSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.elementOfSign);
        }

        public Text GetElementOfSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.elementOfSign);
        }

        public void AddNotElementOfSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.notElementOfSign);
        }

        public Text GetNotElementOfSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.notElementOfSign);
        }

        public void AddForAllSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.forAllSign);
        }

        public Text GetForAllSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.forAllSign);
        }

        public void AddExistsSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.existsSign);
        }

        public Text GetExistsSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.existsSign);
        }

        public void AddInfinitySign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.infinitySign);
        }

        public Text GetInfinitySign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);
            
            return this.GetMathText(Signs.infinitySign);
        }

        public void AddLogicalAndSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.logicalAndSign);
        }

        public Text GetLogicalAndSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.logicalAndSign);
        }

        public void AddLogicalOrSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.logicalOrSign);
        }

        public Text GetLogicalOrSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.logicalOrSign);
        }

        public void AddRealNumbersSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.realNumbersSign);
        }

        public Text GetRealNumbersSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.realNumbersSign);
        }

        public void AddNaturalNumbersSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.naturalNumbersSign);
        }

        public Text GetNaturalNumbersSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.naturalNumbersSign);
        }

        public void AddIntegersSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.integerSign);
        }

        public Text GetIntegersSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.integerSign);
        }

        public void AddRationalNumbersSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.rationalNumberSign);
        }

        public Text GetRationalNumbersSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.rationalNumberSign);
        }

        public void AddComplexNumbersSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.complexNumberSign);
        }

        public Text GetComplexNumbersSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.complexNumberSign);
        }

        public void AddNotEqualSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.notEqualSign);
        }

        public Text GetNotEqualSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.notEqualSign);
        }

        public void AddLessThanOrEqualSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.lessThanOrEqualSign);
        }

        public Text GetLessThanOrEqualSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.lessThanOrEqualSign);
        }

        public void AddGreaterThanOrEqualSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.greaterThanOrEqualSign);
        }

        public Text GetGreaterThanOrEqualSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.greaterThanOrEqualSign);
        }

        public void AddIfAndOnlyIfSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.ifAndOnlyIfSign);
        }

        public Text GetIfAndOnlyIfSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.ifAndOnlyIfSign);
        }

        public void AddEpsilonSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.epsilonSign);
        }

        public Text GetEpsilonSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.epsilonSign);
        }

        public void AddDeltaSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.deltaSign);
        }

        public Text GetDeltaSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.deltaSign);
        }

        public void AddPiSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText(Signs.piSign);
        }

        public Text GetPiSign(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            return this.GetMathText(Signs.piSign);
        }

        public void AddPower(List<string> list) //pridat u vseho kontrolu poctu argumentu
        {
            CallerManager.CorrectParameters(list, 2);

            this.AddText(list[0]);
            this.AddText(new Text(list[1]).SetFontSize(8).SetTextRise(5));
        }

        //pridat moznost davat mocniny do zlomky, zlomly do zlomlu atd.

        public void AddInlineFraction(List<string> list)
        {
            CallerManager.CorrectParameters(list, 2);

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
            CallerManager.CorrectParameters(list, 2);

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

            this.activeParagraph.Add(fraction);
            this.AddText(" ");

            this.stack.Push(new RisenText(this.stack.Font, this.stack.Size, 10));
        }
    }
}