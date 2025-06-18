using System;
using System.IO;

namespace CzeTex
{
    /// <summary>
    /// Class <c>Files</c> provides basic functions for working with files.
    /// </summary>
    public class Files
    {
        string Path;

        public Files(string[] path)
        {
            this.Path = this.GetPath(path);
        }

        public Files(string path) : this(path.Split(System.IO.Path.DirectorySeparatorChar)) { }

        /// <summary>
        /// Returns relative path to file from different program entry points.
        /// </summary>
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

        /// <summary>
        /// Returning basename of file (name without extension).
        /// </summary>
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

        /// <summary>
        /// Reads all lines of a file and returns an array of file's lines.
        /// </summary>
        public string[] LoadFile()
        {
            string[] content;

            content = System.IO.File.ReadAllLines(Path);

            return content;
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

            throw new System.IO.FileNotFoundException($"File {string.Join(System.IO.Path.DirectorySeparatorChar, path)} not found");
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