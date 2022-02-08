using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
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

            var context = HttpContextInitializer.SetupContext(fixture);
            var result = await Authenticate(context, fixture.Input.Secret);


            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task TestThatOldTimestampIsRejected()
        {
            clock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
            var fixture = fixtures[0];

            var context = HttpContextInitializer.SetupContext(fixture);
            var result = await Authenticate(context, fixture.Input.Secret);


            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task TestThatAuthenticationIdHeaderIsRejected()
        { 
            var fixture = fixtures[0];
            clock.Setup(x => x.UtcNow).Returns(DateTimeOffset.FromUnixTimeSeconds(fixture.Input.Timestamp));

            var context = HttpContextInitializer.SetupContext(fixture);
            context.Request.Headers.Add("X-Authenticated-Id", "123");

            var result = await Authenticate(context, fixture.Input.Secret);

            Assert.That(result.Succeeded, Is.False);
        }

     

        private async Task<AuthenticateResult> Authenticate(DefaultHttpContext context, string secret)
        {
            var opt = new HmacAuthenticationSchemeOptions()
            {
                Secret = secret,
            };

            var handler = new HmacAuthenticationHandler(new TestOptionsMonitor<HmacAuthenticationSchemeOptions>(opt), NullLoggerFactory.Instance, null, clock.Object);

            await handler.InitializeAsync(new AuthenticationScheme(HmacAuthenticationHandler.AuthScheme, HmacAuthenticationHandler.AuthScheme, typeof(HmacAuthenticationHandler)), context);
            var result = await handler.AuthenticateAsync();

            return result;
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