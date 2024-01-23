

using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TozawaNGO.Auth.Models.Dtos;
using TozawaNGO.Auth.Services;

namespace TozawaNGO.Auth.Controllers
{
    public abstract class InitController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService) : Controller
    {
        public readonly IMediator _mediator = mediator;
        public readonly ICurrentUserService _currentUserService = currentUserService;
        private ActionExecutingContext Context;
        public readonly IUserTokenService _userTokenService = userTokenService;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Context = context;
            SetUser(context);
        }
        public void SetUser(ActionExecutingContext context)
        {
            var c = context.Controller as InitController;
            if (!string.IsNullOrEmpty(c.Request.GetUserAuthenticationHeader()))
            {
                c._currentUserService.User = new CurrentUserDto();
                var token = c.Request.GetUserAuthenticationHeader();
                c._currentUserService.User = _userTokenService.GenerateUseFromToken(token);
                c._currentUserService.User.AccessToken = token;
            }
            else
            {
                c._currentUserService.User = new CurrentUserDto();
            }
            if (!string.IsNullOrEmpty(c.Request.GetActiveLanguageHeader()))
            {
                c._currentUserService.LanguageId = Guid.Parse(c.Request.GetActiveLanguageHeader());
            }
        }
    }
    public class CheckRoleAttribute(params RoleDto[] r) : ActionFilterAttribute
    {
        public RoleDto[] Roles = r;
        public readonly ICurrentUserService _currentUserService;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            SetUser(context);
            var c = context.Controller as InitController;
            var currentUserService = context.HttpContext.RequestServices.GetService<ICurrentUserService>();
            if (!string.IsNullOrEmpty(c.Request.GetUserAuthenticationHeader()))
            {
                c._currentUserService.User = new CurrentUserDto();
                var token = c.Request.GetUserAuthenticationHeader();
                c._currentUserService.User = c._userTokenService.GenerateUseFromToken(token);
                c._currentUserService.User.AccessToken = token;
            }
            else
            {
                c._currentUserService.User = new CurrentUserDto();
            }
            if (!string.IsNullOrEmpty(c.Request.GetActiveLanguageHeader()))
            {
                c._currentUserService.LanguageId = Guid.Parse(c.Request.GetActiveLanguageHeader());
            }
            if (!currentUserService.IsAuthorizedFor(Roles))
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                throw new AccessViolationException("Forbidden");
            }
        }
        public void SetUser(ActionExecutingContext context)
        {
            var c = context.Controller as InitController;
            if (!string.IsNullOrEmpty(c.Request.GetUserAuthenticationHeader()))
            {
                c._currentUserService.User = new CurrentUserDto();
                var token = c.Request.GetUserAuthenticationHeader();
                c._currentUserService.User = c._userTokenService.GenerateUseFromToken(token);
                c._currentUserService.User.AccessToken = token;
            }
            else
            {
                c._currentUserService.User = new CurrentUserDto();
            }
            if (!string.IsNullOrEmpty(c.Request.GetActiveLanguageHeader()))
            {
                c._currentUserService.LanguageId = Guid.Parse(c.Request.GetActiveLanguageHeader());
            }
        }
    }
}
