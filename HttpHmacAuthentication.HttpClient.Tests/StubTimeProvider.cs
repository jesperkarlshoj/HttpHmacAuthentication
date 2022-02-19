namespace HttpHmacAuthentication.HttpClient.Tests
{
    public class StubTimeProvider : ITimeProvider
    {
        public long UnixTimeUtc {get; set;}

        public StubTimeProvider(long time)
        {
            UnixTimeUtc = time;
        }
    }
}