using System;
using System.IO;
using iText.Kernel.Pdf.Annot;

namespace CzeTex
{
    class Program
    {
        static void Main(string[] args)
        {
            string path;
            if (args.Length < 1)
            {
                path = "examples/example3.txt";
            }
            else
            {
                path = args[0];
            }
            
            Commander commander = new Commander(path);
        }
    }
}