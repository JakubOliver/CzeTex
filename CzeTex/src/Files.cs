using System;
using System.IO;

namespace CzeTex
{
    public class Files{
        string Path;

        public Files(string[] path)
        {
            this.Path = GetRelativePath(path);
            this.FileExists();
        }   

        public string GetRelativePath(string[] path){
            string FilePath = System.IO.Path.Combine(path);
            return System.IO.Path.Combine(new string[] {Directory.GetCurrentDirectory(), "..", "..", "..", FilePath});
        }

        public string[] LoadFile(){
            string[] content;

            content = System.IO.File.ReadAllLines(Path);

            return content;
        }

        public void FileExists(){
            if (!System.IO.File.Exists(Path)){
                throw new System.IO.FileNotFoundException($"File {Path} not found from {AppContext.BaseDirectory}");
            }
        }

        public void ShowContent(){
            foreach (string line in LoadFile()){
                Console.WriteLine(line);
            }
        }
    }
}