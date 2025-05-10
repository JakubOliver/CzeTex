using System;
using System.IO;

namespace CzeTex
{
    public class Files{
        string Path;

        public Files(string[] path)
        {
            this.Path = this.GetPath(path);
        }   

        public string GetRelativePath(string[] path, bool debug = false){
            string FilePath = System.IO.Path.Combine(path);

            if (debug){
                return System.IO.Path.Combine(new string[] {Directory.GetCurrentDirectory(), "..", "..", "..", FilePath});
            } else {
                return FilePath;
            }
        }

        public string[] LoadFile(){
            string[] content;

            content = System.IO.File.ReadAllLines(Path);

            return content;
        }

        public string GetPath(string[] path){
            if (System.IO.File.Exists(GetRelativePath(path))){
                return GetRelativePath(path);
            }

            if (System.IO.File.Exists(GetRelativePath(path, true))){
                return GetRelativePath(path, true);
            }

            throw new System.IO.FileNotFoundException($"File {Path} not found");
        }

        public void ShowContent(){
            foreach (string line in LoadFile()){
                Console.WriteLine(line);
            }
        }
    }
}