using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

using Newtonsoft.Json;

namespace ModpackDownloader
{
    public class ModpackDefinitionHelper
    {
        public const string MANIFEST_FILE_NAME = "manifest.json";


        public Manifest GetManifest(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            var manifestFile = GetManifestFile(filePath);

            if (string.IsNullOrEmpty(manifestFile))
                throw new InvalidOperationException($"Modpack definition file '{filePath}' is empty.");

            var manifest = JsonConvert.DeserializeObject<Manifest>(manifestFile);

            return manifest;
        }


        internal string GetManifestFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new InvalidOperationException($"Modpack definition file '{filePath}' doesn't exist.");

            using (ZipArchive archive = ZipFile.OpenRead(filePath))
            {
                var manifestEntry = archive.Entries.FirstOrDefault(entry => entry.Name == MANIFEST_FILE_NAME)
                    ?? throw new Exception($"Modpack definition file '{filePath}' doesn't contain manifest.");

                using (var streamReader = new StreamReader(manifestEntry.Open()))
                {
                    var content = streamReader.ReadToEnd();

                    return content;
                }
            }
        }
    }
}