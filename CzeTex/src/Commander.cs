using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using iText.Layout.Element;

namespace CzeTex
{
    /// <summary>
    /// Is a centerpoint of CzeTex project, manages calls to another class and function.
    /// </summary>
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

        public Commander(string textFile, string setupFile = "Setup.json")
        {
            file = new Files(textFile);
            content = file.LoadFile();
            string basename = file.GetBaseName();

            trie = new Trie();
            pdf = new PDF(basename, trie);
            setupLoader = new SetupLoader(trie, pdf, setupFile);

            lookingForParameters = false;
            parameteres = new List<string>();
            parameter = new List<string>();
            ReadContent();
        }

        /// <summary>
        /// Parses the content of a file and detects functions in text.
        /// </summary>
        public void ReadContent()
        {
            for (contentIndex = 0; contentIndex < content.Length; contentIndex++)
            {
                words = content[contentIndex].Split(' ');

                for (wordsIndex = 0; wordsIndex < words.Length; wordsIndex++)
                {
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

                        int idx = trie.FindFunction(functionName);

                        if (StringFunctions.LastChar(words[wordsIndex]) == '(')
                        {
                            parameteres = new List<string>();
                            parameter = new List<string>();

                            lookingForParameters = true;
                            while (lookingForParameters)
                            {
                                ReadParameter();
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

        /// <summary>
        /// Reads the parameters of functions in text.
        /// </summary>
        private void ReadParameter()
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

    /// <summary>
    /// Represents one entry in the setup JSON file.
    /// </summary>
    public class JsonEntry
    {
        public string? addFunction { get; set; }
        public string? getFunction { get; set; }
        public bool dynamicallyGenerated { get; set; }
        public string? sign { get; set; }
    }

    /// <summary>
    /// Loads and maps text functions to their corresponding code methods.
    /// </summary>
    public class SetupLoader
    {
        private string path;
        private Dictionary<string, JsonEntry> mapping;
        private Trie trie;
        private PDF pdf;
        private FunctionGeneratorForPDF generator;

        public SetupLoader(Trie trie, PDF pdf, string path = "Setup.json")
        {
            this.trie = trie;
            this.pdf = pdf;
            this.generator = new FunctionGeneratorForPDF(pdf);
            this.path = path;

            mapping = ReadJson();
            AddFunctions();
        }

        /// <summary>
        /// Reads JSON setup file.
        /// </summary>
        private Dictionary<string, JsonEntry> ReadJson()
        {
            string content = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Dictionary<string, JsonEntry>>(content)!;
        }

        /// <summary>
        /// Generates function with parameters from JSON entry and stores it into tree structure.
        /// </summary>
        private void AddDynamicallyGeneratedFunction(KeyValuePair<string, JsonEntry> entry)
        {
            if (entry.Value.sign == null)
            {
                throw new JSONLoaderException($"CzeTex function with name {entry.Key} should have assigned a sign");
            }

            //Looking for an attribute in the Signs class with the same name as entry.Value.sign
            FieldInfo? attribute = typeof(Signs).GetField(entry.Value.sign);

            if (attribute == null || attribute.FieldType != typeof(string))
            {
                throw new JSONLoaderException($"CzeTex function with name {entry.Key} should have corrent a name of sign not {entry.Value.sign}");
            }

            string? sign = attribute.GetValue(null) as string;

            if (sign == null)
            {
                throw new JSONLoaderException($"CzeTex function with name {entry.Key} should have been not null a name of sign not {entry.Value.sign}");
            }

            //Adds dynamically generated add and get functions into the tree structure
            trie.AddFunction(entry.Key,
                            (Action<List<string>>)this.generator.CreateAddSignFunction(sign),
                            (Func<List<string>, Text>)this.generator.CreateGetSignFunction(sign));
        }

        /// <summary>
        /// Finds function with corresponding name from JSON entry and stores it into tree structure.
        /// </summary>
        private void AddExistingFunction(KeyValuePair<string, JsonEntry> entry)
        {
            //Looking for a method in the PDF class with the same name as entry.value.addFunction
            MethodInfo? method = typeof(PDF).GetMethod(entry.Value.addFunction!);

            if (method == null)
            {
                throw new JSONLoaderException($"Method with name {entry.Value.addFunction} does not exists");
            }

            Delegate addMethod = Delegate.CreateDelegate(typeof(Action<List<string>>), pdf, method);

            if (entry.Value.getFunction != null)
            {
                //Looking for a method in the PDF class wit the same name as entry.Value.getFunction
                MethodInfo? getMethod = typeof(PDF).GetMethod(entry.Value.getFunction);

                if (getMethod == null)
                {
                    throw new JSONLoaderException($"Method with name {entry.Value.getFunction} does not exists");
                }

                //Adds add and get method into the tree structure
                Delegate getFunction = Delegate.CreateDelegate(typeof(Func<List<string>, Text>), pdf, getMethod);
                trie.AddFunction(entry.Key,
                                (Action<List<string>>)addMethod,
                                (Func<List<string>, Text>)getFunction);
            }
            else
            {
                //Adds add method into the tree structure (get function is implicitly set as null)
                trie.AddFunction(entry.Key,
                                (Action<List<string>>)addMethod);
            }
        }

        /// <summary>
        /// Maps text functions to their corresponding code methods and stores this mapping in a tree structure.
        /// </summary>
        private void AddFunctions()
        {
            foreach (KeyValuePair<string, JsonEntry> entry in mapping)
            {
                if (entry.Value.dynamicallyGenerated)
                {
                    this.AddDynamicallyGeneratedFunction(entry);
                }
                else
                {
                    this.AddExistingFunction(entry);
                }
            }
        }
    }
}