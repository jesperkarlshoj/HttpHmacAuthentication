using Microsoft.AspNetCore.Authentication;

namespace HttpHmacAuthentication.AspnetCore
{
    public class HmacAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string? Secret { get; set; }
    }

}
