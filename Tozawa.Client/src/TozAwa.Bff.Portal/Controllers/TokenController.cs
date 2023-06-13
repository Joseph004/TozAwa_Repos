using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Tozawa.Bff.Portal.Handlers.Commands;
using Tozawa.Bff.Portal.Handlers.Queries;
using Tozawa.Bff.Portal.Helper;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    [EnableCors("TozAwaCorsPolicyBff")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TokenController : InitController
    {
        public TokenController(IMediator mediator, ICurrentUserService currentUserService, IUserTokenService userTokenService)
        : base(mediator, currentUserService, userTokenService)
        {
        }

        [HttpPost, Route("refresh")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenDto tokenDto) => Ok(await _mediator.Send(new RefreshTokenCommand(tokenDto)));
    }
}