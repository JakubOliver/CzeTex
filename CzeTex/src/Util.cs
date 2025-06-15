using System;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Runtime.InteropServices;
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

    public class UnderLineText : TextCharacteristics
    {
        public UnderLineText(PdfFont font, uint size) : base(font, size)
        {
            this.special = true;
        }

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

    public class LineThroughText : TextCharacteristics
    {
        public LineThroughText(PdfFont font, uint size) : base(font, size)
        {
            this.special = true;
        }

        public override Text Special(Text text)
        {
            text = Define(text);
            text.SetLineThrough();
            return text;
        }
    }

    public class ListText : TextCharacteristics
    {
        List list;

        public ListText(PdfFont font, uint size) : base(font, size)
        {
            this.special = true;
            this.isList = true;
            this.doNotAdd = true;
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

    public class ListItemText : TextCharacteristics
    {
        ListItem item;
        Paragraph paragraph;

        public ListItemText(PdfFont font, uint size) : base(font, size)
        {
            this.item = new ListItem();
            this.paragraph = new Paragraph();
            this.special = true;
            this.isListItem = true;
            this.doNotAdd = true;
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

    public class Node<T>
    {
        T value;
        Node<T>? next;

        public Node(T value)
        {
            this.value = value;
        }

        public Node(T value, Node<T> next)
        {
            this.value = value;
            this.next = next;
        }

        public T Value
        {
            get { return this.value; }
        }

        public Node<T>? Next
        {
            get { return this.next; }
        }
    }
    public class Stack<T>
    {
        protected uint counter;
        private Node<T>? head;

        public Stack()
        {
            counter = 0;
        }

        public void Push(T value)
        {
            if (head == null)
            {
                head = new Node<T>(value);
            }
            else
            {
                head = new Node<T>(value, head);
            }

            counter++;
        }

        public virtual T Pop()
        {
            if (this.head == null)
            {
                throw new Exception("Stack is empty!");
            }

            Node<T> active = this.head;
            this.head = this.head.Next;

            counter--;
            return active.Value;
        }

        public Node<T> TopNode()
        {
            if (this.head == null)
            {
                throw new Exception("Staci is empty!");
            }

            return this.head;
        }

        public T Top()
        {
            return TopNode().Value;
        }
    }

    public class CharacteristicsStack : Stack<TextCharacteristics>
    {
        const uint defaultSize = 12;
        PdfFont defaultFont = PdfFontFactory.CreateFont("src/open-sans/OpenSans-Regular.ttf", PdfEncodings.IDENTITY_H); //dáv všechny fondy na jedno místo a z něho se bude brát
        Document document;
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
                Push(new TextCharacteristics(font, defaultSize));
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
                Push(new TextCharacteristics(defaultFont, size));
            }
        }

        public override TextCharacteristics Pop()
        {
            if (Top().IsListItem())
            {
                TopNode().Next?.Value.Add(Top().GetBack()); //list item musí vždy mít po sobě list, takže nedojde k null dereference
            }

            if (Top().IsList())
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