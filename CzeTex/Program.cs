using System;

namespace CzeTex
{
    class Program
    {
        static void Main(string[] args)
        {
            Files files = new Files(new string[] {"examples", "example1.txt"});
            files.ShowContent();
        }
    }
}