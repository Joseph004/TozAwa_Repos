
namespace Tozawa.Language.Svc.Controllers
{
    public static class HttpRequestExtension
    {
        private static readonly string currentUserHeaderKey = "current-user";
        private static readonly string activeLanguageHeaderKey = "toza-active-language";
        private static readonly string userAuthenticationHeaderKey = "tzuserauthentication";

        public static string GetUserAuthenticationHeader(this HttpRequest request)
        {
            return request.GetHeader(userAuthenticationHeaderKey);
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
