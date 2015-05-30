namespace StrangeToolkit.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public sealed class FilePath
    {
        private const Char Separator = '/';

        private static readonly Char[] SplitArray = { Separator };

        public List<string> Folders { get; private set; }

        public string FileName { get; private set; }

        private static string PreparePath(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            foreach (var ch in invalidChars.Where(ch => fileName.Contains(ch.ToString()) && ch != Separator))
            {
                fileName = fileName.Replace(ch.ToString(), string.Empty);
            }

            return fileName;
        }
        public static FilePath CreateFilePath(string localPath)
        {
            var filePath = new FilePath
            {
                Folders = new List<string>()
            };

            localPath = PreparePath(localPath);

            var split = localPath.Split(SplitArray, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length > 1)
            {
                for (var i = 0; i < split.Length - 1; i++)
                {
                    filePath.Folders.Add(split[i]);
                }
            }

            filePath.FileName = split.Last();
            return filePath;
        }

        public static List<string> CreateFolders(string folderPath)
        {
            folderPath = PreparePath(folderPath);
            var folders = folderPath.Split(SplitArray, StringSplitOptions.RemoveEmptyEntries).ToList();
            return folders;
        }
    }
}
