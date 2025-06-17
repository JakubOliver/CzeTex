using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using iText.Kernel.Pdf.Annot;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace CzeTex
{
    public class Commander
    {
        private PDF pdf;
        private Files file;
        private Trie trie;
        private SetupLoader setupLoader;

        private List<string>? parameteres;
        private bool lookingForParameters;

        private string[] content;
        private int contentIndex;
        private string[]? words;
        private int wordsIndex;

        public Commander(string path)
        {
            file = new Files(path);
            content = file.LoadFile();
            string basename = file.GetBaseName();

            pdf = new PDF(basename);
            trie = new Trie();
            setupLoader = new SetupLoader(trie, pdf);

            /*
            //toto nahradit hledanim v Trie plus nacitani kodu z JSON
            trie.AddFunction("bold", pdf.AddBoldText);
            trie.AddFunction("cursive", pdf.AddCursiveText);
            trie.AddFunction("title", pdf.AddTitle);
            trie.AddFunction("section", pdf.CreateParagraph);
            trie.AddFunction("slash", pdf.AddSlash);
            trie.AddFunction("newpage", pdf.AddNewPage);
            trie.AddFunction("underline", pdf.AddUnderLineText);
            trie.AddFunction("linethrough", pdf.AddLineThroughText);
            trie.AddFunction("list", pdf.AddList);
            trie.AddFunction("listitem", pdf.AddListItem);
            trie.AddFunction("x", pdf.RemoveFont);
            */

            lookingForParameters = false;
            ReadContent();
        }

        public void ReadContent()
        {
            for (contentIndex = 0; contentIndex < content.Length; contentIndex++)
            {
                words = content[contentIndex].Split(' ');

                for (wordsIndex = 0; wordsIndex < words.Length; wordsIndex++)
                {
                    //Console.WriteLine(word);
                    if (words[wordsIndex].Length == 0)
                    {
                        continue;
                    }

                    if (words[wordsIndex][0] != '/')
                    {
                        pdf.AddText(words[wordsIndex]);
                    }
                    else
                    {
                        string functionName = FunctionName(words[wordsIndex]);

                        //TODO: vyceslovne parametry
                        int idx = trie.FindFunction(functionName);
                        //System.Console.WriteLine($"{functionName} {idx}");

                        parameteres = new List<string>();
                        //pridat moznost nacitat parametry ve formátu /add(ahoj)

                        if (LastChar(words[wordsIndex]) == '(')
                        {
                            lookingForParameters = true;
                            while (lookingForParameters)
                            {
                                parameteres.Add(ReadWord());
                            }
                        }

                        ((Action<List<string>>)trie.Functions[idx])(parameteres);
                    }
                }
            }

            pdf.Export();
        }

        private string ReadWord()
        {
            if (words == null)
            {
                throw new Exception("Parameters of functions should be at the same line as the function");
            }

            wordsIndex++;
            string word = words[wordsIndex];

            if (LastChar(word) == ')')
            {
                lookingForParameters = false;
            }

            return word.Trim().Trim(')').Trim(',');
        }

        private char LastChar(string word) {
            return word[word.Length - 1];
        }

        private string FunctionName(string word)
        {
            int end = 0;

            while (end != word.Length && word[end] != '(')
            {
                end++;
            }

            return word[1..end];
        }
    }

    public class SetupLoader
    {
        private static string path = "Setup.json";
        private Dictionary<string, string> mapping;
        private Trie trie;
        private PDF pdf;

        public SetupLoader(Trie trie, PDF pdf)
        {
            this.trie = trie;
            this.pdf = pdf;

            mapping = ReadJson();
            AddFunctions();
        }

        private Dictionary<string, string> ReadJson()
        {
            string content = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(content)!;
        }

        private void AddFunctions()
        {
            foreach (KeyValuePair<string, string> entry in mapping)
            {
                MethodInfo? method = typeof(PDF).GetMethod(entry.Value);

                if (method == null)
                {
                    throw new Exception($"Method with name {entry.Value} does not exists");
                }

                trie.AddFunction(entry.Key, (Action<List<string>>)Delegate.CreateDelegate(typeof(Action<List<string>>), pdf, method));
            }
        }
    }
}