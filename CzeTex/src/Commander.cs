using System;
using System.Collections.Generic;

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
}