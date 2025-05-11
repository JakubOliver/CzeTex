using System;
using iText.Kernel.Pdf.Annot;

namespace CzeTex
{
    class Program
    {
        static void Print()
        {
            Console.WriteLine("hello");
        }

        static void Main(string[] args)
        {
            
            Files files = new Files(new string[] {"examples", "example1.txt"});
            //files.ShowContent();
            string[] content = files.LoadFile();

            Commander commander = new Commander();
            commander.ReadContent(content);
            /*
            PDF pdf = new PDF();

            Trie trie = new Trie();
            trie.AddFunction("bold", Print);

            trie.DFS(trie.root);
            System.Console.WriteLine("-----------------");
            trie.AddFunction("cursive", pdf.AddBoldText);
            trie.DFS(trie.root);
            */
        }
    }
}