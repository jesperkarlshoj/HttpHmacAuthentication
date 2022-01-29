
namespace HttpHmacAuthentication
{
    internal interface ITimeProvider
    {
        long UnixTimeUtc { get; }
    }
}