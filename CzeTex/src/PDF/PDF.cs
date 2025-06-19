using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;

namespace CzeTex{
    /// <summary>
    /// Creates and edits a PDF document.
    /// </summary>
    public partial class PDF
    {
        private PdfWriter writer;
        private PdfDocument pdf;
        private Document document;

        private CharacteristicsStack stack;
        private Trie trie;

        private Paragraph? activeParagraph;
        public PDF(string outputPath, Trie trie)
        {
            writer = new PdfWriter(outputPath);
            pdf = new PdfDocument(writer);
            document = new Document(pdf);
            document.SetFont(Fonts.defaultFont);

            stack = new CharacteristicsStack(document);
            stack.Push(Fonts.defaultFont);

            this.trie = trie;
        }

        /// <summary>
        /// Ends last part of text and exports pdf.
        /// </summary>
        public void Export()
        {
            if (this.activeParagraph != null)
            {
                this.AddParagraph();
            }

            this.document.Close();
        }

        /// <summary>
        /// Creates new paragraph.
        /// </summary>
        public void CreateParagraph(List<string> list)
        {
            if (this.activeParagraph != null)
            {
                this.AddParagraph();
            }

            this.activeParagraph = new Paragraph();
            this.activeParagraph.SetMultipliedLeading(Fonts.defaultSpacing);
        }

        /// <summary>
        /// Adds new paragraph.
        /// </summary>
        public void AddParagraph()
        {
            this.document.Add(this.activeParagraph);
        }

        /// <summary>
        /// Starts title part (pushes title layer to stack).
        /// </summary>
        public void AddTitle(List<string> list)
        {
            foreach (string x in list)
            {
                System.Console.WriteLine(x);
            }
            CallerManager.CorrectParameters(list, 0);

            this.CreateParagraph(list);
            this.stack.Push(Fonts.defaultTitleSize);
        }

        /// <summary>
        /// Starts subTitle part (pushes subTitle layer to stack).
        /// </summary>
        public void AddSubTitle(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.CreateParagraph(list);
            this.stack.Push(Fonts.defaultSubTitleSize);
        }

        /// <summary>
        /// Starts subSubTitle part (pushes subSubTitle layer to stack).
        /// </summary>
        public void AddSubSubTitle(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.CreateParagraph(list);
            this.stack.Push(Fonts.defaultSubSubTitleSize);
        }

        /// <summary>
        /// Starts title part with font size based on parameters.
        /// </summary>
        public void GenericTitle(List<string> list)
        {
            CallerManager.CorrectParameters(list, 1);
            CallerManager.IsParameterUint(list);

            uint size = Convert.ToUInt32(list[0]);
            size = Fonts.defaultTitleSize - (size * 2);

            if (size < Fonts.defaultSize)
            {
                throw new InvalidParametersException(
                    "Generic title is too small");
            }

            list.RemoveAt(0);

            this.CreateParagraph(list);
            this.stack.Push(size);
        }

        /// <summary>
        /// Adds text to document.
        /// </summary>
        public void AddText(string text, bool addWhiteSpace = true)
        {
            this.AddText(new CzeTexText(text), addWhiteSpace);
        }

        /// <summary>
        /// Adds text to document.
        /// </summary>
        public void AddText(CzeTexText text, bool addWhiteSpace = true)
        {
            if (this.activeParagraph == null)
            {
                throw new Exception("No active paragraph. Create a paragraph first.");
            }

            if (this.stack.Top() is DoNotAddTextCharacteristics)
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
                this.activeParagraph.Add(this.stack.Top().Define(new CzeTexText(" ")));
            }
        }

        /// <summary>
        /// Starts bold cursive text part (pushes bold-cursive layer to stack).
        /// </summary>
        public void AddBoldCursiveText(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.stack.Push(Fonts.boldCursiveFont);
        }

        /// <summary>
        /// Starts bold text part (pushes bold layer to stack).
        /// </summary>
        public void AddBoldText(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            if (this.stack.IsFontInStack(Fonts.cursiveFont))
            {
                this.AddBoldCursiveText(list);
            }
            else
            {
                this.stack.Push(Fonts.boldFont);
            }
        }

        /// <summary>
        /// Starts cursive text part (pushes cursive layer to stack).
        /// </summary>
        public void AddCursiveText(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            if (this.stack.IsFontInStack(Fonts.boldFont))
            {
                this.AddBoldCursiveText(list);
            }
            else
            {
                this.stack.Push(Fonts.cursiveFont);
            }
        }

        /// <summary>
        /// Adds / character to document.
        /// </summary>
        public void AddSlash(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.AddText("/");
        }

        /// <summary>
        /// Removes last added characteristics (pops from stack).
        /// </summary>
        public void RemoveFont(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.stack.Pop();
        }

        /// <summary>
        /// Adds new page to the document.
        /// </summary>
        public void AddNewPage(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.CreateParagraph(list);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
        }

        /// <summary>
        /// Starts underline text part (pushes underline text layer to stack).
        /// </summary>
        public void AddUnderLineText(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.stack.Push(new UnderLineText(stack.Font, stack.Size));
        }

        /// <summary>
        /// Starts line through text part (pushes line through layer to stack).
        /// </summary>
        public void AddLineThroughText(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.stack.Push(new LineThroughText(stack.Font, stack.Size));
        }

        /// <summary>
        /// Creates list block in document
        /// </summary>
        public void AddList(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.CreateParagraph(list);
            this.stack.Push(new ListText(stack.Font, stack.Size));
        }

        /// <summary>
        /// Starts list item part text.
        /// </summary>
        public void AddListItem(List<string> list)
        {
            CallerManager.CorrectParameters(list, 0);

            this.stack.Push(new ListItemText(stack.Font, stack.Size));
        }

        /// <summary>
        /// Adds hyperlink to the text.
        /// </summary>
        public void AddLink(List<string> list)
        {
            CallerManager.CorrectParameters(list, 2);

            CzeTexText text = new CzeTexText(list[0]);
            text.SetFontColor(Fonts.linkColor);
            text.SetUnderline();
            text.SetAction(PdfAction.CreateURI(list[1]));

            this.AddText(text);
        }
    }
}