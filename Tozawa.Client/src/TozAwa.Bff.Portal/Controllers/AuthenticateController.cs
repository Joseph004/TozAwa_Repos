using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Handlers.Commands;
using Tozawa.Bff.Portal.Handlers.Queries;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    //[Authorize(AuthenticationSchemes = "tzappauthentication")]
    [EnableCors("TozAwaCorsPolicyBff")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthenticateController : InitController
    {
        public AuthenticateController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService, ILanguageService languageService, AppSettings appSettings)
        : base(mediator, currentUserService, userTokenService)
        {
            UpdateMessages.Configure(languageService, appSettings, currentUserService);
        }

        [HttpGet, Route("current/{oid:Guid}")]
        public async Task<IActionResult> GetCurrentUser(Guid oid) => Ok(await _mediator.Send(new GetCurrentUserQuery(oid)));

        [HttpPost, Route("signin")]
        public async Task<ActionResult> SignInPost([FromBody] LoginCommand request) => Ok(await _mediator.Send(request));

        [HttpPost, Route("root/{userName}")]
        public async Task<ActionResult> CheckLockout(string userName) => Ok(await _mediator.Send(new LoginAttemptCommand { UserName = userName }));
    }
}