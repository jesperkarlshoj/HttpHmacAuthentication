using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpHmacAuthentication.AspnetCore.Tests
{
    public class HmacAuthenticationHandlerTest
    {

        [DatapointSource]
        public Fixture[] fixtures = TestData.Root.Fixtures.FixtureList.ToArray();

        private Mock<ISystemClock> clock;

        [SetUp]
        public void Setup()
        {
            clock = new Mock<ISystemClock>();

        }

        [Test]
        public async Task AssertFailWhenNoHeader()
        {
            

            var handler = new HmacAuthenticationHandler(new TestOptionsMonitor<HmacAuthenticationSchemeOptions>(new HmacAuthenticationSchemeOptions()), NullLoggerFactory.Instance, null, clock.Object);

            var context = new DefaultHttpContext();

            await handler.InitializeAsync(new AuthenticationScheme(HmacAuthenticationHandler.AuthScheme, HmacAuthenticationHandler.AuthScheme, typeof(HmacAuthenticationHandler)), context);
            var result = await handler.AuthenticateAsync();


            Assert.That(result.Succeeded, Is.False);
        }



        [Theory]
        public async Task TestThatHeaderIsValid(Fixture fixture)
        {
            clock.Setup(x => x.UtcNow).Returns(DateTimeOffset.FromUnixTimeSeconds(fixture.Input.Timestamp));

            var opt = new HmacAuthenticationSchemeOptions()
            {
                Secret = fixture.Input.Secret,
            };

            var handler = new HmacAuthenticationHandler(new TestOptionsMonitor<HmacAuthenticationSchemeOptions>(opt), NullLoggerFactory.Instance, null, clock.Object);

            var context = new DefaultHttpContext();
            context.Request.Method = fixture.Input.Method;
            context.Request.Host = new HostString(fixture.Input.Host);
            context.Request.Path = new Uri(fixture.Input.Url).AbsolutePath;
            context.Request.QueryString = new QueryString(new Uri(fixture.Input.Url).Query);
            context.Request.Headers.Add("X-Authorization-Timestamp", fixture.Input.Timestamp.ToString());
            context.Request.Headers.Add("Authorization", fixture.Expectations.AuthorizationHeader);
            

            foreach (var header in fixture.Input.Headers)
            {
                context.Request.Headers.Add(header.Key, header.Value);
            }

            if (!string.IsNullOrEmpty(fixture.Input.ContentBody))
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(fixture.Input.ContentBody));
                context.Request.Body = stream;
                context.Request.Headers.Add("Content-Type", fixture.Input.ContentType);
                context.Request.ContentLength = stream.Length;
            }

            await handler.InitializeAsync(new AuthenticationScheme(HmacAuthenticationHandler.AuthScheme, HmacAuthenticationHandler.AuthScheme, typeof(HmacAuthenticationHandler)), context);
            var result = await handler.AuthenticateAsync();


            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task TestThatOldTimestampIsRejected()
        {
            clock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

            var fixture = fixtures[0];

            var opt = new HmacAuthenticationSchemeOptions()
            {
                Secret = fixture.Input.Secret,
            };

            var handler = new HmacAuthenticationHandler(new TestOptionsMonitor<HmacAuthenticationSchemeOptions>(opt), NullLoggerFactory.Instance, null, clock.Object);

            var context = new DefaultHttpContext();
            context.Request.Method = fixture.Input.Method;
            context.Request.Host = new HostString(fixture.Input.Host);
            context.Request.Path = new Uri(fixture.Input.Url).AbsolutePath;
            context.Request.QueryString = new QueryString(new Uri(fixture.Input.Url).Query);
            context.Request.Headers.Add("X-Authorization-Timestamp", fixture.Input.Timestamp.ToString());
            context.Request.Headers.Add("Authorization", fixture.Expectations.AuthorizationHeader);


            foreach (var header in fixture.Input.Headers)
            {
                context.Request.Headers.Add(header.Key, header.Value);
            }

            if (!string.IsNullOrEmpty(fixture.Input.ContentBody))
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(fixture.Input.ContentBody));
                context.Request.Body = stream;
                context.Request.Headers.Add("Content-Type", fixture.Input.ContentType);
                context.Request.ContentLength = stream.Length;
            }

            await handler.InitializeAsync(new AuthenticationScheme(HmacAuthenticationHandler.AuthScheme, HmacAuthenticationHandler.AuthScheme, typeof(HmacAuthenticationHandler)), context);
            var result = await handler.AuthenticateAsync();


            Assert.That(result.Succeeded, Is.False);


        }

    }

    public class TestOptionsMonitor<T> : IOptionsMonitor<T>
    where T : class, new()
    {
        public TestOptionsMonitor(T currentValue)
        {
            CurrentValue = currentValue;
        }

        public T Get(string name)
        {
            return CurrentValue;
        }

        public IDisposable OnChange(Action<T, string> listener)
        {
            throw new NotImplementedException();
        }

        public T CurrentValue { get; }
    }
}