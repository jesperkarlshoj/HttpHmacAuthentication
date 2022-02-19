using NUnit.Framework;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace HttpHmacAuthentication.Tests
{
    [TestFixture]
    public class HmacAuthenticationDelegatingHanlderTest
    {

        [DatapointSource]
        public Fixture[] fixtures = TestData.Root.Fixtures.FixtureList.ToArray();

        [Theory]
        public async Task TestThatTimeStampHeaderIsCorrect(Fixture fixture)
        {
            var stubHandler = await CaptureStub(fixture);
            var headers = stubHandler.RequestMessage.Headers.ToDictionary(x => x.Key, x => x.Value);

            Assert.That(headers, Does.ContainKey("X-Authorization-Timestamp"));
            Assert.That(headers["X-Authorization-Timestamp"].First(), Is.EqualTo(fixture.Input.Timestamp.ToString()));

        }

        [Theory]
        public async Task TestThatAuthorizationHeaderIsCorrect(Fixture fixture)
        {
            var stubHandler = await CaptureStub(fixture);
            var headers = stubHandler.RequestMessage.Headers.ToDictionary(x => x.Key, x => x.Value);

            Assert.That(headers, Does.ContainKey("Authorization"));

            var authHeader = headers["Authorization"].First();
            Assert.That(authHeader, Is.EqualTo(fixture.Expectations.AuthorizationHeader));

        }

        [Theory]
        public async Task TestThatContentShaIsCorrect(Fixture fixture)
        {
            var stubHandler = await CaptureStub(fixture);

            var headers = stubHandler.RequestMessage.Headers.ToDictionary(x => x.Key, x => x.Value);

            if (string.IsNullOrEmpty(fixture.Input.ContentBody))
            {
                Assert.That(headers, Does.Not.ContainKey("X-Authorization-Content-SHA256"));
            }
            else
            {
                Assert.That(headers, Does.ContainKey("X-Authorization-Content-SHA256"));
                var header = headers["X-Authorization-Content-SHA256"].First();
                Assert.That(header, Is.EqualTo(fixture.Input.ContentSha));
            }
        }

        [Theory]
        public async Task TestThatAuthenticatedIdIsNotAdded(Fixture fixture)
        {
            var stubHandler = await CaptureStub(fixture);

            var headers = stubHandler.RequestMessage.Headers.ToDictionary(x => x.Key, x => x.Value);

            Assert.That(headers, Does.Not.ContainKey("X-Authenticated-Id"));
        }

        [Test]
        public async Task TestThatExceptionIsRaisedOnInvalidServerSignature()
        {
            var f = fixtures.First();
            var fixture = new Fixture
            {
                Input = f.Input,
                Expectations = new Expectation
                {
                    AuthorizationHeader = f.Expectations.AuthorizationHeader,
                    MessageSignature = f.Expectations.MessageSignature,
                    ResponseBody = f.Expectations.ResponseBody,
                    ResponseSignature = f.Expectations.ResponseSignature + "?",
                    SignableMessage = f.Expectations.SignableMessage,

                }
            };
            fixture.Expectations.ResponseSignature += "+";

        }

        private static async Task<StubHttpHandler> CaptureStub(Fixture fixture)
        {
            var uri = new Uri(fixture.Input.Url);

            var stubHandler = new StubHttpHandler(fixture.Expectations.ResponseSignature, fixture.Expectations.ResponseBody);
            var handler = new HmacAuthenticationDelegatingHanlder(fixture.Input.Realm, fixture.Input.Id, fixture.Input.Secret, fixture.Input.SignedHeaders.ToArray(), 
                new StubTimeProvider(fixture.Input.Timestamp), 
                new StubNonceProvider(fixture.Input.Nonce)) { InnerHandler = stubHandler };
            HttpClient client = new HttpClient(handler);

            var msg = new HttpRequestMessage(new HttpMethod(fixture.Input.Method), uri);
            if(fixture.Input.SignedHeaders != null && fixture.Input.SignedHeaders.Count > 0)
            {
                foreach (var header in fixture.Input.Headers)
                {
                    msg.Headers.Add(header.Key, header.Value);
                }
            }
            if(!string.IsNullOrEmpty(fixture.Input.ContentBody))
            {
                var content = new StringContent(fixture.Input.ContentBody);
                content.Headers.ContentType = new MediaTypeHeaderValue(fixture.Input.ContentType);
                msg.Content = content;
            }

            var response = await client.SendAsync(msg, CancellationToken.None);

            return stubHandler;
        }
    }
}