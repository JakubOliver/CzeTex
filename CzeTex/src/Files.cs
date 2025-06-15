using System;
using System.IO;
using System.Linq;

namespace CzeTex
{
    public class Files{
        string Path;

        public Files(string[] path)
        {
            this.Path = this.GetPath(path);
        }   
        
        public Files(string path) : this (path.Split(System.IO.Path.DirectorySeparatorChar)){}

        public string GetRelativePath(string[] path, bool debug = false)
        {
            string FilePath = System.IO.Path.Combine(path);

            if (debug)
            {
                return System.IO.Path.Combine(new string[] { Directory.GetCurrentDirectory(), "..", "..", "..", FilePath });
            }
            else
            {
                return FilePath;
            }
        }

        public string GetBaseName()
        {
            string[] parts = this.Path.Split(System.IO.Path.DirectorySeparatorChar);

            int end = 0;
            string withEnding = parts[parts.Length - 1];

            while (end != withEnding.Length && withEnding[end] != '.')
            {
                end++;
            }

            return withEnding[0..end];
        }

        public string[] LoadFile()
        {
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