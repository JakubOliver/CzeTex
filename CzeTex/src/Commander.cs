using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using iText.Kernel.Pdf.Annot;
using iText.Layout.Element;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace CzeTex
{
    public class Commander
    {
        private PDF pdf;
        private Files file;
        private Trie trie;
        private SetupLoader setupLoader;

        private List<string> parameteres;
        private List<string> parameter;
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

            trie = new Trie();
            pdf = new PDF(basename, trie);
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
            parameteres = new List<string>();
            parameter = new List<string>();
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
                        string functionName = StringFunctions.GetFunctionName(words[wordsIndex]);

                        //TODO: vyceslovne parametry
                        int idx = trie.FindFunction(functionName);
                        //System.Console.WriteLine($"{functionName} {idx}");

                        //pridat moznost nacitat parametry ve formátu /add(ahoj)

                        if (StringFunctions.LastChar(words[wordsIndex]) == '(')
                        {
                            parameteres = new List<string>();
                            parameter = new List<string>();

                            lookingForParameters = true;
                            while (lookingForParameters)
                            {
                                ReadWord();
                            }

                            ((Action<List<string>>)trie.addFunctions[idx])(parameteres);
                        }
                        else
                        {
                            ((Action<List<string>>)trie.addFunctions[idx])(new List<string>());
                        }

                        
                    }
                }
            }

            pdf.Export();
        }

        private void ReadWord()
        {
            wordsIndex++;

            if (words == null || wordsIndex >= words.Length)
            {
                throw new Exception("Parameters of functions should be at the same line as the function");
            }

            string word = words[wordsIndex];

            if (word[0] == ',')
            {
                this.parameteres.Add(string.Join(" ", this.parameter));
                this.parameter = new List<string>();

                if (word.Length == 1)
                {
                    return;
                }
            }

            parameter.Add(word.Trim().Trim(')').Trim(','));

            if (StringFunctions.LastChar(word) == ',')
            {
                this.parameteres.Add(string.Join(" ", this.parameter));
                this.parameter = new List<string>();
            }

            if (StringFunctions.LastChar(word) == ')')
            {
                lookingForParameters = false;
                this.parameteres.Add(string.Join(" ", this.parameter));
            }

            return;
        }
    }

    public class JsonEntry
    {
        public string? addFunction { get; set; }
        public string? getFunction { get; set; }
    }

    public class SetupLoader
    {
        private static string path = "Setup.json";
        private Dictionary<string, JsonEntry> mapping;
        private Trie trie;
        private PDF pdf;

        public SetupLoader(Trie trie, PDF pdf)
        {
            this.trie = trie;
            this.pdf = pdf;

            mapping = ReadJson();
            AddFunctions();
        }

        private Dictionary<string, JsonEntry> ReadJson()
        {
            string content = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Dictionary<string, JsonEntry>>(content)!;
        }

        private void AddFunctions()
        {
            foreach (KeyValuePair<string, JsonEntry> entry in mapping)
            {
                MethodInfo? method = typeof(PDF).GetMethod(entry.Value.addFunction!);

                if (method == null)
                {
                    throw new Exception($"Method with name {entry.Value.addFunction} does not exists");
                }

                Delegate addFunction = Delegate.CreateDelegate(typeof(Action<List<string>>), pdf, method);
                if (entry.Value.getFunction != "null")
                {
                    MethodInfo? getMethod = typeof(PDF).GetMethod(entry.Value.getFunction!);

                    if (getMethod == null)
                    {
                        throw new Exception($"Method with name {entry.Value.getFunction} does not exists");
                    }

                    Delegate getFunction = Delegate.CreateDelegate(typeof(Func<List<string>, Text>), pdf, getMethod);
                    trie.AddFunction(entry.Key, (Action<List<string>>)addFunction, (Func<List<string>, Text>)getFunction);
                }
                else
                {
                    trie.AddFunction(entry.Key, (Action<List<string>>)addFunction);
                }
            }
        }
    }
}