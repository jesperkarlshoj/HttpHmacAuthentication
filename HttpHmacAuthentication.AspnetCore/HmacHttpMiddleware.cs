using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HttpHmacAuthentication.AspnetCore
{
    public  class HmacHttpMiddleware
    {
        private const string LineBreak = "\n";

        private readonly RequestDelegate _next;
        private readonly string secret;

        public HmacHttpMiddleware(RequestDelegate next, string secret)
        {
            _next = next;
            this.secret = secret;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(HmacHeaderNames.TimeStamp, out var timestamp))
            {
                timestamp = "0";
            }


            var header = context.Request.Headers[HeaderNames.Authorization].ToString();

            var nonce = header.Substring(header.IndexOf("nonce") + 7, 36);


            await _next.Invoke(context);

            var key = Convert.FromBase64String(secret);
            using HMACSHA256 hmac = new HMACSHA256(key);

            using var sr = new StreamReader(context.Response.Body);

            var responseStringToSign =
               nonce + LineBreak +
               timestamp + LineBreak +
               await sr.ReadToEndAsync();

            byte[] serverHashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(responseStringToSign));
            var serverSignature = Convert.ToBase64String(serverHashValue);

            context.Response.Headers.Add("X-Server-Authorization-HMAC-SHA256", serverSignature);
            
        }

     
    }

    public static class HmacMiddlewareExtensions
    {
        public static IApplicationBuilder UseHmacResponseSignMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HmacHttpMiddleware>();
        }
    }
}
