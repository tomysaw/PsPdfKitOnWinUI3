using System.Reflection;
using SharpCompress.Archives;

namespace PsPdfKitOnWinUI3.Lib
{
    public static class FileManager
    {
        public static void WriteResourceToFile(string resourceName, string fileName)
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resource?.CopyTo(file);
                }
            }
        }

        public static void ExtractZip(string fileZipPath, string outputFolder)
        {
            using (IArchive archive = ArchiveFactory.Open(fileZipPath))
            {
                foreach (var zipEntry in archive.Entries)
                {
                    string entryFullPath = Path.Combine(outputFolder, zipEntry.Key);
                    if (zipEntry.IsDirectory)
                    {
                        Directory.CreateDirectory(entryFullPath);
                    }
                    else if (zipEntry.IsComplete)
                    {
                        string directory = Path.GetDirectoryName(entryFullPath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        using (FileStream fs = new FileStream(entryFullPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                        {
                            zipEntry.WriteTo(fs);
                        }
                    }
                }
            }
        }
    }
}
