
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TozawaNGO.Auth.Services;

namespace TozawaNGO.Helpers
{
    public class AuthorizeUserRequirement : Attribute, IAuthorizationFilter
    {
        private static readonly string userAuthenticationHeaderKey = "tzuserauthentication";

        public static string GetUserAuthenticationHeader(HttpRequest request)
        {
            return GetHeader(request, userAuthenticationHeaderKey);
        }
        public static string GetHeader(HttpRequest request, string key)
        {
            return request.Headers.Any(x => x.Key == key) ? request.Headers.First(x => x.Key == key).Value.First() : string.Empty;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userTokenService = context.HttpContext.RequestServices.GetService<IUserTokenService>();

            if (!string.IsNullOrEmpty(GetUserAuthenticationHeader(context.HttpContext.Request)))
            {
                if (!userTokenService.ValidateCurrentToken(GetUserAuthenticationHeader(context.HttpContext.Request)))
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
                }
            }
        }
    }

    public class AuthorizeUserRequirementWithNoExpireToken : Attribute, IAuthorizationFilter
    {
        private static readonly string userAuthenticationHeaderKey = "tzuserauthentication";

        public static string GetUserAuthenticationHeader(HttpRequest request)
        {
            return GetHeader(request, userAuthenticationHeaderKey);
        }
        public static string GetHeader(HttpRequest request, string key)
        {
            return request.Headers.Any(x => x.Key == key) ? request.Headers.First(x => x.Key == key).Value.First() : string.Empty;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userTokenService = context.HttpContext.RequestServices.GetService<IUserTokenService>();

            if (!string.IsNullOrEmpty(GetUserAuthenticationHeader(context.HttpContext.Request)))
            {
                if (!userTokenService.ValidateCurrentTokenByIgnoreExpire(GetUserAuthenticationHeader(context.HttpContext.Request)))
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
                }
            }
        }
    }
}