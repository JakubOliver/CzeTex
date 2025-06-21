using System;
using iText.Kernel.Font;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace CzeTex
{
    /// <summary>
    /// Stack for text generation.
    /// </summary>
    /// <remarks>
    /// The appearance of the generated text is determined by the top layer of 
    /// the CharacteristicStack (font, size, special mutation of text etc.).
    /// <para>
    /// A base layer with default font, size and without any special 
    /// characteristic is pushed at the beginning.
    /// </para>
    /// <para>
    /// If function that push to the stack provide only some 
    /// characteristics then the remaining characteristics are 
    /// copied from layer below. 
    /// </para>
    /// </remarks>
    public class CharacteristicsStack : Stack<TextCharacteristics>
    {
        private Document document;
        private bool usingSans;

        public CharacteristicsStack(Document document) : base()
        {
            this.document = document;
            this.usingSans = Fonts.usingSans;
        }

        /// <summary>
        /// Adds new layer to the characteristic stack base 
        /// on new font and font size.
        /// </summary>
        public void Push(
            PdfFont font,
            uint size,
            TextAlignment alignment
        )
        {
            Push(new TextCharacteristics(font, size, alignment));
        }

        /// <summary>
        /// Adds new layer to the characteristic stack based only on new font.
        /// </summary>
        public void Push(PdfFont font)
        {
            if (this.counter != 0)
            {
                Push(new TextCharacteristics(
                    font,
                    Top().Size,
                    Top().Alignment
                ));
            }
            else
            {
                Push(new TextCharacteristics(
                    font,
                    Fonts.defaultSize,
                    Fonts.defaultAlignment
                ));
            }
        }

        /// <summary>
        /// Adds new layer to the characteristic stack base only on new size of font.
        /// </summary>
        public void Push(uint size)
        {
            if (this.counter != 0)
            {
                Push(new TextCharacteristics(
                    Top().Font,
                    size,
                    Top().Alignment
                ));
            }
            else
            {
                Push(new TextCharacteristics(
                    Fonts.sansDefaultFont,
                    size,
                    Fonts.defaultAlignment
                ));
            }
        }

        /// <summary>
        /// Adds new layer to the characteristics stack base only on new alignment of font.
        /// </summary>
        public void Push(TextAlignment alignment)
        {
            if (this.counter != 0)
            {
                Push(new TextCharacteristics(
                    Top().Font,
                    Top().Size,
                    alignment
                ));
            }
            else
            {
                Push(new TextCharacteristics(
                    Fonts.sansDefaultFont,
                    Fonts.defaultSize,
                    alignment
                ));
            }
        }

        /// <summary>
        /// Changes serif of text.
        /// </summary>
        private void ChangeSerif()
        {
            Sans = !Sans;
        }

        /// <summary>
        /// Removes layer from the top of the stack and returns it.
        /// </summary>
        /// <remarks>
        /// If the top layer has some ending function (operation which should be
        /// called at the end of layer) then this function is called
        /// in this block.
        /// </remarks>
        public override TextCharacteristics Pop()
        {
            if (Top() is ListItemText)
            {
                //This warning is unjustified because first condition checks 
                // for this error. So if TopNode().Next == null, then the 
                // rest will not be even evaluated.
                if (TopNode().Next == null ||
                    TopNode().Next!.Value is not ListText)
                {
                    throw new Exception("List should comes before List item");
                }

                //This warning is also unjustified based on the same reasoning.
                TopNode().Next!.Value.Add(Top().GetBack());
            }

            if (Top() is ListText)
            {
                Top().End(document);
            }

            if (Top() is SerifTextCharacteristics)
            {
                this.ChangeSerif();
            }

            return base.Pop();
        }

        /// <summary>
        /// Checks whether the fonts is in stack.
        /// </summary>
        public bool IsFontInStack(PdfFont font)
        {
            Node<TextCharacteristics>? node = this.head;

            while (node != null)
            {
                if (node.Value.Font == font)
                {
                    return true;
                }

                node = node.Next;
            }

            return false;
        }

        public PdfFont Font
        {
            get { return Top().Font; }
        }

        public uint Size
        {
            get { return Top().Size; }
        }

        public TextAlignment Alignment
        {
            get { return Top().Alignment; }
        }
        
        public bool Sans
        {
            get { return this.usingSans; }
            set { this.usingSans = value; }
        }
    }
}