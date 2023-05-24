using Microsoft.AspNetCore.Authentication;

namespace Tozawa.Attachment.Svc.Middleware.ApiKeyMiddleware
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "API Key";
        public string Scheme { get; set; } = DefaultScheme;
        public string AuthenticationType = DefaultScheme;
        public string ApiKey { get; set; }
    }
}