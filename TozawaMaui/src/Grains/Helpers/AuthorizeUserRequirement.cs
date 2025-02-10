
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Grains.Auth.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Grains.Helpers
{
    public class AuthorizeUserRequirement : Attribute, IAsyncAuthorizationFilter
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
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var factory = context.HttpContext.RequestServices.GetService<IGrainFactory>();
            var userTokenService = context.HttpContext.RequestServices.GetService<IUserTokenService>();

            if (!string.IsNullOrEmpty(GetUserAuthenticationHeader(context.HttpContext.Request)))
            {
                var oid = GetUserAuthenticationHeader(context.HttpContext.Request);
                var cachedUser = await factory.GetGrain<ILoggedInGrain>(Guid.Parse(oid)).GetAsync();
                if (cachedUser == null || string.IsNullOrEmpty(cachedUser.Token))
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
                    return;
                }

                if (!userTokenService.ValidateCurrentToken(cachedUser.Token))
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
                }
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
            }
        }
    }

    public class AuthorizeUserRequirementWithNoExpireToken : Attribute, IAsyncAuthorizationFilter
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
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userTokenService = context.HttpContext.RequestServices.GetService<IUserTokenService>();
            var factory = context.HttpContext.RequestServices.GetService<IGrainFactory>();

            if (!string.IsNullOrEmpty(GetUserAuthenticationHeader(context.HttpContext.Request)))
            {
                var oid = GetUserAuthenticationHeader(context.HttpContext.Request);
                var cachedUser = await factory.GetGrain<ILoggedInGrain>(Guid.Parse(oid)).GetAsync();
                if (cachedUser == null || string.IsNullOrEmpty(cachedUser.Token))
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
                    return;
                }

                if (!userTokenService.ValidateCurrentTokenByIgnoreExpire(cachedUser.Token))
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
                }
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
            }
        }
    }
}