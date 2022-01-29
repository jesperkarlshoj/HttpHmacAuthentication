using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;

[assembly: InternalsVisibleTo("HttpHmacAuthentication.Tests")]

namespace HttpHmacAuthentication
{
    public class HmacAuthenticationDelegatingHanlder : DelegatingHandler
    {
        private const string LineBreak = "\n";
        private const string Version = "2.0";
        
        private readonly ITimeProvider timeProvider;
        private readonly INonceProvider nonceProvider;

        private readonly string accessKey;
        private readonly string realm;
        private readonly string secret;
        private readonly string[] signableHeaders;

        public HmacAuthenticationDelegatingHanlder(string realm, string accessKey, string secret) : this(realm, accessKey, secret, Array.Empty<string>(), new TimeProvider(), new NonceProvider())
        {

        }

        public HmacAuthenticationDelegatingHanlder(string realm, string accessKey, string secret, string[] signableHeaders) : this(realm, accessKey, secret, signableHeaders, new TimeProvider(), new NonceProvider())
        {
            
        }

        internal HmacAuthenticationDelegatingHanlder(string realm, string accessKey, string secret, string[] signableHeaders, ITimeProvider timeProvider, INonceProvider nonceProvider)
        {
            this.timeProvider = timeProvider;
            this.nonceProvider = nonceProvider;

            this.realm = realm;
            this.accessKey = accessKey;
            this.secret = secret;
            this.signableHeaders = signableHeaders;

            if (signableHeaders == null) 
            {
                this.signableHeaders = Array.Empty<string>();
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var timestamp = timeProvider.UnixTimeUtc.ToString();
            var nonce = nonceProvider.Nonce;

            var AdditionalSignedHeaderNames = signableHeaders.Length == 0 ? "" : string.Concat(signableHeaders.Select(x => x + ":" + string.Join(",", request.Headers.GetValues(x)) + LineBreak)).ToLower();

            var contentSha = string.Empty;
            if(request.Content != null)
            {
                SHA256 sha256 = new SHA256Managed();
                var checksum = sha256.ComputeHash(request.Content.ReadAsStream());
                contentSha = Convert.ToBase64String(checksum);
            }

            var AuthorizationHeaderParameters = "id=" + URLEncode(accessKey) + "&" +
               "nonce=" + URLEncode(nonce) + "&" +
               "realm=" + URLEncode(realm) + "&" +
               "version=" + URLEncode(Version);

            var StringToSign = request.Method.Method + LineBreak +
               request.RequestUri.Host + LineBreak +
               request.RequestUri.AbsolutePath + LineBreak +
               (request.RequestUri.Query.Length > 0 ? request.RequestUri.Query.Remove(0, 1) : "") + LineBreak +
               AuthorizationHeaderParameters + LineBreak +
               (signableHeaders.Length == 0 ? "" : (AdditionalSignedHeaderNames.ToLower() ))  +
               timestamp;

            if (!string.IsNullOrEmpty(contentSha))
            {
                StringToSign +=
                    LineBreak +
                    string.Join(",", request.Content.Headers.GetValues("Content-Type")) + LineBreak +
                    contentSha;
            }

            var key = Convert.FromBase64String(secret);
            using HMACSHA256 hmac = new HMACSHA256(key);
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(StringToSign));

            var signature = Convert.ToBase64String(hashValue);

            var AdditionalHeaderNames = signableHeaders.Length == 0 ? "" : string.Join(";", signableHeaders);
            var hmacAuthorization = "" +
                (AdditionalHeaderNames.Length == 0 ? "" : ("headers="+DoubleQuoteEnclose(URLEncode(AdditionalHeaderNames)) + ",")) +
                "id=" + DoubleQuoteEnclose(URLEncode(accessKey)) + "," +
                "nonce=" + DoubleQuoteEnclose(URLEncode(nonce)) + "," +
                "realm=" + DoubleQuoteEnclose(URLEncode(realm)) + "," +
                "signature=" + DoubleQuoteEnclose(URLEncode(signature)) + "," +
                "version=" + DoubleQuoteEnclose(URLEncode(Version));

            request.Headers.Add("X-Authorization-Timestamp", timestamp);

            if (!string.IsNullOrEmpty(contentSha))
            {
                request.Headers.Add("X-Authorization-Content-SHA256", contentSha);
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("acquia-http-hmac", hmacAuthorization);

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            var responseStringToSign =
                nonce + LineBreak +
                timestamp + LineBreak +
                await response.Content.ReadAsStringAsync();

            byte[] serverHashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(responseStringToSign));
            var serverSignature = Convert.ToBase64String(serverHashValue);

            if(serverSignature != response.Headers.GetValues("X-Server-Authorization-HMAC-SHA256").First())
            {
                throw new Exception("Invalid signature returned from server");
            }

            return response;
        }

        

        private static string URLEncode(string data)
        {
            return HttpUtility.UrlPathEncode(data).Replace(";", "%3B");
        }

        private static string DoubleQuoteEnclose(string data)
        {
            return $"\"{data}\"";
        }
    }
}