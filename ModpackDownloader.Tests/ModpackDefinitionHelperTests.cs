using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Ninject;
using Ninject.MockingKernel;
using Ninject.MockingKernel.NSubstitute;
using NSubstitute;
using NUnit.Framework;

using ModpackDownloader;

namespace ModpackDownloader.Tests
{
    [TestFixture]
    public class ModpackDefinitionHelperTests
    {
        public const string VALID_MODPACK_FILE = "test-files/valid-modpack.zip";
        public const string MISSING_MANIFEST_MODPACK_FILE = "test-files/missing-manifest-modpack.zip";


        private IKernel Kernel
        {
            get;
            set;
        }


        [SetUp]
        public void SetUp()
        {
            Kernel = new MockingKernel();
            Kernel.Load<NSubstituteModule>();
        }


        [TearDown]
        public void TearDown()
        {
            Kernel.Dispose();
        }


        [Test]
        public void When_ZipFileIsValid_Then_ManifestIsExtracted()
        {
            var helper = Kernel.Get<ModpackDefinitionHelper>();

            Manifest manifest;

            using(var archive = ZipFile.OpenRead(VALID_MODPACK_FILE))
            {
                manifest = helper.GetManifest(archive);
            }

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
            var helper = Kernel.Get<ModpackDefinitionHelper>();
            Exception exception;

             using(var archive = ZipFile.OpenRead(MISSING_MANIFEST_MODPACK_FILE))
            {
                exception = Assert.Throws<Exception>(()=>helper.GetManifest(archive));
            }

            Assert.That(exception.Message, Is.EqualTo("Zip archive doesn't contain manifest."));
        }


        [Test]
        public void When_ZipFileContainsOverrides_Then_OverridesAreApplied()
        {
            Kernel.Get<IDirectoryWrapper>().Exists("tmp").Returns(true);
            
            var streams = new Dictionary<string, MemoryStream>();

            Kernel.Get<IFileWrapper>().Create(Arg.Any<string>()).Returns((callInfo) =>
                {
                    var stream = new MemoryStream();
                    streams.Add(callInfo.Arg<string>(), stream);
                    return stream;
                });
            
            var helper = Kernel.Get<ModpackDefinitionHelper>();

            using(var archive = ZipFile.OpenRead(VALID_MODPACK_FILE))
            {
                helper.ApplyOverrides(archive, "tmp");
            }

            var filePaths = new List<string>{"overrides/a.jar","overrides/b.txt"};

            Assert.That(streams.Keys.Count, Is.EqualTo(2));
            // Are we closing streams?
            Assert.That(streams.All(x => x.Value.CanRead), Is.False);
            Assert.That(streams.Keys, Is.EquivalentTo(filePaths));
        }
    }
}