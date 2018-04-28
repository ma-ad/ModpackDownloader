using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using NUnit.Framework;

namespace ModpackDownloader.Tests
{
    [TestFixture]
    public class DownloaderTests
    {
        [Test]
        public void When_FileUriIsNull_Then_ThrowException()
        {
            const string nullUri = null;
            using (Downloader downloader = new Downloader())
            {
                Assert.ThrowsAsync<ArgumentNullException>(() => downloader.SaveAsTempFileAsync(nullUri));
            }
        }


        [Test]
        public void When_FileUriMalformed_Then_ThrowException()
        {
            const string malformedUri = "wwac";
            using (Downloader downloader = new Downloader())
            {
                Assert.ThrowsAsync<ArgumentException>(() => downloader.SaveAsTempFileAsync(malformedUri));
            }
        }


        [Test]
        public async Task When_ResponseMessageNotSuccessfull_Then_ReturnNull()
        {
            const string wellFormedUri = "https://something.au";
            var messageHandler = new HttpMessageHandlerSubstitute(
                new HttpResponseMessage(HttpStatusCode.BadRequest));
            String downloadedFilePath;

            using (Downloader downloader = new Downloader(messageHandler))
            {
                downloadedFilePath = await downloader.SaveAsTempFileAsync(wellFormedUri);
            }

            Assert.That(downloadedFilePath, Is.Null);
        }


        [Test]
        public async Task When_ResponseOk_Then_ReturnFilePath()
        {
            const string wellFormedUri = "https://something.au";
            const string proposedContent = "testSubject";

            var responseMessage = new HttpResponseMessage(HttpStatusCode.Accepted);
            responseMessage.Content = new StringContent(proposedContent);
            var messageHandler = new HttpMessageHandlerSubstitute(responseMessage);
            string downloadedFilePath;
            string actualContent;

            using (Downloader downloader = new Downloader(messageHandler))
            {
                downloadedFilePath = await downloader.SaveAsTempFileAsync(wellFormedUri);
                using (StreamReader reader = new StreamReader(downloadedFilePath))
                {
                    actualContent = reader.ReadToEnd();
                }
            }

            Assert.That(downloadedFilePath, Is.Not.Null);
            Assert.That(actualContent, Is.EqualTo(proposedContent));
        }
    }
}
