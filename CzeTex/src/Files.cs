using System;
using System.IO;

namespace CzeTex
{
    /// <summary>
    /// Class containing file related constants 
    /// </summary>
    public static class FilesConstants
    {
        public const int lengthOfTxtExtension = 3;
    }

    /// <summary>
    /// Class <c>Files</c> provides basic functions for working with files.
    /// </summary>
    public class Files
    {
        private string path;

        public Files(string[] path)
        {
            this.path = this.GetPath(path);
        }

        public Files(string path) :
            this(path.Split(System.IO.Path.DirectorySeparatorChar)) { }

        public string Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// Returns relative path to file from different program entry points.
        /// </summary>
        public string GetRelativePath(string[] path, bool debug = false)
        {
            string FilePath = System.IO.Path.Combine(path);

            if (debug)
            {
                return System.IO.Path.Combine(new string[]
                    { Directory.GetCurrentDirectory(), "..", "..", "..", FilePath });
            }
            else
            {
                return FilePath;
            }
        }

        /// <summary>
        /// Returning basename of file (name without extension).
        /// </summary>
        public string GetBaseName()
        {
            string[] parts = this.path.Split(System.IO.Path.DirectorySeparatorChar);

            int end = 0;
            string withEnding = parts[parts.Length - 1];

            while (end != withEnding.Length && withEnding[end] != '.')
            {
                end++;
            }

            return withEnding[0..end];
        }

        /// <summary>
        /// Returns path to the to be generated pdf file.
        /// </summary>
        public string GetPDFPath()
        {
            int length = this.path.Length;

            return this.path[0..(length - FilesConstants.lengthOfTxtExtension)]
                + "pdf";
        }

        /// <summary>
        /// Reads all lines of a file and returns an array of file's lines.
        /// </summary>
        public string[] LoadFile()
        {
            string[] content;

            content = System.IO.File.ReadAllLines(path);

            return content;
        }

        public string LoadFileIntoOneString()
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Returns concatenated path based on OS directory separator character
        /// </summary>
        public string ConcatPath(string[] path)
        {
            return string.Join(System.IO.Path.DirectorySeparatorChar, path);
        }

        /// <summary>
        /// Returns correct file path from the current program entry point.
        /// </summary>
        public string GetPath(string[] path)
        {
            if (System.IO.File.Exists(GetRelativePath(path)))
            {
                return GetRelativePath(path);
            }

            if (System.IO.File.Exists(GetRelativePath(path, true)))
            {
                return GetRelativePath(path, true);
            }

            //Try absolute path (at least path from home)
            if (File.Exists(this.ConcatPath(path)))
            {
                return this.ConcatPath(path);
            }

            throw new System.IO.FileNotFoundException(
                $"File {this.ConcatPath(path)} not found!");
        }

        /// <summary>
        /// Displays content of the file to the standard output.
        /// </summary>
        public void ShowContent()
        {
            foreach (string line in LoadFile())
            {
                Console.WriteLine(line);
            }
        }
    }
}