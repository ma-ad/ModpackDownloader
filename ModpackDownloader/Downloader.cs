using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ModpackDownloader
{
    class Downloader : IDisposable
    {
        private HttpClient Client
        {
            get;

            set;
        }


        public Downloader()
        {
            Client = new HttpClient();
        }


        async Task<Stream> GetFileAsStream(string fileUri)
        {
            if (string.IsNullOrEmpty(fileUri))
            {
                throw new ArgumentNullException(nameof(fileUri));
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
