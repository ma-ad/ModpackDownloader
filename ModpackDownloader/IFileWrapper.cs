using System.IO;

namespace ModpackDownloader
{
    public interface IFileWrapper
    {
        Stream Create(string path);
    }
}