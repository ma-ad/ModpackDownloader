using NSubstitute;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

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
                Assert.ThrowsAsync<ArgumentNullException>(() => downloader.GetFileAsStreamAsync(nullUri));
            }
        }


        [Test]
        public void When_FileUriMalformed_Then_ThrowException()
        {
            const string malformedUri = "wwac";
            using (Downloader downloader = new Downloader())
            {
                Assert.ThrowsAsync<ArgumentException>(() => downloader.GetFileAsStreamAsync(malformedUri));
            }
        }


        [Test]
        public async Task When_ResponseMessageNotSuccessfull_Then_ReturnNull()
        {
            const string wellFormedUri = "https://something.au";
            var messageHandler = new HttpMessageHandlerSubstitute(
                new HttpResponseMessage(HttpStatusCode.BadRequest));
            Stream resultStream;

            using (Downloader downloader = new Downloader(messageHandler))
            {
                resultStream = await downloader.GetFileAsStreamAsync(wellFormedUri);
            }

            Assert.That(resultStream, Is.Null);
        }


        [Test]
        public async Task When_ResponseOk_Then_ReturnStream()
        {
            const string wellFormedUri = "https://something.au";
            const string proposedContent = "testSubject";

            var responseMessage = new HttpResponseMessage(HttpStatusCode.Accepted);
            responseMessage.Content = new StringContent(proposedContent);
            var messageHandler = new HttpMessageHandlerSubstitute(responseMessage);
            Stream resultStream;
            string actualContent;

            using (Downloader downloader = new Downloader(messageHandler))
            {
                resultStream = await downloader.GetFileAsStreamAsync(wellFormedUri);
                using (StreamReader reader = new StreamReader(resultStream))
                {
                    actualContent = reader.ReadToEnd();
                }
            }

            Assert.That(resultStream, Is.Not.Null);
            Assert.That(actualContent, Is.EqualTo(proposedContent));
        }
    }
}
