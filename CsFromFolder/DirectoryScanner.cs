using System.Collections.Generic;
using System.IO;

namespace CsFromFolder
{
    static class DirectoryScanner
    {
        public static IEnumerable<string> GetDirectoryFilesFullPathsRecursively(string directoryWithFiles, string searchPattern)
        {
            string[] filesInFolder = Directory.GetFiles(directoryWithFiles, searchPattern);
            foreach (string file in filesInFolder)
            {
                yield return file;
            }

            string[] nestedFolders = Directory.GetDirectories(directoryWithFiles);
            foreach (string nestedFolder in nestedFolders)
            {
                var allFoldersFiles = GetDirectoryFilesFullPathsRecursively(nestedFolder, searchPattern);
                foreach (string nestedFile in allFoldersFiles)
                {
                    yield return nestedFile;
                }
            }
        }
    }
}
