
using Microsoft.AspNetCore.Http;

namespace Grains.Auth.Controllers
{
    public static class HttpRequestExtension
    {
        private static readonly string userAuthenticationHeaderKey = "tzuserauthentication";
        private static readonly string activeLanguageHeaderKey = "toza-active-language";
        private static readonly string workingOraganizationKey = "working-organization";
        public static string GetUserAuthenticationHeader(this HttpRequest request)
        {
            return request.GetHeader(userAuthenticationHeaderKey);
        }
        public static string GetActiveLanguageHeader(this HttpRequest request)
        {
            return request.GetHeader(activeLanguageHeaderKey);
        }
        public static string GetWorkingOrganizationHeader(this HttpRequest request)
        {
            return request.GetHeader(workingOraganizationKey);
        }
        public static string GetHeader(this HttpRequest request, string key)
        {
            return request.Headers.Any(x => x.Key == key) ? request.Headers.First(x => x.Key == key).Value.First() : string.Empty;
        }
    }
}
