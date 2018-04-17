using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModpackDownloader.Tests
{
    class HttpMessageHandlerSubstitute : HttpMessageHandler
    {
        private HttpResponseMessage ResponseMessageToReturn { get; set; }


        public HttpMessageHandlerSubstitute(HttpResponseMessage responseMessageToReturn)
        {
            ResponseMessageToReturn = responseMessageToReturn;
        }


        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(ResponseMessageToReturn);
        }
    }
}
