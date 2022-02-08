using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;

namespace HttpHmacAuthentication.AspnetCore.Tests
{
    public static class HttpContextInitializer
    {
        public static DefaultHttpContext SetupContext(Fixture fixture)
        {


            var context = new DefaultHttpContext();
            context.Request.Method = fixture.Input.Method;
            context.Request.Host = new HostString(fixture.Input.Host);
            context.Request.Path = new Uri(fixture.Input.Url).AbsolutePath;
            context.Request.QueryString = new QueryString(new Uri(fixture.Input.Url).Query);
            context.Request.Headers.Add("X-Authorization-Timestamp", fixture.Input.Timestamp.ToString());
            context.Request.Headers.Add("Authorization", fixture.Expectations.AuthorizationHeader);


            foreach (var header in fixture.Input.Headers)
            {
                context.Request.Headers.Add(header.Key, header.Value);
            }

            if (!string.IsNullOrEmpty(fixture.Input.ContentBody))
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(fixture.Input.ContentBody));
                context.Request.Body = stream;
                context.Request.Headers.Add("Content-Type", fixture.Input.ContentType);
                context.Request.ContentLength = stream.Length;
            }

            return context;
        }
    }
}