using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ModpackDownloader
{
    public class Downloader : IDisposable
    {
        private HttpClient Client
        {
            get;

            set;
        }


        public Downloader(HttpMessageHandler handler = null)
        {
            if (handler != null)
            {
                Client = new HttpClient(handler);
            }
            else
            {
                Client = new HttpClient();
            }
        }


        public async Task<Stream> GetFileAsStreamAsync(string fileUri)
        {
            if (string.IsNullOrEmpty(fileUri))
            {
                throw new ArgumentNullException(nameof(fileUri));
            }
            if (!Uri.IsWellFormedUriString(fileUri, UriKind.Absolute))
            {
                throw new ArgumentException(nameof(fileUri) + " is not well-formed.");
            }

            var result = await Client.GetAsync(fileUri);
            if (!result.IsSuccessStatusCode)
            {
                // TODO add logging
                return null;
            }

            return await result.Content.ReadAsStreamAsync();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Client != null)
                {
                    Client.Dispose();
                }
            }
        }
    }
}
