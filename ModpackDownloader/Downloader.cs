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

        /// <summary>
        /// Downloads file to temporary folder and returns its path.
        /// </summary>
        /// <param name="fileUri">Uri of the file to be downloaded.</param>
        /// <returns>Full file path of the downloaded temp file if successful. Null otherwise.</returns>
        public async Task<String> SaveAsTempFileAsync(string fileUri)
        {
            if (string.IsNullOrEmpty(fileUri))
            {
                throw new ArgumentNullException(nameof(fileUri));
            }
            if (!Uri.IsWellFormedUriString(fileUri, UriKind.Absolute))
            {
                throw new ArgumentException(nameof(fileUri) + " is not well-formed.");
            }

            string tempFilePath;

            using (HttpResponseMessage response = await Client.GetAsync(fileUri, HttpCompletionOption.ResponseHeadersRead))
            {
                if (!response.IsSuccessStatusCode)
                {
                    // TODO add logging
                    return null;
                }
                tempFilePath = Path.GetTempFileName();
                await SaveResponseToFileAsync(response, tempFilePath);
            }

            return tempFilePath;
        }


        private async Task SaveResponseToFileAsync(HttpResponseMessage response, String path)
        {
            using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
            {
                using (Stream streamToWriteTo = File.Open(path, FileMode.Create))
                {
                    await streamToReadFrom.CopyToAsync(streamToWriteTo);
                }
            }
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
