using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using iText.Layout.Element;

namespace CzeTex
{
    /// <summary>
    /// Generic node of one-way link list.
    /// </summary>
    public class Node<T>
    {
        private T value;
        private Node<T>? next;

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

    /// <summary>
    /// Generic stack.
    /// </summary>
    public class Stack<T>
    {
        protected uint counter;
        protected Node<T>? head;

        public Stack()
        {
            counter = 0;
        }

        /// <summary>
        /// Adds value to the stack.
        /// </summary>
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

        /// <summary>
        /// Removes node from top of stack and returns its value.
        /// </summary>
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

        /// <summary>
        /// Returns node at the top of stack.
        /// </summary>
        public Node<T> TopNode()
        {
            if (this.head == null)
            {
                throw new Exception("Stack is empty!");
            }

            return this.head;
        }

        /// <summary>
        /// Returns value of node at the top of stack.
        /// </summary>
        public T Top()
        {
            return TopNode().Value;
        }
    }

    /// <summary>
    /// Universal CzeTex exception class.
    /// </summary>
    public class CzeTexException : Exception
    {
        public CzeTexException(string message) : base(message) { }
    }

    /// <summary>
    /// Class for JSON related exceptions.
    /// </summary>
    public class JSONLoaderException : CzeTexException
    {
        public JSONLoaderException(string message) : base(message) { }
    }

    /// <summary>
    /// Class for command line arguments related exceptions.
    /// </summary>
    public class InvalidArgumentsException : CzeTexException
    {
        public InvalidArgumentsException() : base("Command line arguments are invalid") { }
    }

    /// <summary>
    /// Class for CzeTex functions related exceptions.
    /// </summary>
    public class InvalidNumberOfParametersException : CzeTexException
    {
        public InvalidNumberOfParametersException(string message) : base(message) { }
    }

    /// <summary>
    /// Class for Trie related exceptions.
    /// </summary>
    public class TrieExceptions : CzeTexException
    {
        public TrieExceptions(string message) : base(message) { }
    }

    /// <summary>
    /// Class for parameters related exceptions.
    /// </summary>
    public class InvalidParametersException : CzeTexException
    {
        public InvalidParametersException(string message) : base(message) { }
    }

    /// <summary>
    /// Class for removing from paragraph related exceptions.
    /// </summary>
    public class RemovingFromParagraphException : CzeTexException
    {
        public RemovingFromParagraphException(string message) : base(message) { }
    }

    /// <summary>
    /// Class for incorrect adding to paragraph related exceptions.
    /// </summary>
    public class AddingToNonExistingParagraphException : CzeTexException {
        public AddingToNonExistingParagraphException(string message) : base(message) { }
    }

    /// <summary>
    /// Class of functions related to work with strings.
    /// </summary>
    public static class StringFunctions
    {
        /// <summary>
        /// Returns whether the text satisfy criteria for being CzeTex function.
        /// </summary>
        public static bool IsFunction(string text)
        {
            if (text.Length == 0)
            {
                return false;
            }

            if (text[0] == '/')
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns part of string that is name of function.
        /// </summary>
        public static string GetFunctionName(string word)
        {
            int end = 0;

            while (end != word.Length && word[end] != '(')
            {
                end++;
            }

            return word[1..end];
        }

        /// <summary>
        /// Returns last character of string.
        /// </summary>
        public static char LastChar(string word)
        {
            return word[word.Length - 1];
        }

        /// <summary>
        /// Returns last index of string.
        /// </summary>
        public static int LastIndex(string word)
        {
            return word.Length - 1;
        }

        /// <summary>
        /// Returns last index of generic list.
        /// </summary>
        public static int LastIndex<T>(List<T> list)
        {
            return list.Count - 1;
        }

        /// <summary>
        /// Returns last index of string array.
        /// </summary>
        public static int LastIndex(string[] list)
        {
            return list.Length - 1;
        }
    }

    /// <summary>
    /// Class for functions checking CzeTex functions.
    /// </summary>
    public static class CallerManager
    {
        /// <summary>
        /// Checks whether the calling function received correct number of parameters.
        /// </summary>
        public static void CorrectParameters(List<string> list, int count,
                                            [CallerMemberName] string callerFunction = "")
        {
            if (list.Count != count)
            {
                throw new InvalidNumberOfParametersException(
                    $"Function {callerFunction} should have {count} parameters not {list.Count}.");
            }
        }

        /// <summary>
        /// Checks whether parameters are integers.
        /// </summary>
        public static void IsParameterUint(List<string> list,
                                        [CallerMemberName] string callerFunction = "")
        {
            foreach (string parameter in list)
            {
                if (!uint.TryParse(parameter, out uint n))
                {
                    throw new InvalidParametersException(
                        $"Parameter of function {callerFunction} should be a number");
                }
            }
        }

        /// <summary>
        /// Checks whether paragraph exists.
        /// </summary>
        public static void ParagraphIsSet(Paragraph? paragraph,
                                        [CallerMemberName] string callerFunction = "")
        {
            if (paragraph == null)
            {
                throw new AddingToNonExistingParagraphException(
                    $"Paragraph needs to be set before calling {callerFunction}");
            }
        }
    }

    /// <summary>
    /// Class for dynamically generating PDF functions.
    /// </summary>
    public class FunctionGeneratorForPDF
    {
        PDF pdf;

        public FunctionGeneratorForPDF(PDF pdf)
        {
            this.pdf = pdf;
        }

        /// <summary>
        /// Returns dynamically generated AddSign function.
        /// </summary>
        public Action<List<string>> CreateAddSignFunction(string sign, int numberOfParameters = 0)
        {
            return (List<string> list) => pdf.AddSign(sign, list, numberOfParameters);
        }

        /// <summary>
        /// Returns dynamically generated GetSign function.
        /// </summary>
        public Func<List<string>, CzeTexText> CreateGetSignFunction(string sign,
                                                                    int numberOfParameters = 0)
        {
            return (List<string> list) => pdf.GetSign(sign, list, numberOfParameters);
        }
    }
}