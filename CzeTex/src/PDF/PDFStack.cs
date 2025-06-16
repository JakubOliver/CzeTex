using System;
using iText;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using Org.BouncyCastle.Crypto.Modes;

namespace CzeTex
{
    public class CharacteristicsStack : Stack<TextCharacteristics>
    {
        private Document document;
        public CharacteristicsStack(Document document) : base()
        {
            this.document = document;
        }

        public void Push(PdfFont font)
        {
            if (this.counter != 0)
            {
                Push(new TextCharacteristics(font, Top().Size));
            }
            else
            {
                Push(new TextCharacteristics(font, Fonts.defaultSize));
            }
        }

        public void Push(uint size)
        {
            if (this.counter != 0)
            {
                Push(new TextCharacteristics(Top().Font, size));
            }
            else
            {
                Push(new TextCharacteristics(Fonts.defaultFont, size));
            }
        }

        public override TextCharacteristics Pop()
        {
            if (Top() is ListItemText)
            {
                if (TopNode().Next == null || TopNode().Next?.Value is not ListText)
                {
                    throw new Exception("List should comes before Listitem");
                }

                TopNode().Next?.Value.Add(Top().GetBack()); //list item musí vždy mít po sobě list, takže nedojde k null dereference
            }

            if (Top() is ListText)
            {
                Top().End(document);
            }

            return base.Pop();
        }

        public PdfFont Font
        {
            get { return Top().Font; }
        }

        public uint Size
        {
            get { return Top().Size; }
        }

        public bool IsSpecial()
        {
            return Top().IsSpecial();
        }
    }
}