using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Newtonsoft.Json;

namespace ModpackDownloader
{
    public class ModpackDefinitionHelper
    {
        public const string MANIFEST_FILE_NAME = "manifest.json";


        public IDirectoryWrapper Directory
        {
            get;
        }


        public IFileWrapper File
        {
            get;
        }


        public ModpackDefinitionHelper(IDirectoryWrapper directoryWrapper, IFileWrapper fileWrapper)
        {
            this.File = fileWrapper;
            Directory = directoryWrapper;
        }


        public Manifest GetManifest(ZipArchive archive)
        {
            if (archive == null)
                throw new ArgumentNullException(nameof(archive));

            var manifestEntry = archive.Entries.FirstOrDefault(entry => entry.Name == MANIFEST_FILE_NAME) ??
                throw new Exception($"Zip archive doesn't contain manifest.");

            Manifest manifest;

            using (StreamReader reader = new StreamReader(manifestEntry.Open()))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                manifest = serializer.Deserialize<Manifest>(jsonReader);

                return manifest;
            }
        }


        public void ApplyOverrides(ZipArchive archive, string directoryPath, string overridesDirectory = "overrides")
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new ArgumentException($"Directory '{directoryPath}' doesn't exist.");
            }

            foreach (var entry in archive.Entries)
            {
                // Directories have empty name and fullname ending with '/'
                if (string.IsNullOrEmpty(entry.Name))
                {
                    continue;
                }
                else if (entry.FullName.StartsWith($"{overridesDirectory}/"))
                {
                    using (var fileStream = File.Create(Path.Combine(overridesDirectory, entry.Name)))
                    {
                        using (var entryStream = entry.Open())
                        {
                            entryStream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }
    }
}