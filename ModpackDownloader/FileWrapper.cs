using System.IO;

namespace ModpackDownloader
{
    public class FileWrapper : IFileWrapper
    {
        public Stream Create(string path)
        {
            return File.Create(path);
        }
    }
}