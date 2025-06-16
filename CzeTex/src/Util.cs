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
        protected Node<T>? head;

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
                throw new Exception("Stack is empty!");
            }

            return this.head;
        }

        public T Top()
        {
            return TopNode().Value;
        }
    }
}