
namespace HttpHmacAuthentication.HttpClient
{
    internal interface INonceProvider
    {
        string Nonce { get; }
    }
}