
namespace HttpHmacAuthentication
{
    internal class TimeProvider : ITimeProvider
    {
        public long UnixTimeUtc => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}