using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Web;

namespace HttpHmacAuthentication.AspnetCore
{
    public class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationSchemeOptions>
    {
        public const string AuthScheme = "acquia-http-hmac";
        private const string TokenRegex = $"{AuthScheme} (?<token>.*)";
        private const string LineBreak = "\n";
        private const string Version = "2.0";

        private static readonly TimeSpan maxTimeOffset = TimeSpan.FromSeconds(30);

        public HmacAuthenticationHandler(IOptionsMonitor<HmacAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                Logger.LogInformation("Http request does not contain Aurhorization header");
                return Task.FromResult(AuthenticateResult.Fail("Header not found"));
            }

            var header = Request.Headers[HeaderNames.Authorization].ToString();
            var tokenMatch = Regex.Match(header, TokenRegex);

            if (!tokenMatch.Success)
            {
                Logger.LogInformation("Authorization has wrong scheme");
                return Task.FromResult(AuthenticateResult.Fail("Invalid authorization header"));
            }

            var token = tokenMatch.Groups["token"].Value;

            try
            {
                var t = ExtractTicket(token);


                var claims = new Claim[] { };

                var claimsIdentity = new ClaimsIdentity(claims, nameof(HmacAuthenticationHandler));

                var ticket = new AuthenticationTicket( new ClaimsPrincipal(claimsIdentity), Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail("Failed"));
            }
            

            
        }

        private AuthenticationTicket ExtractTicket(string token)
        {
            if(!Request.Headers.TryGetValue(HmacHeaderNames.TimeStamp, out var timestamp))
            {
                throw new ArgumentException("Missing X-Authorization-Timestamp header");
            }

            var givenTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestamp));
            var offset = Clock.UtcNow - givenTime;


            if(Math.Abs(offset.TotalMilliseconds) > maxTimeOffset.TotalMilliseconds)
            {
                throw new Exception("Time offset too big");
            }


            var tokenRegex = new Regex("(\\w.*?)=\\\"(\\w.*?)\\\"");
            var tokenElements = token.Split(",").Select(t => {
                var m = tokenRegex.Match(t);

                return new string[] { m.Groups[1].Value, m.Groups[2].Value };

            }).ToDictionary(entry => entry[0], entry => entry[1]);


            string contentSha = string.Empty;

            if (Request.ContentLength > 0)
            {
                Request.EnableRewind();

                SHA256 sha256 = new SHA256Managed();
                var checksum = sha256.ComputeHash(Request.Body);
                contentSha = Convert.ToBase64String(checksum);

                Request.Body.Seek(0, SeekOrigin.Begin);
            }


            tokenElements.TryGetValue("headers", out var headers);
            headers = headers?.Replace("\"", "") ?? string.Empty;

            tokenElements.TryGetValue("id", out var id);
            id = id?.Replace("\"", "");

            tokenElements.TryGetValue("nonce", out var nonce);
            nonce = nonce?.Replace("\"", "");

            tokenElements.TryGetValue("realm", out var realm);
            realm = realm?.Replace("\"", "");

            tokenElements.TryGetValue("signature", out var signature);
            signature = signature?.Replace("\"", "");

            tokenElements.TryGetValue("version", out var version);
            version = version?.Replace("\"", "");

            var AdditionalSignedHeaderNames = string.Empty;
            if (!string.IsNullOrEmpty(headers))
            {
                var headerKeys = headers.Split("%3B");

                foreach (var headerKey in headerKeys)
                {
                    Request.Headers.TryGetValue(headerKey, out var headerValue);

                    AdditionalSignedHeaderNames += headerKey.ToLower() + ":" + headerValue.ToString().ToLower() + LineBreak;
                }
            }

            var AuthorizationHeaderParameters = "id=" + URLEncode(id) + "&" +
               "nonce=" + URLEncode(nonce) + "&" +
               "realm=" + URLEncode(realm) + "&" +
               "version=" + URLEncode(Version);

            var stringToSign = Request.Method + LineBreak +
              Request.Host + LineBreak +
              Request.Path + LineBreak +
              (Request.QueryString.HasValue ? Request.QueryString.Value.Remove(0, 1) : "") + LineBreak +
              AuthorizationHeaderParameters + LineBreak +
              (headers.Length == 0 ? "" : (AdditionalSignedHeaderNames.ToLower())) +
              timestamp;

            if (!string.IsNullOrEmpty(contentSha))
            {
                Request.Headers.TryGetValue("Content-Type", out var contentType);
                stringToSign +=
                    LineBreak +
                    contentType + LineBreak +
                    contentSha;
            }

            var key = Convert.FromBase64String(Options.Secret);
            using HMACSHA256 hmac = new HMACSHA256(key);
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));

            var calculatedSignature = Convert.ToBase64String(hashValue);

            

            if(string.CompareOrdinal(signature, calculatedSignature) == 0)
            {
                var claims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, id) };

                var claimsIdentity = new ClaimsIdentity(claims, nameof(HmacAuthenticationHandler));

                var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);
                return ticket;
            }

            throw new Exception("Failed");
        }

        private static string URLEncode(string data)
        {
            return HttpUtility.UrlPathEncode(data).Replace(";", "%3B");
        }
    }

}
