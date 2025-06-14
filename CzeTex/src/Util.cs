using System;
using System.Diagnostics.Metrics;

using iText;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace CzeTex
{
    public class TextCharacteristics
    {
        PdfFont font;
        uint size;

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

        public T Pop()
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

        public T Top()
        {
            if (this.head == null)
            {
                throw new Exception("Staci is empty!");
            }

            return this.head.Value;
        }
    }

    public class CharacteristicsStack : Stack<TextCharacteristics>
    {
        const uint defaultSize = 12;
        PdfFont defaultFont = PdfFontFactory.CreateFont("src/open-sans/OpenSans-Regular.ttf", PdfEncodings.IDENTITY_H); //dáv všechny fondy na jedno místo a z něho se bude brát
        public CharacteristicsStack() : base() { }

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

        public PdfFont Font
        {
            get { return Top().Font; }
        }

        public uint Size
        {
            get { return Top().Size; }
        }
    }
}