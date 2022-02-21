
using System;

namespace HttpHmacAuthentication
{
    internal class NonceProvider : INonceProvider
    {
        public string Nonce => Guid.NewGuid().ToString(); //TODO make random!
    }
}