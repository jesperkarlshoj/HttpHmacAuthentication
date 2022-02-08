using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HttpHmacAuthentication.AspnetCore.Tests
{
    public class HmacHttpMiddlewareTest
    {
        [DatapointSource]
        public Fixture[] fixtures = TestData.Root.Fixtures.FixtureList.ToArray();

        [Theory]
        public async Task TestThatResponseSignatureIsValid(Fixture fixture)
        {
            RequestDelegate mockNextMiddleware = (HttpContext) =>
            {
                return Task.CompletedTask;
            };

            var handler = new HmacHttpMiddleware(mockNextMiddleware, fixture.Input.Secret);

            var httpContext = HttpContextInitializer.SetupContext(fixture);

            var body = new MemoryStream(Encoding.UTF8.GetBytes(fixture.Expectations.ResponseBody));
            body.Seek(0, SeekOrigin.Begin);

            httpContext.Response.Body = body;

            await handler.Invoke(httpContext);

            var serverSignature = httpContext.Response.Headers["X-Server-Authorization-HMAC-SHA256"].ToString();

            Assert.That(serverSignature, Is.EqualTo( fixture.Expectations.ResponseSignature));
        }

    }
}