using System;
using System.Collections.Generic;

namespace CzeTex{
    public class TrieNode
    {
        public TrieNode[]? children;
        public int idx;

        public void CreateChildren()
        {
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

        public void AddFunction(string name, Action<List<string>> function){
            TrieNode current = GetFunctionNode(name);
            current.idx = NumberOfFunctions;
            Functions.Add((Action<List<string>>)function);
            NumberOfFunctions++;
        }

        /*
        public void AddFunction(string name, Action function)
        {
            TrieNode current = GetFunctionNode(name);
            current.idx = NumberOfFunctions;
            Functions.Add((Action)function);
            NumberOfFunctions++;
        }
        */

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