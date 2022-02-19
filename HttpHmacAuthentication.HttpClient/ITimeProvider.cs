
namespace HttpHmacAuthentication.HttpClient
{
    internal interface ITimeProvider
    {
        long UnixTimeUtc { get; }
    }
}