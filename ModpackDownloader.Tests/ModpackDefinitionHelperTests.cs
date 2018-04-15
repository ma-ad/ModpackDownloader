using System;
using System.IO;
using System.Linq;

using ModpackDownloader;
using NUnit.Framework;

namespace ModpackDownloader.Tests
{
    [TestFixture]
    public class ModpackDefinitionHelperTests
    {
        [Test]
        public void When_ZipFileIsValid_Then_ManifestIsExtracted()
        {
            var helper = new ModpackDefinitionHelper();
            var manifest = helper.GetManifest("test-files/valid-modpack.zip");

            Assert.That(manifest.Name, Is.EqualTo("ModPackName"));
            Assert.That(manifest.Version, Is.EqualTo("1.0.6"));
            Assert.That(manifest.Author, Is.EqualTo("author"));
            Assert.That(manifest.ManifestType, Is.EqualTo("minecraftModpack"));
            Assert.That(manifest.ManifestVersion, Is.EqualTo(1));
            Assert.That(manifest.Overrides, Is.EqualTo("overrides"));
            Assert.That(manifest.Minecraft.Version, Is.EqualTo("1.12.2"));
            Assert.That(manifest.Minecraft.ModLoaders.Count, Is.EqualTo(1));
            Assert.That(manifest.Minecraft.ModLoaders[0].Id, Is.EqualTo("forge-14.23.2.2651"));
            Assert.That(manifest.Minecraft.ModLoaders[0].Primary, Is.True);
            Assert.That(manifest.Files[0].ProjectId, Is.EqualTo("251079"));
            Assert.That(manifest.Files[0].FileId, Is.EqualTo("2446790"));
            Assert.That(manifest.Files[0].Required, Is.True);
            Assert.That(manifest.Files[1].ProjectId, Is.EqualTo("223794"));
            Assert.That(manifest.Files[1].FileId, Is.EqualTo("2543840"));
            Assert.That(manifest.Files[1].Required, Is.False);
        }


        [Test]
        public void When_ZipFileIsMissingManifest_Then_ThrowException()
        {
            var helper = new ModpackDefinitionHelper();

            var exception = Assert.Throws<Exception>(()=>helper.GetManifest("test-files/missing-manifest-modpack.zip"));

            Assert.That(exception.Message, Is.EqualTo("Modpack definition file 'test-files/missing-manifest-modpack.zip' doesn't contain manifest."));
        }
    }
}