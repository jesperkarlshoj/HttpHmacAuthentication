namespace HttpHmacAuthentication.HttpClient.Tests
{
    public class StubNonceProvider : INonceProvider
    {
        public string Nonce { get; set; }

        public StubNonceProvider(string nonce)
        {
            Nonce = nonce;
        }
    }
}