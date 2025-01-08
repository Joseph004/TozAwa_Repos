

using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Grains.Auth.Models.Dtos;
using Grains.Auth.Services;
using Microsoft.Extensions.DependencyInjection;
using Grains.Models;

namespace Grains.Auth.Controllers
{
    public abstract class InitController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService, IGrainFactory factory) : Controller, IActionFilter, IAsyncActionFilter
    {
        public readonly IGrainFactory _factory = factory;
        public readonly IMediator _mediator = mediator;
        public readonly ICurrentUserService _currentUserService = currentUserService;
        private ActionExecutingContext Context;
        public readonly IUserTokenService _userTokenService = userTokenService;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Context = context;
            bool valid = await SetUser(context);
            if (valid)
            {
                await next();
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
            }
        }
        public async Task<bool> SetUser(ActionExecutingContext context)
        {
            var result = true;
            var c = context.Controller as InitController;
            try
            {
                if (!string.IsNullOrEmpty(c.Request.GetUserAuthenticationHeader()))
                {
                    c._currentUserService.User = new CurrentUserDto();
                    var oid = c.Request.GetUserAuthenticationHeader();
                    var cachedUser = await _factory.GetGrain<ILoggedInGrain>(Guid.Parse(oid)).GetAsync();
                    var validate = _userTokenService.ValidateCurrentToken(cachedUser.Token);
                    if (!validate)
                    {
                        c._currentUserService.User = new CurrentUserDto();
                        return false;
                    }
                    c._currentUserService.User = _userTokenService.GenerateUseFromToken(cachedUser.Token);
                    c._currentUserService.User.AccessToken = cachedUser.Token;
                    c._currentUserService.User.RefreshToken = cachedUser.RefreshToken;
                    result = true;
                }
                else
                {
                    c._currentUserService.User = new CurrentUserDto();
                    result = false;
                }
                if (!string.IsNullOrEmpty(c.Request.GetActiveLanguageHeader()))
                {
                    c._currentUserService.LanguageId = Guid.Parse(c.Request.GetActiveLanguageHeader());
                }
                if (!string.IsNullOrEmpty(c.Request.GetWorkingOrganizationHeader()))
                {
                    c._currentUserService.User.WorkingOrganizationId = Guid.Parse(c.Request.GetWorkingOrganizationHeader());
                }
            }
            catch (Exception)
            {
                c._currentUserService.User = new CurrentUserDto();
                return false;
            }

            return result;
        }
    }
    public class CheckRoleAttribute(params FunctionType[] f) : Attribute, IAsyncAuthorizationFilter, IFilterMetadata
    {
        public FunctionType[] Functions = f;
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var factory = context.HttpContext.RequestServices.GetService<IGrainFactory>();
            var userTokenService = context.HttpContext.RequestServices.GetService<IUserTokenService>();
            var currentUserService = context.HttpContext.RequestServices.GetService<ICurrentUserService>();
            bool valid = await SetUser(context, factory, userTokenService, currentUserService);
            if (valid)
            {
                if (!string.IsNullOrEmpty(context.HttpContext.Request.GetUserAuthenticationHeader()))
                {
                    var oid = context.HttpContext.Request.GetUserAuthenticationHeader();
                    var cachedUser = await factory.GetGrain<ILoggedInGrain>(Guid.Parse(oid)).GetAsync();
                    var validate = userTokenService.ValidateCurrentToken(cachedUser.Token);
                    if (!validate)
                    {
                        currentUserService.User = new CurrentUserDto();
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
                        return;
                    }
                    currentUserService.User = new CurrentUserDto();
                    currentUserService.User = userTokenService.GenerateUseFromToken(cachedUser.Token);
                    currentUserService.User.AccessToken = cachedUser.Token;
                    currentUserService.User.RefreshToken = cachedUser.RefreshToken;
                }
                else
                {
                    currentUserService.User = new CurrentUserDto();
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
                    return;
                }
                if (!string.IsNullOrEmpty(context.HttpContext.Request.GetActiveLanguageHeader()))
                {
                    currentUserService.LanguageId = Guid.Parse(context.HttpContext.Request.GetActiveLanguageHeader());
                }
                if (!string.IsNullOrEmpty(context.HttpContext.Request.GetWorkingOrganizationHeader()))
                {
                    currentUserService.User.WorkingOrganizationId = Guid.Parse(context.HttpContext.Request.GetWorkingOrganizationHeader());
                }
                if (!currentUserService.IsAuthorizedFor(Functions))
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
                    return;
                }
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new ContentResult() { Content = "Oops, you are not authorized please contact support!!!", StatusCode = (int)HttpStatusCode.Unauthorized };
                return;
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
                if (!string.IsNullOrEmpty(context.HttpContext.Request.GetWorkingOrganizationHeader()))
                {
                    currentUserService.User.WorkingOrganizationId = Guid.Parse(context.HttpContext.Request.GetWorkingOrganizationHeader());
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
}
