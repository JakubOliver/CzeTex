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
            this.AddText(new CzeTexText(text).SetFont(Fonts.mathFont));
        }

        /// <summary>
        /// Returns text in math font.
        /// </summary>
        public CzeTexText GetMathText(string text)
        {
            CzeTexText mathText = new CzeTexText(text).SetFont(Fonts.mathFont);
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
        public CzeTexText GetSign(string sign, List<string> list, int numberOfParameters = 0)
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
            CallerManager.ParagraphIsSet(this.activeParagraph);

            //Method ParagraphIsSet checks whether the activeParagraph
            //is null, therefore the warning is unjustified.
            this.AddParameterToParagraph(this.activeParagraph!, list[0], true);
            this.AddParameterToParagraph(this.activeParagraph!, list[1], true, 5, true);
        }

        /// <summary>
        /// Adds mathematical symbolic for subindex.
        /// </summary>
        public void AddSubindex(List<string> list)
        {
            CallerManager.CorrectParameters(list, 2);
            CallerManager.ParagraphIsSet(this.activeParagraph);

            //Method ParagraphIsSet checks whether the activeParagraph
            //is null, therefore the warning is unjustified.
            this.AddParameterToParagraph(this.activeParagraph!, list[0], true);
            this.AddParameterToParagraph(this.activeParagraph!, list[1], true, -3, true);
        }

        /// <summary>
        /// Adds inline fraction into text.
        /// </summary>
        public void AddInlineFraction(List<string> list)
        {
            CallerManager.CorrectParameters(list, 2);
            CallerManager.ParagraphIsSet(this.activeParagraph);

            //Method ParagraphIsSet checks whether the activeParagraph
            //is null, therefore the warning is unjustified.
            this.AddParameterToParagraph(this.activeParagraph!, list[0], true, 2);
            this.AddText("/", false);
            this.AddParameterToParagraph(this.activeParagraph!, list[1], true, -2, true);
        }

        /// <summary>
        /// Decodes and adds parameters into their respective function paragraphs.
        /// </summary>
        public Paragraph AddParameterToParagraph(Paragraph paragraph, string parameter,
                                                bool addStraight = false, float rise = 0,
                                                bool addWhiteSpace = false)
        {
            string[] parameterSplit = parameter.Split();

            for (int j = 0; j < parameterSplit.Length; j++)
            {
                CzeTexText textToAdd;
                if (StringFunctions.IsFunction(parameterSplit[j]))
                {
                    string functionName = StringFunctions.GetFunctionName(parameterSplit[j]);
                    int idx = trie.FindFunction(functionName);
                    if (trie.getFunctions[idx] == null)
                    {
                        throw new InvalidParametersException(
                            $"Function {functionName} does not have getFunction.");
                    }

                    textToAdd = ((Func<List<string>, CzeTexText>)trie.getFunctions[idx]!)(new List<string>());
                }
                else
                {
                    textToAdd = this.GetMathText(parameterSplit[j]);
                }

                textToAdd.AddTextRise(rise);
                if (addStraight)
                {
                    this.AddText(textToAdd, addWhiteSpace);
                }
                else
                {
                    paragraph.Add(textToAdd);
                }

                if (j != StringFunctions.LastIndex(parameterSplit))
                {
                    if (addStraight)
                    {
                        this.AddText("");
                    }
                    else
                    {
                        //The iText library has auto cropping enabled after every word in the cell, 
                        //so it is necessary to use non-printable characters instead of spaces
                        paragraph.Add(new CzeTexText("\u00A0"));
                    }
                }
            }

            return paragraph;
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
            cell.SetPadding(0);
            cell.SetPaddingTop(0.5f);

            Paragraph paragraph = new Paragraph();
            paragraph.SetMarginBottom(0);
            paragraph.SetMarginBottom(0);
            paragraph.SetPadding(0);
            paragraph.SetMultipliedLeading(0.8f);

            for (int i = 0; i < repetition; i++)
            {
                this.AddParameterToParagraph(paragraph, text, rise: -(Fonts.defaultSize + 1));
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

            Cell denominator = this.SetFractionPart(list[1]);
            denominator.SetBorderBottom(new SolidBorder(1));
            fraction.AddCell(denominator);

            this.activeParagraph!.Add(fraction);
            this.AddText(" ");
        }
    }
}