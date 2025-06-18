using System;
using iText;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;

namespace CzeTex
{
    /// <summary>
    /// Characterise text based on font, size and special actions.
    /// </summary>
    public class TextCharacteristics
    {
        protected PdfFont font;
        protected uint size;

        public TextCharacteristics(PdfFont font, uint size)
        {
            this.font = font;
            this.size = size;
        }

        public PdfFont Font
        {
            get { return this.font; }
        }

        public uint Size
        {
            get { return this.size; }
        }

        /// <summary>
        /// Sets text's font and font size and returns it.
        /// </summary>
        public CzeTexText Define(CzeTexText text)
        {
            text.SetFont(font);
            text.SetFontSize(size);
            return text;
        }

        /// <summary>
        /// Transforme text base on implementation of this function in different 
        /// layers (instances of class TextCharacteristics and its children).
        /// </summary>
        public virtual CzeTexText Special(CzeTexText text)
        {
            throw new Exception("Special is not defined");
        }

        /// <summary>
        /// Performs when layer is poped from characteristic stack.
        /// Implementation is variable and is based on implmentation of children.
        /// </summary>
        public virtual void End(Document document)
        {
            throw new Exception("End method is not defined");
        }

        /// <summary>
        /// Adds ListItem to the layer.
        /// Implementation is variable and is based on implementation of children.
        /// </summary>
        public virtual void Add(ListItem item)
        {
            throw new Exception("Add method is not defined");
        }

        /// <summary>
        /// Returns ListItem.
        /// Implementation is variable and is based on implementation of children.
        /// </summary>
        public virtual ListItem GetBack()
        {
            throw new Exception("GetBack method is not defined");
        }
    }

    /// <summary>
    /// Parent class for all descendants of TextCharacteristics which have 
    /// implemented Special function.
    /// </summary>
    /// <remarks>
    /// This class is mostly used only for recognition its descendants.
    /// </remarks>
    public class SpecialTextCharacteristics : TextCharacteristics
    {
        public SpecialTextCharacteristics(PdfFont font, uint size) : base(font, size) { }
    }

    /// <summary>
    /// Descendant of TextCharacteristics where all text at 
    /// this layer is underlined.
    /// </summary>
    public class UnderLineText : SpecialTextCharacteristics
    {
        public UnderLineText(PdfFont font, uint size) : base(font, size) { }

        /// <summary>
        /// CzeTexText is set to be underlined.
        /// </summary>
        public override CzeTexText Special(CzeTexText text)
        {
            text.SetFont(font);
            text.SetFontSize(size);
            text.SetUnderline();
            return text;
        }
    }

    /// <summary>
    /// Descendant of TextCharacteristics where all text at
    /// this layer has line through itself.
    /// </summary>
    public class LineThroughText : SpecialTextCharacteristics
    {
        public LineThroughText(PdfFont font, uint size) : base(font, size) { }

        /// <summary>
        /// CzeTexText is modified to have a line through it.
        /// </summary>
        public override CzeTexText Special(CzeTexText text)
        {
            text = Define(text);
            text.SetLineThrough();
            return text;
        }
    }

    /// <summary>
    /// Descendant of TextCharacteristics where all text
    /// at this layer is risen.
    /// </summary>
    public class RisedText : SpecialTextCharacteristics
    {
        private uint rise;

        public RisedText(PdfFont font, uint size, uint rise) : base(font, size)
        {
            this.rise = rise;
        }

        /// <summary>
        /// CzeTexText is modified to appear rised.
        /// </summary>
        public override CzeTexText Special(CzeTexText text)
        {
            text = Define(text);
            text.AddTextRise(rise);
            return text;
        }
    }

    /// <summary>
    /// Parent class of all descendants of TextCharacteristics which
    /// should not be added to the text.
    /// </summary>
    /// <remarks>
    /// This class is mostly used only for recognition its descendants.
    /// </remarks>
    public class DoNotAddTextCharacteristics : TextCharacteristics
    {
        public DoNotAddTextCharacteristics(PdfFont font, uint size) : base(font, size) { }
    }

    /// <summary>
    /// Descendant of TextCharacteristics class. Represents list block.
    /// </summary>
    public class ListText : DoNotAddTextCharacteristics
    {
        List list;

        public ListText(PdfFont font, uint size) : base(font, size)
        {
            this.list = new List();
        }

        /// <summary>
        /// Adds ListItem to list.
        /// </summary>
        public void AddItem(ListItem item)
        {
            list.Add(item);
        }

        /// <summary>
        /// Adds list to the document.
        /// </summary>
        public override void End(Document document)
        {
            document.Add(list);
        }

        /// <summary>
        /// Adds ListItem to list.
        /// </summary>
        public override void Add(ListItem item)
        {
            this.list.Add(item);
        }
    }

    /// <summary>
    /// Descendant of TextCharacteristics class. 
    /// Represents list item in document.
    /// </summary>
    public class ListItemText : DoNotAddTextCharacteristics
    {
        ListItem item;
        Paragraph paragraph;

        public ListItemText(PdfFont font, uint size) : base(font, size)
        {
            this.item = new ListItem();
            this.paragraph = new Paragraph();
            this.paragraph.SetMarginTop(0);
            this.paragraph.SetMarginBottom(0);
        }

        /// <summary>
        /// Adds text to the list item.
        /// </summary>
        public override CzeTexText Special(CzeTexText text)
        {
            text = Define(text);
            paragraph.Add(text);
            paragraph.Add(Define(new CzeTexText(" ")));

            return text;
        }

        /// <summary>
        /// Returns list item.
        /// </summary>
        public override ListItem GetBack()
        {
            item.Add(paragraph);
            return this.item;
        }
    }

    /// <summary>
    /// Wrapper for Text class which provides more functions.
    /// </summary>
    public class CzeTexText : Text
    {
        //Unfortunately the iText library uses floating-point numbers to 
        //representation rise. Therefore even though I use non 
        //floating-point numbers here, they will be casted.
        protected float rise;

        public CzeTexText(string text) : base(text)
        {
            this.rise = 0;
        }

        public float Rise
        {
            set { this.rise = value; }
            get { return this.rise; }
        }

        /// <summary>
        /// Adds to the text rise.
        /// </summary>
        public CzeTexText AddTextRise(float rise)
        {
            return this.SetTextRise(this.rise + rise);
        }

        /// <summary>
        /// Sets the text rise.
        /// </summary>
        public override CzeTexText SetTextRise(float rise)
        {
            this.rise = rise;
            base.SetTextRise(this.rise);
            return this;
        }

        /// <summary>
        /// Sets font size.
        /// </summary>
        public override CzeTexText SetFontSize(float size)
        {
            base.SetFontSize(size);
            return this;
        }

        /// <summary>
        /// Sets text font.
        /// </summary>
        public override CzeTexText SetFont(PdfFont font)
        {
            base.SetFont(font);
            return this;
        }
    }
}