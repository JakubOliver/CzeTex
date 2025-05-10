using System;

namespace CzeTex
{
    class Program
    {
        static void Main(string[] args)
        {
            Files files = new Files(new string[] {"examples", "example1.txt"});
            //files.ShowContent();
            string[] content = files.LoadFile();

            PDF pdf = new PDF();
            pdf.CreateParagraph();
            
            foreach (string line in content)
            {
                pdf.AddText(line);
            }
            pdf.AddParagraph();
            pdf.Export();
        }
    }
}