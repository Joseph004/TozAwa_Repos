using System.Net;
using Grains.Auth.Models.Dtos;
using Grains.Auth.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Grains.Auth.Controllers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class OnlyAdminAttribute : Attribute, IAsyncAuthorizationFilter, IFilterMetadata
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var currentUserService = context.HttpContext.RequestServices.GetService<ICurrentUserService>();
        var factory = context.HttpContext.RequestServices.GetService<IGrainFactory>();
        var userTokenService = context.HttpContext.RequestServices.GetService<IUserTokenService>();
        bool valid = await SetUser(context, factory, userTokenService, currentUserService);
        if (!valid)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.HttpContext.Response.WriteAsync("Forbidden", CancellationToken.None);
            throw new AccessViolationException("Forbidden");
        }

        if (!currentUserService.IsAdmin())
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.HttpContext.Response.WriteAsync("Forbidden", CancellationToken.None);
            throw new AccessViolationException("Forbidden");
        }
    }
    public async Task<bool> SetUser(AuthorizationFilterContext context, IGrainFactory factory, IUserTokenService userTokenService, ICurrentUserService currentUserService)
    {
        var result = true;
        try
        {
            if (!string.IsNullOrEmpty(context.HttpContext.Request.GetUserAuthenticationHeader()))
            {
                currentUserService.User = new CurrentUserDto();
                var oid = context.HttpContext.Request.GetUserAuthenticationHeader();
                var cachedUser = await factory.GetGrain<ILoggedInGrain>(Guid.Parse(oid)).GetAsync();
                var validate = userTokenService.ValidateCurrentToken(cachedUser.Token);
                if (!validate)
                {
                    currentUserService.User = new CurrentUserDto();
                    return false;
                }
                currentUserService.User = userTokenService.GenerateUseFromToken(cachedUser.Token);
                currentUserService.User.AccessToken = cachedUser.Token;
                currentUserService.User.RefreshToken = cachedUser.RefreshToken;
                result = true;
            }
            else
            {
                currentUserService.User = new CurrentUserDto();
                result = false;
            }
            if (!string.IsNullOrEmpty(context.HttpContext.Request.GetActiveLanguageHeader()))
            {
                currentUserService.LanguageId = Guid.Parse(context.HttpContext.Request.GetActiveLanguageHeader());
            }
        }
        catch (Exception)
        {
            currentUserService.User = new CurrentUserDto();
            return false;
        }

        return result;
    }
}