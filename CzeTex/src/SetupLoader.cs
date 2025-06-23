using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using iText.Layout.Element;

namespace CzeTex
{
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
            Files file = new Files(path);
            string content = file.LoadFileIntoOneString();
            return JsonSerializer.Deserialize<Dictionary<string, JsonEntry>>(content)!;
        }

        /// <summary>
        /// Generates function with parameters from JSON entry 
        /// and stores it into tree structure.
        /// </summary>
        private void AddDynamicallyGeneratedFunction(KeyValuePair<string, JsonEntry> entry)
        {
            if (entry.Value.sign == null)
            {
                throw new JSONLoaderException($"CzeTex function with name {entry.Key}" +
                    $" should have assigned a sign!");
            }

            //Looking for an attribute in the Signs class 
            // with the same name as entry.Value.sign
            FieldInfo? attribute = typeof(Signs).GetField(entry.Value.sign);

            if (attribute == null || attribute.FieldType != typeof(string))
            {
                throw new JSONLoaderException($"CzeTex function with name {entry.Key}" +
                    $" should have correct name of sign not {entry.Value.sign}!");
            }

            string? sign = attribute.GetValue(null) as string;

            if (sign == null)
            {
                throw new JSONLoaderException($"CzeTex function with name {entry.Key}" +
                    $" should have been not null name of sign not {entry.Value.sign}!");
            }

            //Adds dynamically generated add and get functions into the tree structure
            trie.AddFunction(
                entry.Key,
                (Action<List<string>>)this.generator.CreateAddSignFunction(sign),
                (Func<List<string>, Text>)this.generator.CreateGetSignFunction(sign)
            );
        }

        /// <summary>
        /// Finds function with corresponding name from JSON entry 
        /// and stores it into tree structure.
        /// </summary>
        private void AddExistingFunction(KeyValuePair<string, JsonEntry> entry)
        {
            //Looking for a method in the PDF class with 
            // the same name as entry.value.addFunction
            MethodInfo? method = typeof(PDF).GetMethod(entry.Value.addFunction!);

            if (method == null)
            {
                throw new JSONLoaderException(
                    $"Method with name {entry.Value.addFunction} does not exists!");
            }

            Delegate addMethod = Delegate.CreateDelegate(
                typeof(Action<List<string>>),
                pdf,
                method
            );

            if (entry.Value.getFunction != null)
            {
                //Looking for a method in the PDF class with 
                //the same name as entry.Value.getFunction
                MethodInfo? getMethodInfo =
                    typeof(PDF).GetMethod(entry.Value.getFunction);

                if (getMethodInfo == null)
                {
                    throw new JSONLoaderException(
                        $"Method with name {entry.Value.getFunction} does not exists!");
                }

                Delegate getMethod = Delegate.CreateDelegate(
                    typeof(Func<List<string>, Text>),
                    pdf,
                    getMethodInfo
                );

                //Adds add and get method into the tree structure
                trie.AddFunction(
                    entry.Key,
                    (Action<List<string>>)addMethod,
                    (Func<List<string>, Text>)getMethod
                );
            }
            else
            {
                //Adds add method into the tree structure 
                //(get function is implicitly set as null)
                trie.AddFunction(
                    entry.Key,
                    (Action<List<string>>)addMethod
                );
            }
        }

        /// <summary>
        /// Maps text functions to their corresponding code 
        /// methods and stores this mapping in a tree structure.
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