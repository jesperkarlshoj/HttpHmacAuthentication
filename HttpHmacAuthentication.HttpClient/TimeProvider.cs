
namespace HttpHmacAuthentication.HttpClient
{
    internal class TimeProvider : ITimeProvider
    {
        public long UnixTimeUtc => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}