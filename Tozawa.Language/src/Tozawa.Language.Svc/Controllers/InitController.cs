

using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tozawa.Language.Svc.Models.Dtos;
using Tozawa.Language.Svc.Models.Enums;
using Tozawa.Language.Svc.Services;

namespace Tozawa.Language.Svc.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors("TozAwaCorsPolicyBff")]
    public abstract class InitController : Controller
    {
        public readonly IMediator _mediator;
        public readonly ICurrentUserService _currentUserService;
        public readonly IUserTokenService _userTokenService;
        private ActionExecutingContext Context;
        public InitController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _userTokenService = userTokenService;
        }

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
    public class CheckRoleAttribute : ActionFilterAttribute
    {
        public FunctionType[] Functions;
        public readonly ICurrentUserService _currentUserService;
        public CheckRoleAttribute(params FunctionType[] f)
        {
            Functions = f;
        }
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
            if (!currentUserService.IsAuthorizedFor(Functions))
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
    public class CheckFeatureAttribute : ActionFilterAttribute
    {
        private readonly Guid[] _organizationIds;
        private readonly int[] _featureIds;

        public CheckFeatureAttribute(params int[] featureIds)
        {
            _featureIds = featureIds;
        }

        public CheckFeatureAttribute(params Guid[] organizationIds)
        {
            _organizationIds = organizationIds;
        }

        public CheckFeatureAttribute(int[] featureIds, Guid[] organizationIds)
        {
            _featureIds = featureIds;
            _organizationIds = organizationIds;
        }

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

            if (_featureIds != null)
            {
                if (!currentUserService.HasFeatures(_featureIds))
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    throw new AccessViolationException("Forbidden");
                }
            }

            if (_organizationIds != null)
            {
                if (currentUserService.User?.OrganizationId == null || !_organizationIds.Contains(currentUserService.User.OrganizationId))
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    throw new AccessViolationException("Forbidden");
                }
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
