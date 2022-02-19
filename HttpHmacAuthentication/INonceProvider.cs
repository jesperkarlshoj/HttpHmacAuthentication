
namespace HttpHmacAuthentication
{
    internal interface INonceProvider
    {
        string Nonce { get; }
    }
}