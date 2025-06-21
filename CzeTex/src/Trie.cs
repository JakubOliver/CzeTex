using System;
using System.Collections.Generic;
using iText.Layout.Element;

namespace CzeTex{
    /// <summary>
    /// Obtains constants important mainly for the Trie class.
    /// </summary>
    public static class TrieConstants
    {
        public const int numberOfChildren = 26;
        public const char smallestAvailableCharacter = 'a';
    }

    /// <summary>
    /// Vertex of trie tree.
    /// </summary>
    public class TrieNode
    {
        public TrieNode[]? children;
        public int idx = -1;

        /// <summary>
        /// Creates children of this vertex.
        /// </summary>
        public void CreateChildren()
        {
            children = new TrieNode[TrieConstants.numberOfChildren];
        }
    }

    /// <summary>
    /// A trie tree that stores the indices of functions in arrays in its nodes.
    /// </summary>
    /// <remarks>
    /// This trie is built only with nodes representing lowercase English letters, 
    /// from this part comes the restriction that
    /// CzeTex function names can be only made from lowercase letters.
    /// <para>
    /// Bijection between the name of CzeTex function and trie is the following: 
    ///     the function name corresponds to the sequence of letters along the path 
    ///     from the root to the function node (node representing last character 
    ///     of function name), therefore cannot exists two CzeTex functions 
    ///     with the same name. 
    /// </para>
    /// </remarks>
    public class Trie
    {
        public TrieNode root;
        public int NumberOfFunctions = 0;
        public List<Delegate> addFunctions = new List<Delegate>();
        public List<Delegate?> getFunctions = new List<Delegate?>();

        public Trie()
        {
            root = new TrieNode();
        }

        /// <summary>
        /// Returns index of vertex in children array.
        /// </summary>
        private int GetIdx(char c)
        {
            return c - TrieConstants.smallestAvailableCharacter;
        }

        /// <summary>
        /// Returns node where should be placed id of function.
        /// </summary>
        private TrieNode GetFunctionNode(string name)
        {
            TrieNode current = root;
            int idx;

            foreach (char c in name)
            {
                idx = GetIdx(c);
                if (current.children == null)
                {
                    current.CreateChildren();
                }

                //This warning is unjustified because current.children is is 
                //initialized in CreateChildren function.
                //Therefore if current.children did not exist, 
                // then it was created in previous if statement.
                //So I disabled the warning.
                if (current.children![idx] == null)
                {
                    current.children[idx] = new TrieNode();
                }

                current = current.children[idx];
            }

            return current;
        }

        /// <summary>
        /// Add into the function node index of add and get function in respective arrays.
        /// </summary>
        public void AddFunction(
            string name,
            Action<List<string>> addFunction,
            Func<List<string>, Text>? getFunction = null
        )
        {
            TrieNode current = GetFunctionNode(name);

            //It is not necessary to check whether the CzeTex function name has duplicate, 
            //because in the JSON file cannot exists two valid entries with same key.
            //However, for general purpose it makes sense to check for duplicate keys.
            /*
            if (current.idx != -1)
            {
                throw new TrieExceptions($"Functions with name {name} already exists");
            }
            */

            current.idx = NumberOfFunctions;

            addFunctions.Add(addFunction);
            getFunctions.Add(getFunction);

            NumberOfFunctions++;
        }

        /// <summary>
        /// Returns index of the function in functions arrays from function node.
        /// </summary>
        public int FindFunction(string name)
        {
            TrieNode current = root;
            int idx;

            foreach (char c in name)
            {
                idx = GetIdx(c);
                if (current.children == null ||
                    idx < 0 ||
                    idx > TrieConstants.numberOfChildren ||
                    current.children[idx] == null
                )
                {
                    throw new TrieExceptions(
                        $"Given CzeTex function {name} does not exist!");
                }

                current = current.children[idx];
            }

            return current.idx;
        }

        /// <summary>
        /// Depth-First Search for trie.
        /// </summary>
        public void DFS(TrieNode node)
        {
            if (node == null || node.children == null)
            {
                return;
            }

            for (int i = 0; i < TrieConstants.numberOfChildren; i++)
            {
                if (node.children[i] != null)
                {
                    Console.WriteLine("{0}",
                        (char)(i + TrieConstants.smallestAvailableCharacter));
                    DFS(node.children[i]);
                }
            }
        }
    }
}