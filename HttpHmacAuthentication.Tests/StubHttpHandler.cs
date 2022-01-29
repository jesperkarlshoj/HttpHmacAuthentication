using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpHmacAuthentication.Tests
{
    public class StubHttpHandler : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc;

        public HttpRequestMessage RequestMessage { get; set; }
        public HttpResponseMessage ResponseMessage { get; set; }

        public StubHttpHandler(string responseSignature, string body)
        {
            handlerFunc = (r, c) =>
            {
                RequestMessage = r;
                return Return200(responseSignature, body);
            };
        }

        public StubHttpHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
        {
            this.handlerFunc = handlerFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return handlerFunc(request, cancellationToken);
        }

        private static Task<HttpResponseMessage> Return200(string responseSignature, string body)
        {
            var msg = new HttpResponseMessage(HttpStatusCode.OK);
            msg.Headers.Add("X-Server-Authorization-HMAC-SHA256", responseSignature);
            if (!string.IsNullOrEmpty(body))
            {
                msg.Content = new StringContent(body);
            }

            return Task.FromResult(msg);
        }
    }
}