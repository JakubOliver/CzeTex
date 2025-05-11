using System;
using System.Collections.Generic;
using System.ComponentModel;
using iText.StyledXmlParser.Jsoup.Nodes;
using Org.BouncyCastle.Tls.Crypto;

namespace CzeTex{
    public class Commander{
        PDF pdf;
        Trie trie;
        public Commander(){
            pdf = new PDF();
            trie = new Trie();

            //toto nahradit hledanim v Trie plus nacitani kodu z JSON
            trie.AddFunction("bold", pdf.AddBoldText);
            trie.AddFunction("cursive", pdf.AddCursiveText);
            trie.AddFunction("title", pdf.AddTitle);
            trie.AddFunction("section", pdf.CreateParagraph);
        }

        public void ReadContent(string[] content){
            string[] words;
            for (int i = 0; i < content.Length; i++){
                words = content[i].Split(' ');

                for (int j = 0; j < words.Length; j++){
                    //Console.WriteLine(word);
                    if (words[j].Length == 0){
                        continue;
                    }

                    if (words[j][0] != '/'){
                        pdf.AddText(words[j]);
                    } else {
                        string functionName = words[j].Split('(')[0].Trim('/');

                        //TODO: vyceslovne parametry
                        int idx = trie.FindFunction(functionName);
                        if (words[j].Split('(').Length == 1){
                            ((Action)trie.Functions[idx])();
                        } else {
                            string parametr = words[j].Split('(')[1].Trim(')');
                            ((Action<string>)trie.Functions[idx])(parametr);
                        }
                    }
                    /*
                    if (word.Split('(')[0] == "/title"){ //vyresit vyce slovne nadpisy
                        pdf.AddTitle(word.Split('(')[1].TrimEnd(')'));
                    }

                    if (word == "/section"){
                        pdf.CreateParagraph();
                    }

                    if (word.Split('(')[0] == "/bold"){
                        pdf.AddBoldText(word.Split('(')[1].TrimEnd(')'));
                    }

                    if (word.Split('(')[0] == "/cursive"){
                        pdf.AddCursiveText(word.Split('(')[1].TrimEnd(')'));
                    }*/
                }
            }
            pdf.Export();
        }
    }

    public class TrieNode{
        public TrieNode[]? children;
        public int idx;

        public void CreateChildren(){
            children = new TrieNode[27];
        }
    }

    /*
    public class FunctionNode<T1> : TrieNode{
        Action<T1> function;

        public FunctionNode(Action<T1> function){
            this.function = function;
        }
        public void Execute(T1 arg){
            function(arg);
        }
    }

    public class FunctionNode : TrieNode{
        Action function;
        public int idx;

        public FunctionNode(Action function){
            this.function = function;
        }

        public FunctionNode(int idx){
            this.idx = idx;
        }
        public void Execute(){
            function();
        }
    }  
    */

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
                    
                    throw new Exception($"Give CzeTex function {name} does not exist!");
                }

                current = current.children[idx];
            }

            return current.idx;
        }

        /*
        public void AddFunction(string name, Action<string> function){
            TrieNode current = GetFunctionNode(name);

            current = new FunctionNode<string>(function);
        }

        public void AddFunction(string name, Action function){
            TrieNode current = GetFunctionNode(name);

            current = new FunctionNode(function);
        }

        private TrieNode FindFunction(string name){
            TrieNode current = root;
            int idx;
            foreach (char c in name){
                idx = GetIdx(c);
                if (current.children == null || idx < 0 || idx > 26 || current.children[idx] == null){
                    
                    throw new Exception($"Give CzeTex function {name} does not exist!");
                }

                current = current.children[idx];
            }

            return current;
        }

        public FunctionNode<string> GetFunction1(string name){
            return (FunctionNode<string>) FindFunction(name);
        }

        public FunctionNode GetFunction0(string name){
            return (FunctionNode) FindFunction(name);
        }
        */

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