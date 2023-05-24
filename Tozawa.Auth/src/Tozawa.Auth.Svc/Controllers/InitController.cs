

using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tozawa.Auth.Svc.Models.Dtos;
using Tozawa.Auth.Svc.Models.Enums;
using Tozawa.Auth.Svc.Services;

namespace Tozawa.Auth.Svc.Controllers
{
    [Authorize]
    public abstract class InitController : Controller
    {
        public readonly IMediator _mediator;
        public readonly ICurrentUserService _currentUserService;
        private ActionExecutingContext Context;
        public InitController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Context = context;
            SetUser(context);

        }
        public void SetUser(ActionExecutingContext context)
        {
            var c = context.Controller as InitController;
            if (!string.IsNullOrEmpty(c.Request.GetCurrentUserHeader()))
            {
                c._currentUserService.User = System.Text.Json.JsonSerializer.Deserialize<CurrentUserDto>(c.Request.GetCurrentUserHeader());
            }
            else
            {
                c._currentUserService.User = new CurrentUserDto();
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
            if (!string.IsNullOrEmpty(c.Request.GetCurrentUserHeader()))
            {
                currentUserService.User = System.Text.Json.JsonSerializer.Deserialize<CurrentUserDto>(c.Request.GetCurrentUserHeader());
            }
            else
            {
                currentUserService.User = new CurrentUserDto();
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
            if (!string.IsNullOrEmpty(c.Request.GetCurrentUserHeader()))
            {
                c._currentUserService.User = System.Text.Json.JsonSerializer.Deserialize<CurrentUserDto>(c.Request.GetCurrentUserHeader());
            }
            else
            {
                c._currentUserService.User = new CurrentUserDto();
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
            if (!string.IsNullOrEmpty(c.Request.GetCurrentUserHeader()))
            {
                currentUserService.User = System.Text.Json.JsonSerializer.Deserialize<CurrentUserDto>(c.Request.GetCurrentUserHeader());
            }
            else
            {
                currentUserService.User = new CurrentUserDto();
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
            if (!string.IsNullOrEmpty(c.Request.GetCurrentUserHeader()))
            {
                c._currentUserService.User = System.Text.Json.JsonSerializer.Deserialize<CurrentUserDto>(c.Request.GetCurrentUserHeader());
            }
            else
            {
                c._currentUserService.User = new CurrentUserDto();
            }
        }
    }
}
