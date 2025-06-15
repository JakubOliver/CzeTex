using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using iText.StyledXmlParser.Jsoup.Nodes;
using Org.BouncyCastle.Tls.Crypto;

namespace CzeTex{
    public class Commander
    {
        PDF pdf;
        Files file;
        Trie trie;

        string[] content;

        public Commander(string path)
        {
            file = new Files(path);
            content = file.LoadFile();
            string basename = file.GetBaseName();

            pdf = new PDF(basename);
            trie = new Trie();

            //toto nahradit hledanim v Trie plus nacitani kodu z JSON
            trie.AddFunction("bold", pdf.AddBoldText);
            trie.AddFunction("cursive", pdf.AddCursiveText);
            trie.AddFunction("title", pdf.AddTitle);
            trie.AddFunction("section", pdf.CreateParagraph);
            trie.AddFunction("slash", pdf.AddSlash);
            trie.AddFunction("newpage", pdf.AddNewPage);
            trie.AddFunction("x", pdf.RemoveFont);

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

    public class TrieNode{
        public TrieNode[]? children;
        public int idx;

        public void CreateChildren(){
            children = new TrieNode[27];
        }
    }

    public class Trie{
        public TrieNode root;
        public int NumberOfFunctions = 0;
        public List<Delegate> Functions = new List<Delegate>();

        public Trie(){
            root = new TrieNode();
        }

        private int GetIdx(char c){
            return c - 'a';
        }

        private TrieNode GetFunctionNode(string name){
            TrieNode current = root;
            int idx;

            foreach (char c in name){
                idx = GetIdx(c);
                if (current.children == null){
                    current.CreateChildren();
                }

                if (current.children![idx] == null){ //tento warning je neopodstatněný, ponevadz current.children se incializuje v předešlém ifu => takže jsem ji potlačil
                    current.children[idx] = new TrieNode();
                }
                current = current.children[idx];
            }

            return current;
        }

        public void AddFunction(string name, Action<string> function){
            TrieNode current = GetFunctionNode(name);
            current.idx = NumberOfFunctions;
            Functions.Add((Action<string>)function);
            NumberOfFunctions++;
        }

        public void AddFunction(string name, Action function){
            TrieNode current = GetFunctionNode(name);
            current.idx = NumberOfFunctions;
            Functions.Add((Action)function);
            NumberOfFunctions++;
        }

        public int FindFunction(string name){
            TrieNode current = root;
            int idx;
            foreach (char c in name){
                idx = GetIdx(c);
                if (current.children == null || idx < 0 || idx > 26 || current.children[idx] == null){
                    
                    throw new Exception($"Given CzeTex function {name} does not exist!");
                }

                current = current.children[idx];
            }

            return current.idx;
        }

        public void DFS(TrieNode node){
            if (node == null || node.children == null){
                return;
            }

            for (int i = 0; i < 27; i++){
                if (node.children[i] != null){
                    Console.WriteLine("{0}", (char)(i + 'a'));
                    DFS(node.children[i]);
                }
            }
        }
    }
}