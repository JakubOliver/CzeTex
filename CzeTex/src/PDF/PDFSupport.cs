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
    public class TextCharacteristics
    {
        protected PdfFont font;
        protected uint size;
        protected bool special;
        protected bool isList;
        protected bool isListItem;
        protected bool doNotAdd;

        public TextCharacteristics(PdfFont font, uint size)
        {
            this.font = font;
            this.size = size;

            this.special = false;
            this.isList = false;
            this.isListItem = false;
            this.doNotAdd = false;
        }

        public PdfFont Font
        {
            get { return this.font; }
        }

        public uint Size
        {
            get { return this.size; }
        }

        public bool IsSpecial()
        {
            return this.special;
        }

        public bool IsList()
        {
            return this.isList;
        }

        public bool IsListItem()
        {
            return this.isListItem;
        }

        public bool DoNotAdd()
        {
            return this.doNotAdd;
        }

        public Text Define(Text text)
        {
            text.SetFont(font);
            text.SetFontSize(size);
            return text;
        }

        public virtual Text Special(Text text)
        {
            throw new Exception("Special is not defined");
        }

        public virtual void End(Document document)
        {
            throw new Exception("End method is not defined");
        }

        public virtual void Add(ListItem item)
        {
            throw new Exception("Add method is not defined");
        }

        public virtual ListItem GetBack()
        {
            throw new Exception("GetBack method is not defined");
        }
    }

    public class SpecialTextCharacteristics : TextCharacteristics
    {
        public SpecialTextCharacteristics(PdfFont font, uint size) : base(font, size)
        {
            this.special = true;
        }
    }

    public class UnderLineText : SpecialTextCharacteristics
    {
        public UnderLineText(PdfFont font, uint size) : base(font, size){}

        public override Text Special(Text text)
        {
            text.SetFont(font);
            text.SetFontSize(size);
            text.SetUnderline();
            return text;
        }
    }

    /*
    public class DottedUnderlineText : TextCharacteristics
    {
        public DottedUnderlineText(PdfFont font, uint size) : base(font, size)
        {
            this.special = true;
        }

        public override Text Special(Text text)
        {
            text = Define(text);
            text.SetUnderline(0.5f, -2, new DottedLine());
            return text;
        }
    }
    */

    public class LineThroughText : SpecialTextCharacteristics
    {
        public LineThroughText(PdfFont font, uint size) : base(font, size){}

        public override Text Special(Text text)
        {
            text = Define(text);
            text.SetLineThrough();
            return text;
        }
    }

    public class DoNotAddTextCharacteristics : TextCharacteristics
    {
        public DoNotAddTextCharacteristics(PdfFont font, uint size) : base(font, size)
        {
            this.doNotAdd = true;
        }
    }

    public class ListText : DoNotAddTextCharacteristics
    {
        List list;

        public ListText(PdfFont font, uint size) : base(font, size)
        {
            this.isList = true;
            this.list = new List();
        }

        public void AddItem(ListItem item)
        {
            list.Add(item);
        }

        public override void End(Document document)
        {
            document.Add(list);
        }

        public override void Add(ListItem item)
        {
            this.list.Add(item);
        }
    }

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

            this.isListItem = true;
        }

        public override Text Special(Text text)
        {
            text = Define(text);
            paragraph.Add(text);
            paragraph.Add(Define(new Text(" ")));

            return text;
        }

        public override ListItem GetBack()
        {
            item.Add(paragraph);
            return this.item;
        }
    }
}