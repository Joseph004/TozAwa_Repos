
using Microsoft.AspNetCore.Http;

namespace Tozawa.Attachment.Svc.Controllers
{
    public static class HttpRequestExtension
    {
        private static readonly string currentUserHeaderKey = "current-user";
        private static readonly string activeLanguageHeaderKey = "toza-active-language";

        public static string GetCurrentUserHeader(this HttpRequest request)
        {
            return request.GetHeader(currentUserHeaderKey);
        }
        public static string GetActiveLanguageHeader(this HttpRequest request)
        {
            return request.GetHeader(activeLanguageHeaderKey);
        }
        public static string GetHeader(this HttpRequest request, string key)
        {
            return request.Headers.Any(x => x.Key == key) ? request.Headers.First(x => x.Key == key).Value.First() : string.Empty;
        }
    }
}
