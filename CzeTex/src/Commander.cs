using System;
using System.Collections.Generic;

namespace CzeTex
{
    /// <summary>
    /// Class contains constants for Commander class.
    /// </summary>
    public static class CommanderConstants
    {
        public const char specialCharacter = '/';
    }

    /// <summary>
    /// Is a centerpoint of CzeTex project, manages calls 
    /// to another class and function.
    /// </summary>
    public class Commander
    {
        private PDF pdf;
        private Files file;
        private Trie trie;
        private SetupLoader setupLoader;

        private List<string> parameters;
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
            string outputPath = file.GetPDFPath();

            trie = new Trie();
            pdf = new PDF(outputPath, trie);
            setupLoader = new SetupLoader(trie, pdf, setupFile);

            lookingForParameters = false;
            parameters = new List<string>();
            parameter = new List<string>();
            ReadContent();

            pdf.Export();
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

                    if (words[wordsIndex][0] != CommanderConstants.specialCharacter)
                    {
                        pdf.AddText(words[wordsIndex]);
                    }
                    else
                    {
                        string functionName =
                            StringFunctions.GetFunctionName(words[wordsIndex]);

                        int idx = trie.FindFunction(functionName);

                        if (this.HasParametersNext() || this.ReadRestForParameter())
                        {
                            while (lookingForParameters)
                            {
                                ReadParameter();
                            }

                            ((Action<List<string>>)trie.addFunctions[idx])
                                (parameters);
                        }
                        else
                        {
                            ((Action<List<string>>)trie.addFunctions[idx])
                                (new List<string>());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds parameter into the list of parameters.
        /// </summary>
        private void AddParameter(bool reset = false, bool split = false)
        {
            string[] splitted = string.Join(" ", this.parameter).Split(",");

            foreach (string p in splitted)
            {
                this.parameters.Add(p);
            }

            if (reset)
            {
                this.parameter = new List<string>();
            }
        }

        /// <summary>
        /// Resets parameters.
        /// </summary>
        private void ResetParameters()
        {
            parameters = new List<string>();
            parameter = new List<string>();
        }

        /// <summary>
        /// Adds parameter into list of parameters if string ends with comma.
        /// </summary>
        private bool EndWithComma(string word)
        {
            if (StringFunctions.LastChar(word) == ',')
            {
                this.AddParameter(true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds parameter into list of parameters and end looking for
        /// additional parameters if string ends with closing brackets.
        /// </summary>
        private bool EndWithClosingBrackets(string word)
        {
            if (StringFunctions.LastChar(word) == ')')
            {
                lookingForParameters = false;
                this.AddParameter(false, true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether are parameters in the next word.
        /// </summary>
        private bool HasParametersNext()
        {
            if (StringFunctions.LastChar(words![wordsIndex]) != '(')
            {
                return false;
            }

            ResetParameters();
            this.lookingForParameters = true;
            return true;
        }

        /// <summary>
        /// Checks whether are parameters in the same string as function name.
        /// </summary>
        private bool ReadRestForParameter()
        {
            int start = 0;
            //This warning is unjustified because when this method is called
            //we already read part of this word, therefore is not null.
            string word = this.words![this.wordsIndex];
            int length = word.Length;

            while (start < length && word[start] != '(')
            {
                start++;
            }

            if (start == length || word[start + 1] == ')')
            {
                return false;
            }

            start++;

            this.ResetParameters();
            this.parameter.Add(word[start..].Trim(')').Trim(','));

            this.EndWithComma(word);

            if (this.EndWithClosingBrackets(word))
            {
                this.lookingForParameters = false;
            }
            else
            {
                this.lookingForParameters = true;
            }

            return true;
        }

        /// <summary>
        /// Reads the parameters of functions in text.
        /// </summary>
        private void ReadParameter()
        {
            wordsIndex++;

            if (words == null || wordsIndex >= words.Length)
            {
                throw new Exception("Parameters of functions" +
                    "should be at the same line as the function");
            }

            string word = words[wordsIndex];

            if (word.Length == 0)
            {
                return;
            }

            if (word[0] == ',')
            {
                this.AddParameter(true);

                if (word.Length == 1)
                {
                    return;
                }
            }

            parameter.Add(word.Trim().Trim(')').Trim(','));

            this.EndWithComma(word);

            this.EndWithClosingBrackets(word);

            return;
        }
    }    
}