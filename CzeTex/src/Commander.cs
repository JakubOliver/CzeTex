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

        private string[] content;

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

            ReadContent();
        }

        public void ReadContent()
        {
            string[] words;
            for (int i = 0; i < content.Length; i++)
            {
                words = content[i].Split(' ');

                for (int j = 0; j < words.Length; j++)
                {
                    //Console.WriteLine(word);
                    if (words[j].Length == 0)
                    {
                        continue;
                    }

                    if (words[j][0] != '/')
                    {
                        pdf.AddText(words[j]);
                    }
                    else
                    {
                        string functionName = FunctionName(words[j]);

                        //TODO: vyceslovne parametry
                        int idx = trie.FindFunction(functionName);
                        //System.Console.WriteLine($"{functionName} {idx}");
                        ((Action)trie.Functions[idx])();
                    }
                }
            }

            pdf.Export();
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

                trie.AddFunction(entry.Key, (Action)Delegate.CreateDelegate(typeof(Action), pdf, method));
            }
        }
    }
}