using System.Collections.Generic;

namespace ModpackDownloader
{
    public class Manifest
    {
        public Minecraft Minecraft { get; set; }


        public string ManifestType { get; set; }


        public int ManifestVersion { get; set; }


        public string Name { get; set; }


        public string Version { get; set; }


        public string Author { get; set; }


        public List<FileType> Files { get; set; }


        public string Overrides { get; set; }
    }


    public class Minecraft
    {
        public string Version { get; set; }


        public List<ModLoader> ModLoaders { get; set; }
    }


    public class ModLoader
    {
        public string Id { get; set; }


        public bool Primary { get; set; }
    }


    public class FileType
    {
        public string ProjectId { get; set; }


        public string FileId { get; set; }


        public bool Required { get; set; }
    }
}